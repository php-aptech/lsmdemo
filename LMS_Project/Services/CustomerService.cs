using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class CustomerService
    {
        public static async Task<tbl_Customer> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Customer.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task Complete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var status = await db.tbl_CustomerStatus.FirstOrDefaultAsync(x => x.Type == 3);
                var data = await db.tbl_Customer.SingleOrDefaultAsync(x => x.Id == id);
                if (data != null && status != null)
                    data.CustomerStatusId = status.Id;
                await db.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Chia đều tư vấn viên
        /// </summary>
        /// <returns></returns>
        public static async Task<int> GetSaleRadom()
        {
            using (var db = new lmsDbContext())
            {
                var sales = await db.tbl_UserInformation
                    .Where(x => x.RoleId == ((int)RoleEnum.sale) && x.StatusId == ((int)AccountStatus.active) && x.Enable == true)
                    .OrderBy(x => x.UserInformationId)
                    .ToListAsync();
                if (!sales.Any())
                    return 0;
                var customer = await db.tbl_Customer.Where(x => x.Enable == true).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                if (customer == null)
                    return sales[0].UserInformationId;
                for (int i = 0; i < sales.Count() - 1; i++)
                {
                    if (customer.SaleId == sales[i].UserInformationId)
                        return sales[i + 1].UserInformationId;
                }
                return sales[0].UserInformationId;
            }
        }
        public static async Task<tbl_Customer> Insert(CustomerCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var model = new tbl_Customer(itemModel);
                var branch = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == model.BranchId);
                if (branch == null)
                    throw new Exception("Không tìm thấy trung tâm");
                var status = await db.tbl_CustomerStatus.FirstOrDefaultAsync(x => x.Type == 1);
                if (status == null)
                    throw new Exception("Lỗi, vui lòng liên hệ quản trị viên");//Thêm các trạng thái mặc định Migrations/Configuration/Seed
                if (user.RoleId == ((int)RoleEnum.sale))
                    model.SaleId = user.UserInformationId;
                int saleId = await GetSaleRadom();
                model.CustomerStatusId = status.Id;
                model.SaleId = model.SaleId ?? saleId;
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_Customer.Add(model);
                await db.SaveChangesAsync();
                //thông báo cho tư vấn viên
                if (itemModel.SaleId.HasValue)
                {
                    Thread sendNoti = new Thread(async () =>
                    {
                        await NotificationService.Send(new tbl_Notification
                        {
                            Title = "Tư vấn cho khách hàng mới.",
                            Content = "Bạn được chỉ định tư vấn cho khách hàng " + itemModel.FullName,
                            UserId = itemModel.SaleId
                        }, user);
                    });
                    sendNoti.Start();
                }
                return model;
            }
        }
        public class CheckExistModel
        {
            [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
            public string Mobile { get; set; }
            [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
            public string Email { get; set; }
        }
        /// <summary>
        /// Kiểm tra thông tin khách hàng này đã tồn tại hay chưa
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<bool> CheckExist(CheckExistModel itemModel)
        {
            using (var db = new lmsDbContext())
            {
                string email = "";
                string mobile = "";
                if (!string.IsNullOrEmpty(itemModel.Email))
                    email = itemModel.Email;
                if (!string.IsNullOrEmpty(itemModel.Mobile))
                    mobile = itemModel.Mobile;
                var check = await db.tbl_Customer
                    .AnyAsync(x => (x.Email.ToUpper() == itemModel.Email.ToUpper() || x.Mobile.ToUpper() == itemModel.Mobile.ToUpper()) && x.Enable == true);
                if (check)
                    return true;
                return false;
            }
        }
        public static async Task<tbl_Customer> Update(CustomerUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Customer.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                if (itemModel.BranchId.HasValue)
                {
                    var branch = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == itemModel.BranchId);
                    if (branch == null)
                        throw new Exception("Không tìm thấy trung tâm");
                }
                //thông báo cho tư vấn viên mới nếu đổi tư vấn viên
                if (itemModel.SaleId.HasValue && itemModel.SaleId != entity.SaleId)
                {
                    Thread sendNoti = new Thread(async () =>
                    {
                        await NotificationService.Send(new tbl_Notification
                        {
                            Title = "Tư vấn cho khách hàng mới.",
                            Content = "Bạn được chỉ định tư vấn cho khách hàng " + itemModel.FullName,
                            UserId = itemModel.SaleId
                        }, user);
                    });
                    sendNoti.Start();
                }
                //thông báo cho tư vấn viên nếu người khác đổi trạng thái khách hàng của tư vấn viên này
                if (itemModel.SaleId.HasValue && itemModel.SaleId != user.UserInformationId && itemModel.CustomerStatusId != entity.CustomerStatusId)
                {
                    Thread sendNoti = new Thread(async () =>
                    {
                        await NotificationService.Send(new tbl_Notification
                        {
                            Title = "Trạng thái khách hàng thay đổi",
                            Content = "Khách hàng " + itemModel.FullName + " đã được " + user.FullName + " thay đổi trạng thái. Vui lòng kiểm tra",
                            UserId = itemModel.SaleId
                        }, user);
                    });
                    sendNoti.Start();
                }
                entity.LearningNeedId = itemModel.LearningNeedId ?? entity.LearningNeedId;
                entity.CustomerStatusId = itemModel.CustomerStatusId ?? entity.CustomerStatusId;
                entity.FullName = itemModel.FullName ?? entity.FullName;
                entity.Mobile = itemModel.Mobile ?? entity.Mobile;
                entity.Email = itemModel.Email ?? entity.Email;
                entity.SourceId = itemModel.SourceId ?? entity.SourceId;
                entity.SaleId = itemModel.SaleId ?? entity.SaleId;
                entity.PurposeId = itemModel.PurposeId ?? entity.PurposeId;
                entity.AreaId = itemModel.AreaId ?? entity.AreaId;
                entity.DistrictId = itemModel.DistrictId ?? entity.DistrictId;
                entity.WardId = itemModel.WardId ?? entity.WardId;
                entity.Address = itemModel.Address ?? entity.Address;
                entity.BranchId = itemModel.BranchId ?? entity.BranchId;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                entity.JobId = entity.JobId ?? entity.JobId;
                await db.SaveChangesAsync();

                return entity;
            }
        }
        public class SendMailModel
        {
            /// <summary>
            /// mẫu 1,2
            /// </summary>
            public string Ids { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
        }
        public static async Task SendMail(SendMailModel model)
        {
            using (var db = new lmsDbContext())
            {
                var emails = await db.tbl_Customer
                    .Where(x => model.Ids.Contains(x.Id.ToString()) && x.Enable == true)
                    .Select(x => x.Email)
                    .ToListAsync();
                if (emails.Any())
                {
                    Thread send = new Thread(() =>
                    {
                        foreach (var item in emails)
                        {
                            AssetCRM.SendMail(item, model.Title, model.Content);
                        }
                    });
                    send.Start();
                }

            }
        }

        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Customer.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public static async Task<AppDomainResult> GetAll(CustomerSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new CustomerSearch();
                string myBranchIds = "";
                if (user.RoleId != ((int)RoleEnum.admin))
                    myBranchIds = user.BranchIds;
                int mySaleId = 0;
                if (user.RoleId == ((int)RoleEnum.sale))
                    mySaleId = user.UserInformationId;
                string sql = $"Get_Customer @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@FullName = N'{baseSearch.FullName ?? ""}'," +
                    $"@Code = N'{baseSearch.Code ?? ""}'," +
                    $"@CustomerStatusIds = N'{baseSearch.CustomerStatusIds ?? ""}'," +
                    $"@BranchIds = N'{baseSearch.BranchIds ?? ""}'," +
                    $"@MyBranchIds = N'{myBranchIds ?? ""}'," +
                    $"@MySaleId = N'{mySaleId}'," +
                    $"@Sort = N'{baseSearch.Sort}'," +
                    $"@SortType = N'{(baseSearch.SortType ? 1 : 0)}'";
                var data = await db.Database.SqlQuery<Get_Customer>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_Customer(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public static async Task ImportData(List<CustomerCreate> itemModels, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (!itemModels.Any())
                            throw new Exception("Không tìm thấy dữ liệu");
                        foreach (var item in itemModels)
                        {
                            var model = new tbl_Customer(item);
                            var status = await db.tbl_CustomerStatus.FirstOrDefaultAsync(x => x.Type == 1);
                            if (status == null)
                                throw new Exception("Lỗi, vui lòng liên hệ quản trị viên");//Thêm các trạng thái mặc định Migrations/Configuration/Seed
                            
                            model.CustomerStatusId = status.Id;
                            if (!string.IsNullOrEmpty(user.BranchIds))
                            {
                                model.BranchId = int.Parse(user.BranchIds.Substring(0, 1));
                            }

                            model.CreatedBy = model.ModifiedBy = user.FullName;
                            db.tbl_Customer.Add(model);
                            await db.SaveChangesAsync();
                        }
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }
    }
}