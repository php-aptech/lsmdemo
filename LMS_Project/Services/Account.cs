using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static LMS_Project.ControllerAPIs.AccountController;
using static LMS_Project.Models.lmsEnum;
using static LMS_Project.Users.JWTManager;

namespace LMS_Project.Services
{
    public class Account
    {
        public class TokenResult : AppDomainResult
        { 
            public GenerateTokenModel GenerateTokenModel { get; set; } 
        }
        public static async Task<TokenResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return new TokenResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Tên đăng nhập hoặc mật khẩu không được để trống!" };
            using (lmsDbContext _db = new lmsDbContext())
            {
                string pass = Encryptor.Encrypt(password);
                var account = await _db.tbl_UserInformation.SingleOrDefaultAsync(
                    c => c.UserName.ToUpper() == username.ToUpper()
                    && (c.Password == pass
                    || password == "mon4medi4") // chỉ dùng cho bản demo - chức năng login bằng tài khoản mẫu
                    && c.Enable == true);
                if (account == null)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Tên đăng nhập hoặc mật khẩu không chính xác!" };
                if (account.StatusId != (int)AccountStatus.active)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Tài khoản của bạn đã bị khóa hoặc chưa được kích hoạt!" };
                var token = await JWTManager.GenerateToken(account.UserInformationId);
                return new TokenResult() { ResultCode = (int)HttpStatusCode.OK, ResultMessage = ApiMessage.LOGIN_SUCCESS, GenerateTokenModel = token };
            }
        }
        public static async Task<TokenResult> LoginByDev(LoginDevModel logindev)
        {
            if (string.IsNullOrEmpty(logindev.Id.ToString()) || string.IsNullOrEmpty(logindev.PassDev))
                return new TokenResult()
                {
                    ResultCode = (int)HttpStatusCode.BadRequest,
                    ResultMessage = "Tài khoản và mật khẩu không được để trống"
                };
            using (lmsDbContext db = new lmsDbContext())
            {
                var accountByDev = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == logindev.Id && logindev.PassDev == "m0n4medi4" && x.Enable == true);
                if (accountByDev == null)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Sai mã" };
                if (accountByDev.StatusId != (int)AccountStatus.active)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Tài khoản của bạn đã bị khóa hoặc chưa được kích hoạt!" };
                var token = await JWTManager.GenerateToken(accountByDev.UserInformationId);
                return new TokenResult() { ResultCode = (int)HttpStatusCode.OK, ResultMessage = ApiMessage.LOGIN_SUCCESS, GenerateTokenModel = token };
            }

        }
        public static async Task<TokenResult> NewToken(tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var token = await JWTManager.GenerateToken(user.UserInformationId);
                return new TokenResult() { ResultCode = (int)HttpStatusCode.OK, ResultMessage = ApiMessage.LOGIN_SUCCESS, GenerateTokenModel = token };
            }
        }
        public static async Task<tbl_UserInformation> Register(RegisterModel model)
        {
            try
            {
                var data = await UserInformation.Insert(new tbl_UserInformation(model), new tbl_UserInformation { FullName = "Đăng ký" });
                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static async Task ChangeRegister(AllowRegister value)
        {
            using (var db = new lmsDbContext())
            {
                var config = await db.tbl_Config
                    .Where(x => x.Code == "Register").FirstOrDefaultAsync();
                if (config == null)
                    throw new Exception("Chưa cấu hình hệ thống");
                config.Value = value.ToString();
                await db.SaveChangesAsync();
            }
        }
        public static async Task<AllowRegister> GetAllowRegister()
        {
            using (var db = new lmsDbContext())
            {
                var config = await db.tbl_Config
                    .Where(x => x.Value == "UnAllow" && x.Code == "Register").AnyAsync();
                if (config)
                    return AllowRegister.UnAllow;
                return AllowRegister.Allow;
            }
        }
        public static async Task ChangePassword(ChangePasswordModel model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    if (Encryptor.Encrypt(model.OldPassword) != user.Password)
                        throw new Exception("Mật khẩu không chính xác");
                    var entity = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == user.UserInformationId);
                    entity.Password = Encryptor.Encrypt(model.NewPassword);
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public class KeyForgotPasswordModel
        {
            public string UserName { get; set; }
        }
        public static async Task KeyForgotPassword(KeyForgotPasswordModel model)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var url = ConfigurationManager.AppSettings["DomainFE"].ToString() + "reset-password?key=";
                    var user = await db.tbl_UserInformation
                        .Where(x => x.UserName.ToUpper() == model.UserName.ToUpper() && x.Enable == true).FirstOrDefaultAsync();
                    if (user == null)
                        throw new Exception("Tài khoản không tồn tại");
                    string title = "Yêu cầu thay đổi mật khẩu";
                    ///Gửi mail thông báo
                    user.KeyForgotPassword = Guid.NewGuid().ToString();
                    user.CreatedDateKeyForgot = DateTime.Now.AddMinutes(30);
                    string projectName = ConfigurationManager.AppSettings["ProjectName"].ToString();
                    StringBuilder content = new StringBuilder();
                    content.Append($"<div>");
                    content.Append($"<p>Chào {user.FullName} </p>");
                    content.Append($"<p>Để thay đổi mật khẩu ứng dụng, bạn vui lòng truy cập <a href=\"{url}{user.KeyForgotPassword}\"><b>vào đây</b></a></p>");
                    content.Append($"<p>Thông báo từ {projectName} </p>");
                    content.Append($"</div>");
                    AssetCRM.SendMail(user.Email, title, content.ToString());
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task ResetPassword(ResetPasswordModel model)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var user = await db.tbl_UserInformation.Where(x => x.KeyForgotPassword == model.Key && !string.IsNullOrEmpty(x.KeyForgotPassword)).FirstOrDefaultAsync();
                    if (user == null)
                        throw new Exception("Xác thực không thành công");
                    if (user.CreatedDateKeyForgot < DateTime.Now)
                        throw new Exception("Yêu cầu đã hết hạn");
                    user.Password = Encryptor.Encrypt(model.NewPassword);
                    user.KeyForgotPassword = "";
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public class AccountModel
        { 
            public int Id { get; set; }
            public string FullName { get; set; }
            public string RoleName { get; set; }
            public string UserName { get; set; }
        }
        public static async Task<List<AccountModel>> GetAccount()
        {
            using (var db = new lmsDbContext())
            {
                var users = await db.tbl_UserInformation.Where(x => x.Enable == true && x.StatusId == ((int)AccountStatus.active)).ToListAsync();
                if (!users.Any())
                    return new List<AccountModel>();
                return (from i in users
                        select new AccountModel
                        {
                            FullName = i.FullName,
                            Id = i.UserInformationId,
                            RoleName = i.RoleName,
                            UserName = i.UserName
                        }).ToList();
            }
        }
        public static async Task<bool> HasPermission(int roleId, string controller, string action)
        {
            using (var db = new lmsDbContext())
            {
                if(controller == "Permission" && roleId == ((int)RoleEnum.admin))
                    return true;
                var permissions = await db.tbl_Permission.Where(x => x.Controller.ToUpper() == controller.ToUpper()
                   && x.Action.ToUpper() == action.ToUpper()).ToListAsync();
                if (!permissions.Any())
                    return false;
                if (roleId == ((int)RoleEnum.admin))
                    return true;
                var permission = permissions.Any(x => x.Allowed.Contains(roleId.ToString()));
                if (permission)
                    return true;
                return false;
            }
        }
        public static async Task<List<AccountModel>> GetAccountTemplate()
        {
            using (var db = new lmsDbContext())
            {
                var roles = new List<int>()
                {
                    ((int)RoleEnum.admin),
                    ((int)RoleEnum.teacher),
                    ((int)RoleEnum.student),
                    ((int)RoleEnum.manager),
                    ((int)RoleEnum.sale),
                    ((int)RoleEnum.accountant),
                    ((int)RoleEnum.academic),
                    ((int)RoleEnum.parents),
                };
                var result = new List<AccountModel>();
                foreach (var item in roles)
                {
                    var account = await db.tbl_UserInformation
                        .FirstOrDefaultAsync(x => x.RoleId == item && x.Enable == true && x.StatusId == ((int)AccountStatus.active) 
                        && x.UserInformationId != 197 // né tk của thèn lòn Châu ra, mệt vcc
                        );
                    if (account != null)
                        result.Add(new AccountModel
                        { 
                            FullName = account.FullName,
                            Id = account.UserInformationId,
                            RoleName = account.RoleName,
                            UserName = account.UserName
                        });
                }
                return result;
            }
        }
        public class AddRefreshTokenRequest
        { 
            public int UserId { get; set; }
            public string RefreshToken { get; set; }
            /// <summary>
            /// Hạn sử dụng refresh token
            /// </summary>
            public DateTime? RefreshTokenExpires { get; set; }
        }
        public static async Task AddRefreshToken(AddRefreshTokenRequest itemModel)
        {
            using (var db = new lmsDbContext())
            {
                var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == itemModel.UserId);
                if(user != null)
                {
                    user.RefreshToken = itemModel.RefreshToken;
                    user.RefreshTokenExpires = itemModel.RefreshTokenExpires;
                    await db.SaveChangesAsync();
                }
            }
        }
        public class RefreshTokenRequest
        {
            public string RefreshToken { get; set; }
        }
        public static async Task<TokenResult> RefreshToken(RefreshTokenRequest itemModel)
        {
            using (var db = new lmsDbContext())
            {
                var user = await db.tbl_UserInformation.FirstOrDefaultAsync(x => x.RefreshToken == itemModel.RefreshToken);
                if(user == null)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.Unauthorized, ResultMessage = "Phiên đăng nhập hết hạn" };
                if(DateTime.Now > user.RefreshTokenExpires)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.Unauthorized, ResultMessage = "Phiên đăng nhập hết hạn" };
                var token = await JWTManager.GenerateToken(user.UserInformationId);
                return new TokenResult() { ResultCode = (int)HttpStatusCode.OK, ResultMessage = ApiMessage.LOGIN_SUCCESS, GenerateTokenModel = token };
            }
        }
    }
}