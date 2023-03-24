using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
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
    public class UserInformation
    {
        public static string ConvertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        public static async Task<tbl_UserInformation> GetById(int userinfomationId)
        {
            tbl_UserInformation account = new tbl_UserInformation();
            using (lmsDbContext _db = new lmsDbContext())
            {
                account = await _db.tbl_UserInformation.Where(c => c.UserInformationId == userinfomationId).FirstOrDefaultAsync();
            }
            return account;
        }
        public static tbl_UserInformation GetByUserName(string username)
        {
            tbl_UserInformation account = new tbl_UserInformation();
            using (lmsDbContext _db = new lmsDbContext())
            {
                account = _db.tbl_UserInformation.Where(c => c.UserName.ToUpper() == username.ToUpper()).FirstOrDefault();
            }
            return account;
        }
        public static async Task<tbl_UserInformation> Insert(tbl_UserInformation user, tbl_UserInformation userLogin, string programIds = "")
        {
            using (var db = new lmsDbContext())
            {
                //nếu không phải học viên hoặc phụ huynh thì thông báo cho admin
                if (user.RoleId != (int)RoleEnum.student || user.RoleId != (int)RoleEnum.parents)
                {
                    Thread sendTeacher = new Thread(async () =>
                    {
                        List<tbl_UserInformation> admins = await db.tbl_UserInformation.Where(x => x.Enable == false && x.RoleId == (int)RoleEnum.admin).ToListAsync();

                        tbl_Notification notification = new tbl_Notification();
                        notification.Title = "Tài khoản mới được tạo";
                        notification.Content = userLogin.FullName + " đã tạo tài khoản mới. Vui lòng kiểm tra";
                        foreach (var ad in admins)
                        {
                            notification.UserId = ad.UserInformationId;
                            await NotificationService.Send(notification, user, "", false);
                        }
                    });
                    sendTeacher.Start();
                }
                await ValidateUser(user.UserName);

                if (userLogin.RoleId == ((int)RoleEnum.sale))
                    user.SaleId = userLogin.UserInformationId;
                else
                    user.SaleId = await CustomerService.GetSaleRadom();
                user.CreatedBy = user.ModifiedBy = userLogin.FullName;
                user.Password = Encryptor.Encrypt(user.Password);
                user.ActiveDate = DateTime.Now;
                db.tbl_UserInformation.Add(user);
                await db.SaveChangesAsync();

                //Thêm giáo viên vào chương trình khi tạo
                if (user.RoleId == ((int)RoleEnum.teacher) && !string.IsNullOrEmpty(programIds))
                {
                    var listProgram = programIds.Split(',').ToList();
                    foreach (var item in listProgram)
                    {
                        int teacherId = int.Parse(item);
                        db.tbl_TeacherInProgram.Add(new tbl_TeacherInProgram
                        {
                            CreatedBy = userLogin.FullName,
                            CreatedOn = DateTime.Now,
                            Enable = true,
                            ModifiedBy = userLogin.FullName,
                            ModifiedOn = DateTime.Now,
                            ProgramId = teacherId,
                            TeacherId = user.UserInformationId
                        });
                        await db.SaveChangesAsync();
                    }
                }
                return user;
            }
        }
        public static async Task ValidateUser(string userName)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var checkUser = await db.tbl_UserInformation
                        .Where(x => x.UserName.ToUpper() == userName.ToUpper() && x.Enable == true).AnyAsync();
                    if (checkUser)
                        throw new Exception($"Tên đăng nhập {userName} đã tồn tại");

                    if (!UserNameFormat(userName))
                        throw new Exception($"Tài khoản đăng nhập {userName} không hợp lệ");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static bool UserNameFormat(string value)
        {
            string[] arr = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ"," ",};
            foreach (var item in arr)
            {
                if (value.Contains(item))
                    return false;
            }
            return true;
        }
        public static async Task<tbl_UserInformation> Update(tbl_UserInformation user, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                try
                {

                    if (userLogin.RoleId != ((int)RoleEnum.admin) & (user.RoleId == ((int)RoleEnum.admin) || user.RoleId == ((int)RoleEnum.manager)))
                        throw new Exception("Bạn không thể cập nhật thông tin Quản trị viên và Quản lý");
                    var entity = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == user.UserInformationId);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    if (!string.IsNullOrEmpty(user.UserName) && user.UserName.ToUpper() != entity.UserName.ToUpper())
                    {
                        var validate = await db.tbl_UserInformation.Where(x => x.UserName.ToUpper() == user.UserName.ToUpper() && x.Enable == true).AnyAsync();
                        if (validate)
                            throw new Exception("Tên đăng nhập đã tồn tại");
                    }
                    entity.FullName = user.FullName ?? entity.FullName;
                    entity.UserName = user.UserName ?? entity.UserName;
                    entity.Email = user.Email ?? entity.Email;
                    entity.DOB = user.DOB ?? entity.DOB;
                    entity.Gender = user.Gender ?? entity.Gender;
                    entity.BranchIds = user.BranchIds ?? entity.BranchIds;
                    entity.Mobile = user.Mobile ?? entity.Mobile;
                    entity.Address = user.Address ?? entity.Address;
                    if (entity.StatusId == ((int)AccountStatus.inActive) && user.StatusId == ((int)AccountStatus.active))
                        entity.ActiveDate = DateTime.Now;
                    entity.StatusId = user.StatusId ?? entity.StatusId;
                    entity.Avatar = user.Avatar ?? entity.Avatar;
                    entity.AreaId = user.AreaId ?? entity.AreaId;
                    entity.DistrictId = user.DistrictId ?? entity.DistrictId;
                    entity.WardId = user.WardId ?? entity.WardId;
                    entity.Password = user.Password == null ? entity.Password : Encryptor.Encrypt(user.Password);
                    entity.SourceId = user.SourceId ?? entity.SourceId;
                    entity.LearningNeedId = user.LearningNeedId ?? entity.LearningNeedId;
                    entity.SaleId = user.SaleId == 0 ? entity.SaleId : user.SaleId;
                    entity.PurposeId = user.PurposeId ?? entity.PurposeId;
                    entity.Extension = user.Extension ?? entity.Extension;
                    entity.ParentId = user.ParentId;
                    entity.ModifiedOn = user.ModifiedOn;
                    entity.ModifiedBy = userLogin.FullName;
                    entity.JobId = user.JobId ?? entity.JobId;
                    await db.SaveChangesAsync();
                    return entity;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task Delete(int userInformationId)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == userInformationId);
                    if (user == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    user.Enable = false;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public class RoleModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public static async Task<AppDomainResult> GetAll(UserSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) return new AppDomainResult { TotalRow = 0, Data = null };

                string myBranchIds = "";
                if (user.RoleId != ((int)RoleEnum.admin))
                    myBranchIds = user.BranchIds;
                int saleId = 0;
                if (user.RoleId == ((int)RoleEnum.sale))
                    saleId = user.UserInformationId;
                string sql = $"Get_User @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@FullName = N'{baseSearch.FullName ?? ""}'," +
                    $"@UserCode = N'{baseSearch.UserCode ?? ""}'," +
                    $"@RoleIds = N'{baseSearch.RoleIds ?? ""}'," +
                    $"@BranchIds = N'{baseSearch.BranchIds ?? ""}'," +
                    $"@MyBranchIds= N'{myBranchIds}'," +
                    $"@SaleId = {saleId}," +
                    $"@Genders = N'{baseSearch.Genders ?? ""}'," +
                    $"@Sort = {baseSearch.Sort}," +
                    $"@ParentIds = '{baseSearch.ParentIds ?? ""}'," +
                    $"@StudentIds = '{baseSearch.StudentIds ?? ""}'," +
                    $"@SortType = {baseSearch.SortType}";
                var data = await db.Database.SqlQuery<Get_UserInformation>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new UserInformationModel(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public static async Task ImportData(List<RegisterModel> model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (!model.Any())
                            throw new Exception("Không tìm thấy dữ liệu");
                        foreach (var item in model)
                        {
                            var checkUser = await db.tbl_UserInformation
                                .Where(x => x.UserName.ToUpper() == item.UserName.ToUpper() && x.Enable == true).AnyAsync();
                            if (checkUser)
                                throw new Exception($"Tên đăng nhập {item.UserName} đã tồn tại");

                            if (!UserNameFormat(item.UserName))
                                throw new Exception($"Tài khoản đăng nhập {item.UserName} không hợp lệ");
                            var newUser = new tbl_UserInformation(item);
                            newUser.CreatedBy = newUser.ModifiedBy = user.FullName;

                            if (user.RoleId == ((int)RoleEnum.sale))
                                newUser.SaleId = user.UserInformationId;

                            if (!string.IsNullOrEmpty(user.BranchIds))
                            {
                                newUser.BranchIds = user.BranchIds.Substring(0, 1);
                            }

                            newUser.ActiveDate = DateTime.Now;
                            db.tbl_UserInformation.Add(newUser);
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
        public static async Task<bool> Update_OneSignal_DeviceId(string oneSignal_deviceId, tbl_UserInformation userInformation)
        {
            using (var db = new lmsDbContext())
            {
                if (!string.IsNullOrEmpty(oneSignal_deviceId))
                {
                    var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == userInformation.UserInformationId);
                    user.OneSignal_DeviceId = oneSignal_deviceId;
                    await db.SaveChangesAsync();
                }
                return true;
            }
        }
        public static async Task AutoInActive()
        {
            using (var db = new lmsDbContext())
            {
                var time = DateTime.Now.AddMonths(-3);
                var users = await db.tbl_UserInformation
                    .Where(x => x.StatusId == ((int)AccountStatus.active) && x.Enable == true && x.ActiveDate < time && x.RoleId == ((int)RoleEnum.student))
                    .Select(x => x.UserInformationId).ToListAsync();
                if (users.Any())
                {
                    foreach (var item in users)
                    {
                        var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == item);
                        user.StatusId = ((int)AccountStatus.inActive);
                        await db.SaveChangesAsync();
                    }
                }
            }
        }
        public class Get_UserAvailable
        {
            public int UserInformationId { get; set; }
            public string FullName { get; set; }
            public string UserCode { get; set; }
            public int RoleId { get; set; }
            public string RoleName { get; set; }
        }
        public static async Task<List<Get_UserAvailable>> GetUserAvailable(int roleId, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                int saleId = 0;
                if (userLog.RoleId == ((int)RoleEnum.sale))
                    saleId = userLog.UserInformationId;
                string sql = $"Get_UserAvailable @RoleId = {roleId}, @SaleId = {saleId}";
                var data = await db.Database.SqlQuery<Get_UserAvailable>(sql).ToListAsync();
                return data;
            }
        }
    }
}