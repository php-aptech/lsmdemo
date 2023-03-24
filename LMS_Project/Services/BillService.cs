using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class BillService
    {
        public static async Task<tbl_Bill> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Bill.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public class Get_ClassAvailable
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Thumbnail { get; set; }
            public int MaxQuantity { get; set; }
            public int StudentQuantity { get; set; }
            /// <summary>
            /// true = có thể đăng ký
            /// </summary>
			public bool Fit { get; set; }
            public string Note { get; set; }
            public double? Price { get; set; }
        }
        public class GetClassAvailableSearch
        {
            public int StudentId { get; set; }
            public int BranchId { get; set; }
            public string Search { get; set; }
            public int ProgramId { get; set; }
        }
        /// <summary>
        /// Lấy danh sách lớp học khi đăng ký
        /// </summary>
        /// <param name="StudentId"></param>
        /// <param name="BranchId"></param>
        /// <returns></returns>
        public static async Task<List<Get_ClassAvailable>> GetClassAvailable(GetClassAvailableSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_ClassAvailable @StudentId = {baseSearch.StudentId}," +
                    $"@BranchId = {baseSearch.BranchId}," +
                    $"@ProgramId = {baseSearch.ProgramId}," +
                    $"@Search = N'{baseSearch.Search}'";
                var data = await db.Database.SqlQuery<Get_ClassAvailable>(sql).ToListAsync();
                return data;
            }
        }
        /// <summary>
        /// Đăng ký học
        /// </summary>
        /// <param name="itemModel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<tbl_Bill> Insert(BillCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == itemModel.StudentId && x.RoleId == 3);
                        if (student == null)
                            throw new Exception("Không tìm thấy học viên");
                        var branch = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == itemModel.BranchId);
                        if (branch == null)
                            throw new Exception("Không tìm thấy trung tâm");

                        var model = new tbl_Bill(itemModel);
                        model.CreatedBy = model.ModifiedBy = user.FullName;
                        db.tbl_Bill.Add(model);
                        await db.SaveChangesAsync();



                        //Kiểm tra quyền thanh toán, nếu không có quyền thanh toán sẽ gửi yêu cầu duyệt
                        var paymentAllow = await db.tbl_PaymentAllow.AnyAsync(x => x.UserId == user.UserInformationId && x.Enable == true);
                        if (!paymentAllow && user.RoleId != ((int)RoleEnum.admin) && user.RoleId != ((int)RoleEnum.accountant))
                        {
                            double money = itemModel.Paid;
                            itemModel.Paid = 0;
                            model.Paid = 0;
                            var paymentApprove = new tbl_PaymentApprove
                            {
                                BillId = model.Id,
                                CreatedBy = user.FullName,
                                CreatedOn = DateTime.Now,
                                Enable = true,
                                ModifiedBy = user.FullName,
                                ModifiedOn = DateTime.Now,
                                Money = money,
                                Note = itemModel.Note,
                                Status = 1,
                                StatusName = "Chờ duyệt",
                                UserId = user.UserInformationId
                            };
                            db.tbl_PaymentApprove.Add(paymentApprove);
                            await db.SaveChangesAsync();
                        }

                        double totalPrice = 0;
                        double reduced = 0;
                        if (itemModel.Details.Any())
                        {
                            foreach (var item in itemModel.Details)
                            {
                                if (itemModel.Type == 1)//Đăng ký học
                                {
                                    var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == item.ClassId);
                                    if (_class != null)
                                    {
                                        if (_class.Status == 3)
                                            throw new Exception("Lớp học đã kết thúc");
                                        var detail = new tbl_BillDetail(item);
                                        detail.CreatedBy = detail.ModifiedBy = user.FullName;
                                        detail.BillId = model.Id;
                                        detail.ProgramId = _class.ProgramId;
                                        detail.Quantity = 1;
                                        detail.Price = _class.Price;
                                        detail.TotalPrice = _class.Price;
                                        detail.StudentId = model.StudentId;
                                        totalPrice += _class.Price;
                                        db.tbl_BillDetail.Add(detail);
                                        var checkExist = await db.tbl_StudentInClass.AnyAsync(x => x.StudentId == model.StudentId && x.ClassId == _class.Id && x.Enable == true);
                                        if (checkExist)
                                            throw new Exception($"Học viên đã có trong lớp {_class.Name}");
                                        db.tbl_StudentInClass.Add(new tbl_StudentInClass
                                        {
                                            BranchId = _class.BranchId,
                                            ModifiedBy = user.FullName,
                                            ClassId = _class.Id,
                                            CreatedBy = user.FullName,
                                            CreatedOn = DateTime.Now,
                                            Enable = true,
                                            ModifiedOn = DateTime.Now,
                                            Type = 1,
                                            TypeName = "Chính thức",
                                            Note = "",
                                            StudentId = model.StudentId,
                                        });
                                        student.LearningStatus = 2;
                                        student.LearningStatusName = "Đang học";
                                    }
                                    else //Đăng ký đặt lớp chương trình học
                                    {
                                        if (item.ClassId.HasValue && item.ClassId != 0)
                                            throw new Exception("Không tìm thấy lớp học");//Bắt trường hợp FE truyền sai lớp học

                                        var program = await db.tbl_Program.SingleOrDefaultAsync(x => x.Id == item.ProgramId);
                                        if (program == null)
                                            throw new Exception("Không tìm thấy chương trình học");
                                        var detail = new tbl_BillDetail(item);
                                        detail.CreatedBy = detail.ModifiedBy = user.FullName;
                                        detail.BillId = model.Id;
                                        detail.Quantity = 1;
                                        detail.Price = program.Price;
                                        detail.TotalPrice = program.Price;
                                        detail.StudentId = model.StudentId;
                                        totalPrice += program.Price;
                                        db.tbl_BillDetail.Add(detail);
                                        db.tbl_ClassRegistration.Add(new tbl_ClassRegistration
                                        {
                                            BranchId = model.BranchId,
                                            StudentId = model.StudentId,
                                            ModifiedBy = user.FullName,
                                            Price = detail.TotalPrice,
                                            Status = 1,
                                            StatusName = "Chờ xếp lớp",
                                            ProgramId = program.Id,
                                            CreatedBy = user.FullName,
                                            CreatedOn = DateTime.Now,
                                            Enable = true,
                                            ModifiedOn = DateTime.Now,
                                        });
                                    }
                                }
                                else if (itemModel.Type == 2) //Mua dịch vụ
                                {

                                    var cart = await db.tbl_Cart.SingleOrDefaultAsync(x => x.Id == item.CartId && x.Enable == true);
                                    if (cart == null)
                                        throw new Exception("Không tìm thấy giỏ hàng");
                                    var product = await db.tbl_Product.SingleOrDefaultAsync(x => x.Id == cart.ProductId);
                                    if (product == null)
                                        throw new Exception("Không tìm thấy sản phẩm");
                                    var detail = new tbl_BillDetail(item);
                                    detail.CreatedBy = detail.ModifiedBy = user.FullName;
                                    detail.BillId = model.Id;
                                    detail.ProductId = product.Id;
                                    detail.Quantity = cart.Quantity;
                                    detail.Price = product.Price;
                                    detail.TotalPrice = product.Price * cart.Quantity;
                                    detail.StudentId = model.StudentId;
                                    totalPrice += detail.TotalPrice;
                                    db.tbl_BillDetail.Add(detail);
                                    cart.Enable = false;//Xóa giỏ
                                }
                                else if (itemModel.Type == 3) //Đăng ký lớp dạy kèm
                                {
                                    var program = await db.tbl_Program.SingleOrDefaultAsync(x => x.Id == item.ProgramId);
                                    if (program == null)
                                        throw new Exception("Không tìm thấy chương trình học");

                                    var curriculum = await db.tbl_Curriculum.SingleOrDefaultAsync(x => x.Id == item.CurriculumId);
                                    if (curriculum == null)
                                        throw new Exception("Không tìm thấy giáo trình");

                                    var _class = new tbl_Class
                                    {
                                        AcademicId = 0,
                                        BranchId = model.BranchId,
                                        CreatedBy = user.FullName,
                                        CreatedOn = DateTime.Now,
                                        CurriculumId = item.CurriculumId,
                                        Enable = true,
                                        GradeId = program.GradeId,
                                        Name = $"Dạy kèm {student.FullName} - {student.UserCode} [{program.Name} - {curriculum.Name}]",
                                        ModifiedBy = user.FullName,
                                        ModifiedOn = DateTime.Now,
                                        Price = program.Price,
                                        ProgramId = item.ProgramId,
                                        Status = 2,
                                        StatusName = "Đang diễn ra",
                                        TeacherId = 0,
                                        Type = 3,
                                        TypeName = "Dạy kèm"
                                    };
                                    db.tbl_Class.Add(_class);
                                    await db.SaveChangesAsync();

                                    //tạo giáo trình
                                    if (item.CurriculumId != null)
                                    {
                                        var curriculumInClass = new tbl_CurriculumInClass
                                        {
                                            ClassId = _class.Id,
                                            CurriculumId = item.CurriculumId,
                                            Name = curriculum.Name,
                                            IsComplete = false,
                                            CompletePercent = 0,
                                            CreatedBy = user.FullName,
                                            CreatedOn = DateTime.Now,
                                            Enable = true
                                        };
                                        db.tbl_CurriculumInClass.Add(curriculumInClass);
                                        await db.SaveChangesAsync();
                                        var curriculumDetails = await db.tbl_CurriculumDetail.Where(x => x.Enable == true && x.CurriculumId == curriculumInClass.CurriculumId).ToListAsync();
                                        if (curriculumDetails.Any())
                                        {
                                            foreach (var itemCurDetail in curriculumDetails)
                                            {
                                                var curDetailInClass = new tbl_CurriculumDetailInClass
                                                {
                                                    CurriculumIdInClass = curriculumInClass.Id,
                                                    CurriculumDetailId = itemCurDetail.Id,
                                                    IsComplete = false,
                                                    Name = itemCurDetail.Name,
                                                    Index = itemCurDetail.Index,
                                                    CompletePercent = 0,
                                                    Enable = true,
                                                    CreatedBy = user.FullName,
                                                    CreatedOn = DateTime.Now,
                                                    ModifiedBy = user.FullName,
                                                    ModifiedOn = DateTime.Now,
                                                };
                                                db.tbl_CurriculumDetailInClass.Add(curDetailInClass);
                                                db.SaveChanges();
                                                //Thêm cái file vào chương
                                                var file = await db.tbl_FileInCurriculumDetail.Where(x => x.Enable == true && x.CurriculumDetailId == itemCurDetail.Id).ToListAsync();
                                                if (file.Any())
                                                {
                                                    foreach (var itemFile in file)
                                                    {
                                                        var fileCreate = new tbl_FileCurriculumInClass
                                                        {
                                                            CurriculumDetailId = curDetailInClass.Id,
                                                            FileCurriculumId = itemFile.Id,
                                                            IsComplete = false,
                                                            IsHide = false,
                                                            FileName = itemFile.FileName,
                                                            FileUrl = itemFile.FileUrl,
                                                            Index = itemFile.Index,
                                                            ClassId = model.Id,
                                                            Enable = true,
                                                            CreatedBy = user.FullName,
                                                            CreatedOn = DateTime.Now,
                                                            ModifiedBy = user.FullName,
                                                            ModifiedOn = DateTime.Now
                                                        };
                                                        db.tbl_FileCurriculumInClass.Add(fileCreate);
                                                        await db.SaveChangesAsync();
                                                    }
                                                }
                                            }
                                        }
                                    }



                                    var studentInClass = new tbl_StudentInClass
                                    {
                                        BranchId = model.BranchId,
                                        ClassId = _class.Id,
                                        CreatedBy = user.FullName,
                                        CreatedOn = DateTime.Now,
                                        Enable = true,
                                        ModifiedBy = user.FullName,
                                        ModifiedOn = DateTime.Now,
                                        Note = "",
                                        Type = 1,
                                        TypeName = "Chính thức",
                                        Warning = false,
                                        StudentId = student.UserInformationId,
                                    };
                                    db.tbl_StudentInClass.Add(studentInClass);

                                    var detail = new tbl_BillDetail(item);
                                    detail.CreatedBy = detail.ModifiedBy = user.FullName;
                                    detail.BillId = model.Id;
                                    detail.ProductId = 0;
                                    detail.Quantity = 1;
                                    detail.Price = program.Price;
                                    detail.TotalPrice = _class.Price;
                                    detail.ProgramId = program.Id;
                                    detail.CurriculumId = item.CurriculumId;
                                    detail.StudentId = model.StudentId;
                                    totalPrice += _class.Price;
                                    db.tbl_BillDetail.Add(detail);

                                    await db.SaveChangesAsync();

                                }
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            totalPrice = itemModel.Price;
                        }
                        if (itemModel.DiscountId.HasValue && totalPrice > 0 && itemModel.DiscountId > 0 && itemModel.Type != 4)
                        {
                            var discount = await db.tbl_Discount.SingleOrDefaultAsync(x => x.Id == itemModel.DiscountId);
                            if (discount == null)
                                throw new Exception("Không tìm thấy khuyến mãi");
                            if (discount.PackageType == 1 && itemModel.Details.Count() > 1)
                                throw new Exception("Khuyến mãi dành cho mua lẻ");
                            if (discount.PackageType == 2 && itemModel.Details.Count() == 1)
                                throw new Exception("Khuyến mãi dành cho gói combo");
                            if (discount.Status == 2)
                                throw new Exception("Khuyến mãi đã hết hạn");
                            if (discount.Quantity <= discount.UsedQuantity)
                                throw new Exception("Khuyến mãi đã dùng hết");

                            //Tính khuyến mãi
                            if (discount.Type == 1)
                                reduced = discount.Value;
                            else
                            {
                                reduced = (totalPrice / 100) * discount.Value;
                                if (reduced > discount.MaxDiscount)
                                    reduced = discount.MaxDiscount ?? 0;
                            }
                            discount.UsedQuantity += 1;
                        }
                        model.TotalPrice = totalPrice;
                        model.Reduced = reduced;
                        model.Debt = (totalPrice - (reduced + model.Paid));
                        if (model.Paid > 0)
                        {
                            string printContent = await PaymentSessionService.GetPrintContent(
                                    1,
                                    model.StudentId,
                                    $"Thanh toán {(model.Type == 1 ? "đăng ký học" : model.Type == 2 ? "mua dịch vụ" : "")}",
                                    model.Paid,
                                    user.FullName,
                                    student.FullName,
                                    student.UserCode
                                    );
                            db.tbl_PaymentSession.Add(new tbl_PaymentSession
                            {
                                BranchId = model.BranchId,
                                CreatedBy = user.FullName,
                                CreatedOn = DateTime.Now,
                                Enable = true,
                                ModifiedBy = user.FullName,
                                ModifiedOn = DateTime.Now,
                                Type = 1,
                                TypeName = "Thu",
                                PaymentMethodId = model.PaymentMethodId,
                                Reason = $"Thanh toán {(model.Type == 1 ? "đăng ký học" : model.Type == 2 ? "mua dịch vụ" : "")}",
                                UserId = model.StudentId,
                                Note = model.Note,
                                Value = model.Paid,
                                PrintContent = printContent
                            });
                        }
                        if (model.Debt <= 0)
                        {
                            model.CompleteDate = DateTime.Now;
                            var details = await db.tbl_BillDetail
                                .Where(x => x.BillId == model.Id && x.CartId.HasValue && x.CartId != 0 && x.Enable == true)
                                .ToListAsync();
                            if (details.Any())
                            {
                                foreach (var item in details)
                                {
                                    var product = await db.tbl_Product.SingleOrDefaultAsync(x => x.Id == item.ProductId);
                                    if (product.Type == 1)//Khóa video thì tạo mã active
                                    {
                                        List<string> activeCodes = await db.tbl_VideoActiveCode.Select(i => i.ActiveCode).ToListAsync();
                                        for (int i = 1; i <= item.Quantity; i++)
                                        {
                                            string activeCode = AssetCRM.RandomStringWithText(10);
                                            while (activeCodes.Any(ac => ac.Contains(activeCode)))
                                            {
                                                activeCode = AssetCRM.RandomStringWithText(10);
                                            }
                                            var videoActiveCode = new tbl_VideoActiveCode
                                            {
                                                ActiveCode = activeCode,
                                                StudentId = item.StudentId,
                                                ProductId = product.Id,
                                                BillDetailId = item.Id,
                                                CreatedBy = user.FullName,
                                                CreatedOn = DateTime.Now,
                                                Enable = true,
                                                IsUsed = false,
                                                ModifiedBy = user.FullName,
                                                ModifiedOn = DateTime.Now
                                            };
                                            db.tbl_VideoActiveCode.Add(videoActiveCode);
                                            await db.SaveChangesAsync();
                                        }
                                    }
                                    else if (product.Type == 2)//Thêm bộ đề cho học viên
                                    {
                                        var packageStudent = new tbl_PackageStudent
                                        {
                                            CreatedBy = user.FullName,
                                            CreatedOn = DateTime.Now,
                                            Enable = true,
                                            ModifiedBy = user.FullName,
                                            ModifiedOn = DateTime.Now,
                                            PackageId = product.Id,
                                            StudentId = item.StudentId
                                        };
                                        db.tbl_PackageStudent.Add(packageStudent);
                                        product.TotalStudent += 1;
                                        await db.SaveChangesAsync();
                                    }
                                    else if (product.Type == 3)//Thêm lượt chấm cho học viên
                                    {
                                        var markQuantity = await db.tbl_MarkQuantity.FirstOrDefaultAsync(x => x.StudentId == item.StudentId && x.Enable == true);
                                        if (markQuantity == null)
                                        {
                                            markQuantity = new tbl_MarkQuantity
                                            {
                                                CreatedBy = user.FullName,
                                                CreatedOn = DateTime.Now,
                                                Enable = true,
                                                ModifiedBy = user.FullName,
                                                ModifiedOn = DateTime.Now,
                                                Quantity = product.MarkQuantity,
                                                StudentId = item.StudentId,
                                                UsedQuantity = 0,
                                            };
                                            db.tbl_MarkQuantity.Add(markQuantity);
                                        }
                                        else
                                        {
                                            markQuantity.Quantity += product.MarkQuantity;
                                            markQuantity.ModifiedBy = user.FullName;
                                            markQuantity.ModifiedOn = DateTime.Now;
                                        }
                                        await db.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                        //thông báo tạo hóa đơn cho học viên                
                        var sendStudent = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == model.StudentId);
                        tbl_Notification notification = new tbl_Notification()
                        {
                            Title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " thông báo hóa đơn",
                            Content = "Bạn có hóa đơn mới. Giá trị của hóa đơn là " + String.Format("{0:0,0}", model.TotalPrice) + ". Vui lòng kiểm tra.",
                            UserId = sendStudent.UserInformationId,
                            IsSeen = false,
                            CreatedBy = user.FullName,
                            CreatedOn = DateTime.Now,
                            Enable = true
                        };
                        db.tbl_Notification.Add(notification);
                        await db.SaveChangesAsync();
                        Thread threadStudent = new Thread(() =>
                        {
                            AssetCRM.OneSignalPushNotifications(notification.Title, notification.Content, sendStudent.OneSignal_DeviceId);
                            AssetCRM.SendMail(sendStudent.Email, notification.Title, notification.Content);
                        });
                        threadStudent.Start();
                        //thông báo cho phụ huynh nếu có
                        if (sendStudent.ParentId.HasValue)
                        {
                            tbl_UserInformation parent = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == sendStudent.ParentId && x.Enable == true);
                            if (parent != null)
                            {
                                notification.Title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " thông báo hóa đơn";
                                notification.Content = "Học viên " + sendStudent.FullName + " có hóa đơn mới. Giá trị của hóa đơn là " + String.Format("{0:0,0}", model.TotalPrice) + ". Vui lòng kiểm tra.";
                                notification.UserId = parent.UserInformationId;
                                db.tbl_Notification.Add(notification);
                                await db.SaveChangesAsync();
                                Thread threadParent = new Thread(() =>
                                {
                                    AssetCRM.OneSignalPushNotifications(notification.Title, notification.Content, parent.OneSignal_DeviceId);
                                    AssetCRM.SendMail(parent.Email, notification.Title, notification.Content);
                                });
                                threadParent.Start();
                            }
                        }
                        //thông báo cho admin
                        var admins = await db.tbl_UserInformation.Where(x => x.RoleId == (int)RoleEnum.admin && x.Enable == true && x.UserInformationId != user.UserInformationId).ToListAsync();
                        foreach (var ad in admins)
                        {
                            notification.Title = "Hóa đơn mới được tạo";
                            notification.Content = user.FullName + " đã tạo hóa đơn mới. Giá trị của hóa đơn là " + String.Format("{0:0,0}", model.TotalPrice) + ". Vui lòng kiểm tra";
                            notification.UserId = ad.UserInformationId;
                            db.tbl_Notification.Add(notification);
                            await db.SaveChangesAsync();
                            Thread threadAd = new Thread(() =>
                            {
                                AssetCRM.OneSignalPushNotifications(notification.Title, notification.Content, ad.OneSignal_DeviceId);
                                AssetCRM.SendMail(ad.Email, notification.Title, notification.Content);
                            });
                            threadAd.Start();
                        }
                        await db.SaveChangesAsync();
                        tran.Commit();
                        return model;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }
        public class PaymentCreate
        {
            public int Id { get; set; }
            public double Paid { get; set; }
            public string Note { get; set; }
            /// <summary>
            /// Ngày hẹn thanh toán
            /// </summary>
            public DateTime? PaymentAppointmentDate { get; set; }
        }
        /// <summary>
        /// Thanh toán
        /// </summary>
        /// <returns></returns>
        public static async Task<tbl_Bill> Payment(PaymentCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await db.tbl_Bill.SingleOrDefaultAsync(x => x.Id == itemModel.Id);
                        if (entity == null)
                            throw new Exception("Không tìm thấy hóa đơn");
                        if (entity.Debt == 0)
                            throw new Exception("Đã thanh toán hết");
                        if (itemModel.Paid == 0)
                            throw new Exception("Số tiền thanh toán không phù hợp");

                        //Kiểm tra quyền thanh toán, nếu không có quyền thanh toán sẽ gửi yêu cầu duyệt
                        var paymentAllow = await db.tbl_PaymentAllow.AnyAsync(x => x.UserId == user.UserInformationId && x.Enable == true);
                        if (!paymentAllow && user.RoleId != ((int)RoleEnum.admin) && user.RoleId != ((int)RoleEnum.accountant))
                        {
                            double money = itemModel.Paid;
                            itemModel.Paid = 0;
                            var paymentApprove = new tbl_PaymentApprove
                            {
                                BillId = entity.Id,
                                CreatedBy = user.FullName,
                                CreatedOn = DateTime.Now,
                                Enable = true,
                                ModifiedBy = user.FullName,
                                ModifiedOn = DateTime.Now,
                                Money = money,
                                Note = itemModel.Note,
                                Status = 1,
                                StatusName = "Chờ duyệt",
                                UserId = user.UserInformationId
                            };
                            db.tbl_PaymentApprove.Add(paymentApprove);
                            await db.SaveChangesAsync();
                            var branch = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == entity.BranchId);

                            string content = $"Bạn đã yêu cầu duyệt thanh toán tại trung tâm {branch?.Name} vào thời gian {(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))}, vui lòng đợi duyệt.";
                            Thread send = new Thread(async () =>
                            {
                                await NotificationService.Send(new tbl_Notification
                                {
                                    Content = content,
                                    Title = "Yêu cầu duyệt thanh toán",
                                    UserId = user.UserInformationId
                                }, user);
                                await NotificationService.Send(new tbl_Notification
                                {
                                    Content = content,
                                    Title = "Yêu cầu duyệt thanh toán",
                                    UserId = entity.StudentId
                                }, user);
                            });
                        }
                        else
                        {

                            entity.Paid += itemModel.Paid;
                            entity.Debt = (entity.TotalPrice - (entity.Reduced + entity.Paid));
                            entity.PaymentAppointmentDate = itemModel.PaymentAppointmentDate ?? entity.PaymentAppointmentDate;
                            entity.ModifiedOn = DateTime.Now;
                            entity.ModifiedBy = user.FullName;
                            if (entity.Debt <= 0)
                            {
                                entity.CompleteDate = DateTime.Now;
                                var details = await db.tbl_BillDetail
                                            .Where(x => x.BillId == entity.Id && x.CartId.HasValue && x.CartId != 0 && x.Enable == true)
                                            .ToListAsync();
                                foreach (var item in details)
                                {
                                    var product = await db.tbl_Product.SingleOrDefaultAsync(x => x.Id == item.ProductId);
                                    if (product.Type == 1)//Khóa video thì tạo mã active
                                    {
                                        List<string> activeCodes = await db.tbl_VideoActiveCode.Select(i => i.ActiveCode).ToListAsync();
                                        for (int i = 1; i <= item.Quantity; i++)
                                        {
                                            string activeCode = AssetCRM.RandomStringWithText(10);
                                            while (activeCodes.Any(ac => ac.Contains(activeCode)))
                                            {
                                                activeCode = AssetCRM.RandomStringWithText(10);
                                            }
                                            var videoActiveCode = new tbl_VideoActiveCode
                                            {
                                                ActiveCode = activeCode,
                                                StudentId = item.StudentId,
                                                ProductId = product.Id,
                                                BillDetailId = item.Id,
                                                CreatedBy = user.FullName,
                                                CreatedOn = DateTime.Now,
                                                Enable = true,
                                                IsUsed = false,
                                                ModifiedBy = user.FullName,
                                                ModifiedOn = DateTime.Now
                                            };
                                            db.tbl_VideoActiveCode.Add(videoActiveCode);
                                            await db.SaveChangesAsync();
                                        }
                                    }
                                    else if (product.Type == 2)//Thêm bộ đề cho học viên
                                    {
                                        var packageStudent = new tbl_PackageStudent
                                        {
                                            CreatedBy = user.FullName,
                                            CreatedOn = DateTime.Now,
                                            Enable = true,
                                            ModifiedBy = user.FullName,
                                            ModifiedOn = DateTime.Now,
                                            PackageId = product.Id,
                                            StudentId = item.StudentId
                                        };
                                        db.tbl_PackageStudent.Add(packageStudent);
                                        product.TotalStudent += 1;
                                        await db.SaveChangesAsync();
                                    }
                                    else if (product.Type == 3)//Thêm lượt chấm cho học viên
                                    {
                                        var markQuantity = await db.tbl_MarkQuantity.FirstOrDefaultAsync(x => x.StudentId == item.StudentId && x.Enable == true);
                                        if (markQuantity == null)
                                        {
                                            markQuantity = new tbl_MarkQuantity
                                            {
                                                CreatedBy = user.FullName,
                                                CreatedOn = DateTime.Now,
                                                Enable = true,
                                                ModifiedBy = user.FullName,
                                                ModifiedOn = DateTime.Now,
                                                Quantity = product.MarkQuantity,
                                                StudentId = item.StudentId,
                                                UsedQuantity = 0,
                                            };
                                            db.tbl_MarkQuantity.Add(markQuantity);
                                        }
                                        else
                                        {
                                            markQuantity.Quantity += product.MarkQuantity;
                                            markQuantity.ModifiedBy = user.FullName;
                                            markQuantity.ModifiedOn = DateTime.Now;
                                        }
                                        await db.SaveChangesAsync();
                                    }
                                }
                            }
                            db.tbl_PaymentSession.Add(new tbl_PaymentSession
                            {
                                BranchId = entity.BranchId,
                                CreatedBy = user.FullName,
                                CreatedOn = DateTime.Now,
                                Enable = true,
                                ModifiedBy = user.FullName,
                                ModifiedOn = DateTime.Now,
                                PaymentMethodId = entity.PaymentMethodId,
                                Reason = $"Thanh toán {(entity.Type == 1 ? "đăng ký học" : entity.Type == 2 ? "mua dịch vụ" : "")}",
                                UserId = entity.StudentId,
                                Note = itemModel.Note,
                                Type = 1,
                                TypeName = "Thu",
                                Value = itemModel.Paid,
                                PrintContent = Task.Run(() => PaymentSessionService.GetPrintContent(
                                    1,
                                    entity.StudentId,
                                    $"Thanh toán {(entity.Type == 1 ? "đăng ký học" : entity.Type == 2 ? "mua dịch vụ" : "")}",
                                    entity.Paid,
                                    user.FullName
                                    )).Result,
                            });
                            await db.SaveChangesAsync();
                        }
                        tran.Commit();
                        return entity;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }

        public class BillResult : AppDomainResult
        {
            public double SumtotalPrice { get; set; }
            public double SumPaid { get; set; }
            public double SumDebt { get; set; }
        }
        public static async Task<BillResult> GetAll(BillSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new BillSearch();
                string myBranchIds = "";
                if (user.RoleId != ((int)RoleEnum.admin))
                    myBranchIds = user.BranchIds;
                if (user.RoleId == ((int)RoleEnum.student))
                    baseSearch.StudentIds = user.UserInformationId.ToString();
                string sql = $"Get_Bill @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@BranchIds = N'{baseSearch.BranchIds ?? ""}'," +
                    $"@StudentIds = N'{baseSearch.StudentIds ?? ""}'," +
                    $"@ParentIds = N'{baseSearch.ParentIds ?? ""}'," +
                    $"@MyBranchIds = N'{myBranchIds ?? ""}'," +
                    $"@FromDate = N'{baseSearch.FromDate ?? ""}'," +
                    $"@ToDate = N'{baseSearch.ToDate ?? ""}'";
                var data = await db.Database.SqlQuery<Get_Bill>(sql).ToListAsync();
                if (!data.Any()) return new BillResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_Bill(i)).ToList();
                double sumtotalPrice = data[0].SumTotalPrice;
                double sumPaid = data[0].SumPaid;
                double sumDebt = data[0].SumDebt;

                return new BillResult
                {
                    TotalRow = totalRow,
                    Data = result,
                    Success = true,
                    SumDebt = sumDebt,
                    SumPaid = sumPaid,
                    SumtotalPrice = sumtotalPrice
                };
            }
        }

        public static async Task<AppDomainResult> GetDiscountHistory(BillSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new BillSearch();
                string myBranchIds = "";
                if (user.RoleId != ((int)RoleEnum.admin))
                    myBranchIds = user.BranchIds;
                if (user.RoleId == ((int)RoleEnum.student))
                    baseSearch.StudentIds = user.UserInformationId.ToString();
                string sql = $"Get_DiscountHistory @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@BranchIds = N'{baseSearch.BranchIds ?? ""}'," +
                    $"@StudentIds = N'{baseSearch.StudentIds ?? ""}'," +
                    $"@MyBranchIds = N'{myBranchIds ?? ""}'";
                var data = await db.Database.SqlQuery<Get_Bill>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                return new AppDomainResult { TotalRow = totalRow, Data = data };
            }
        }

        public static async Task<List<Get_BillDetail>> GetDetail(int billId)
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_BillDetail @BillId= {billId}";
                var data = await db.Database.SqlQuery<Get_BillDetail>(sql).ToListAsync();
                return data;
            }
        }

        public static async Task PaymentNotification()
        {
            using (var db = new lmsDbContext())
            {
                try
                {

                    DateTime now = DateTime.Now;
                    DateTime firstDay = new DateTime(now.Year, now.Month, now.Day, 00, 00, 00);
                    DateTime lastDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

                    var data = await db.tbl_Bill.Where(x => x.Enable == true && x.Debt > 0 && x.PaymentAppointmentDate != null && x.PaymentAppointmentDate >= firstDay && x.PaymentAppointmentDate <= lastDay).ToListAsync();
                    foreach (var item in data)
                    {
                        var user = await db.tbl_UserInformation.FirstOrDefaultAsync(x => x.UserInformationId == item.StudentId);
                        if (user != null)
                        {

                            tbl_Notification notification = new tbl_Notification()
                            {
                                Title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " thông báo hóa đơn",
                                Content = "Bạn có hóa đơn " + item.Code + " đã đến hạn thanh toán " + item.PaymentAppointmentDate.Value.ToString("dd/MM/yyyy") + " với số tiền là " + String.Format("{0:0,0}", item.Debt) + ". Xin vui lòng thanh toán.",
                                UserId = user.UserInformationId,
                                IsSeen = false,
                                CreatedBy = user.FullName,
                                CreatedOn = DateTime.Now,
                                Enable = true
                            };
                            db.tbl_Notification.Add(notification);
                            await db.SaveChangesAsync();
                            Thread threadStudent = new Thread(() =>
                            {
                                AssetCRM.OneSignalPushNotifications(notification.Title, notification.Content, user.OneSignal_DeviceId);
                                AssetCRM.SendMail(user.Email, notification.Title, notification.Content);
                            });
                            threadStudent.Start();
                        }
                        if (user.ParentId.HasValue)
                        {
                            var parent = await db.tbl_UserInformation.FirstOrDefaultAsync(x => x.UserInformationId == user.ParentId);
                            if (parent != null)
                            {
                                tbl_Notification notification = new tbl_Notification()
                                {
                                    Title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " thông báo hóa đơn",
                                    Content = "Học viên " + user.FullName + " có hóa đơn " + item.Code + " đã đến hạn thanh toán " + item.PaymentAppointmentDate.Value.ToString("dd/MM/yyyy") + " với số tiền là " + String.Format("{0:0,0}", item.Debt) + ". Xin vui lòng thanh toán.",
                                    UserId = parent.UserInformationId,
                                    IsSeen = false,
                                    CreatedBy = user.FullName,
                                    CreatedOn = DateTime.Now,
                                    Enable = true
                                };
                                db.tbl_Notification.Add(notification);
                                await db.SaveChangesAsync();
                                Thread threadParent = new Thread(() =>
                                {
                                    AssetCRM.OneSignalPushNotifications(notification.Title, notification.Content, parent.OneSignal_DeviceId);
                                    AssetCRM.SendMail(parent.Email, notification.Title, notification.Content);
                                });
                                threadParent.Start();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    AssetCRM.Writelog("Bill", "NotificationPayment", 1, e.Message + e.InnerException);
                }
            }
        }
    }
}