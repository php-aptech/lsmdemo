using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace LMS_Project.Models
{
    public partial class tbl_UserInformation
    {
        [Key]
        public int UserInformationId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public DateTime? DOB { get; set; } // ngày sinh
        /// <summary>
        /// 0 - Khác
        /// 1 - Nam
        /// 2 - Nữ
        /// </summary>
        public int? Gender { get; set; }
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Mobile { get; set; } // số điện thoại
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        public string Address { get; set; }
        public int? StatusId { get; set; } // 1 khóa 0 hoạt động
        public int? RoleId { get; set; } //1 admin 2 giáo viên 3 học viên
        public string RoleName { get; set; }
        public string Avatar { get; set; }
        public int? AreaId { get; set; } // tình/tp
        public int? DistrictId { get; set; }//quận/huyện
        public int? WardId { get; set; } // phường/xã 
        public string OneSignal_DeviceId { get; set; }
        public string Password { get; set; }

        public string KeyForgotPassword { get; set; }

        public DateTime? CreatedDateKeyForgot { get; set; }
        /// <summary>
        /// trung tâm
        /// </summary>
        public string BranchIds { get; set; }
        /// <summary>
        /// Ngày hoạt động
        /// </summary>
        public DateTime ActiveDate { get; set; }
        /// <summary>
        /// 1 - Mới
        /// 2 - Đang học
        /// 3 - Học xong
        /// </summary>
        public int LearningStatus { get; set; }
        public string LearningStatusName { get; set; }
        /// <summary>
        /// Mục đích học
        /// </summary>
        public int? SourceId { get; set; }
        /// <summary>
        /// Nhu cầu học
        /// </summary>
        public int? LearningNeedId { get; set; }
        /// <summary>
        /// Tư vấn viên
        /// </summary>
        public int SaleId { get; set; }
        /// <summary>
        /// Mục đích học
        /// </summary>
        public int? PurposeId { get; set; }
        public string Extension { get; set; }
        public bool? Enable { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
        /// <summary>
        /// id cha
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// nghề nghiệp
        /// </summary>
        public int? JobId { get; set; }
        public string RefreshToken { get;set; }
        /// <summary>
        /// Hạn sử dụng refresh token
        /// </summary>
        public DateTime? RefreshTokenExpires { get; set; }
        public tbl_UserInformation() { }
        public tbl_UserInformation(object model)
        {
            foreach (PropertyInfo me in this.GetType().GetProperties())
            {
                foreach (PropertyInfo item in model.GetType().GetProperties())
                {
                    if (me.Name == item.Name)
                    {
                        me.SetValue(this, item.GetValue(model));
                    }
                }
            }
        }
    }
    public class Get_UserInformation
    {
        [Key]
        public int UserInformationId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public DateTime? DOB { get; set; } = DateTime.Now; // ngày sinh
        /// <summary>
        /// 0 - Khác
        /// 1 - Nam
        /// 2 - Nữ
        /// </summary>
        public int? Gender { get; set; }
        public string Mobile { get; set; } // số điện thoại
        public string Email { get; set; }
        public string Address { get; set; }
        public int? StatusId { get; set; } // 1 khóa 0 hoạt động
        public int? RoleId { get; set; } //1 admin 2 giáo viên 3 học viên
        public string RoleName { get; set; }
        public string Avatar { get; set; }
        public int? AreaId { get; set; } // tình/tp
        public int? DistrictId { get; set; }//quận/huyện
        public int? WardId { get; set; } // phường/xã 
        public string OneSignal_DeviceId { get; set; }
        public string Password { get; set; }

        public string KeyForgotPassword { get; set; }

        public DateTime? CreatedDateKeyForgot { get; set; }
        /// <summary>
        /// trung tâm
        /// </summary>
        public string BranchIds { get; set; }
        /// <summary>
        /// 1 - Mới
        /// 2 - Đang học
        /// 3 - Học xong
        /// </summary>
        public int LearningStatus { get; set; }
        public string LearningStatusName { get; set; }
        /// <summary>
        /// Mục đích học
        /// </summary>
        public int? SourceId { get; set; }
        /// <summary>
        /// Nhu cầu học
        /// </summary>
        public int? LearningNeedId { get; set; }
        /// <summary>
        /// Tư vấn viên
        /// </summary>
        public int SaleId { get; set; }
        /// <summary>
        /// Mục đích học
        /// </summary>
        public int? PurposeId { get; set; }
        public string Extension { get; set; }
        /// <summary>
        /// nghề nghiệp
        /// </summary>
        public int? JobId { get; set; }
        public bool? Enable { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
        public string SourceName { get; set; }
        public string LearningNeedName { get; set; }
        public string PurposeName { get; set; }
        public string SaleName { get; set; }
        public string AreaName { get; set; }
        public string WardName { get; set; }
        public string DistrictName { get; set; }
        public int TotalRow { get; set; }
    }
    public class UserInformationModel
    {
        public int UserInformationId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public DateTime? DOB { get; set; } // ngày sinh
        /// <summary>
        /// 0 - Khác
        /// 1 - Nam
        /// 2 - Nữ
        /// </summary>
        public int? Gender { get; set; }
        public string Mobile { get; set; } // số điện thoại
        public string Email { get; set; }
        public string Address { get; set; }
        public int? StatusId { get; set; } // 1 khóa 0 hoạt động
        public int? RoleId { get; set; } //1 admin 2 giáo viên 3 học viên
        public string RoleName { get; set; }
        public string Avatar { get; set; }
        public int? AreaId { get; set; } // tình/tp
        public int? DistrictId { get; set; }//quận/huyện
        public int? WardId { get; set; } // phường/xã 
        /// <summary>
        /// trung tâm
        /// </summary>
        public string BranchIds { get; set; }
        /// <summary>
        /// 1 - Mới
        /// 2 - Đang học
        /// 3 - Học xong
        /// </summary>
        public int LearningStatus { get; set; }
        public string LearningStatusName { get; set; }
        /// <summary>
        /// Mục đích học
        /// </summary>
        public int? SourceId { get; set; }
        /// <summary>
        /// Nhu cầu học
        /// </summary>
        public int? LearningNeedId { get; set; }
        /// <summary>
        /// Tư vấn viên
        /// </summary>
        public int SaleId { get; set; }
        /// <summary>
        /// Mục đích học
        /// </summary>
        public int? PurposeId { get; set; }
        public string Extension { get; set; }
        /// <summary>
        /// nghề nghiệp
        /// </summary>
        public int? JobId { get; set; }
        public bool? Enable { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
        public string SourceName { get; set; }
        public string LearningNeedName { get; set; }
        public string PurposeName { get; set; }
        public string SaleName { get; set; }
        public string AreaName { get; set; }
        public string WardName { get; set; }
        public string DistrictName { get; set; }
        public UserInformationModel() { }
        public UserInformationModel(object model)
        {
            foreach (PropertyInfo me in this.GetType().GetProperties())
            {
                foreach (PropertyInfo item in model.GetType().GetProperties())
                {
                    if (me.Name == item.Name)
                    {
                        me.SetValue(this, item.GetValue(model));
                    }
                }
            }
        }
    }
}
