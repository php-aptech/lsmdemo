using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Areas.Request
{
    public class BaseUpdate
    {
        [Required(ErrorMessage = "Id is required")]
        public int? Id { get; set; }
        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
    }
    public class PaymentSessionUpdate : BaseUpdate
    {
        public string PrintContent { get; set; }
    }
    public class FileCurriculumInClassUpdate : BaseUpdate
    {
        public int? Index { get; set; }   
    }
    public class CurriculumDetailInClassUpdate: BaseUpdate
    {
        public string Name { get; set; }
        public int? Index { get; set; }
    }
    public class CurriculumInClassUpdate : BaseUpdate
    {
        public string Name { get; set; }
    }
    public class FileInCurriculumDetailUpdate : BaseUpdate
    {
        public int? Index { get; set; }
    }
   
    public class DocumentLibraryUpdate : BaseUpdate
    {
        public string Background { get; set; }
        public string FileUrl { get; set; }
        public string Description { get; set; }
    }
    public class DocumentLibraryDirectoryUpdate : BaseUpdate
    {
        /// <summary>
        /// Tên chủ đề
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }
    }
    public class FeedbackReplyUpdate : BaseUpdate
    {
        /// <summary>
        /// Nội dung
        /// </summary>
        [Required(ErrorMessage ="Vui lòng nhập nội dung")]
        public string Content { get; set; }
    }
    public class FeedbackUpdate : BaseUpdate
    {
        public string Title { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Cờ ưu tiên
        /// </summary>
        public bool? IsPriority { get; set; }
        /// <summary>
        /// Cờ ẩn danh
        /// </summary>
        public bool? IsIncognito { get; set; }
        public int? Status { get; set; }
    }

    public class ContractUpdate : BaseUpdate
    {
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Content { get; set; }
    }
    public class NewsFeedReplyUpdate : BaseUpdate
    {
        /// <summary>
        /// Nội dung bình luận
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Content { get; set; }
    }
    public class NewsFeedCommentUpdate : BaseUpdate
    {
        /// <summary>
        /// Nội dung bình luận
        /// </summary>
        [Required(ErrorMessage ="Vui lòng nhập nội dung")]
        public string Content { get; set; }
    }
    public class ScheduleAvailableUpdate : BaseUpdate
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Note { get; set; }
    }
    public class SalaryUpdate : BaseUpdate
    { /// <summary>
      /// Khấu trừ
      /// </summary>
        public double? Deduction { get; set; }
        /// <summary>
        /// Thưởng thêm
        /// </summary>
        public double? Bonus { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        /// <summary>
        /// 1 - Chưa chốt
        /// 2 - Đã chốt
        /// 3 - Đã thanh toán
        /// </summary>
        public int? Status { get; set; }
        [JsonIgnore]
        public string StatusName { get
            {
                return Status == 1 ? "Chưa chốt"
                        : Status == 2 ? "Đã chốt"
                        : Status == 3 ? "Đã thanh toán" : null;
            }
        }
    }
    public class PackageSkillUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string Thumbnail { get; set; }
    }
    public class PackageSectionUpdate : BaseUpdate
    {
        public string Name { get; set; }
    }


    public class NewsFeedUpdate : BaseUpdate
    {
        /// <summary>
        /// Nội dung bản tin
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Màu Bản tin
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// Nền Bản tin
        /// </summary>
        public string BackGroundUrl { get; set; }
        /// <summary>
        /// Nhóm
        /// </summary>
        public int? NewsFeedGroupId { get; set; }
        /// <summary>
        /// Danh sách chi nhánh
        /// </summary>
        public List<int?> ListBranchId { get; set; }

        /// <summary>
        /// File => tbl_NewsFeedFile
        /// </summary>
        public List<NewsFeedFile> FileListUpdate { get; set; }
    }

    public class UserInNFGroupUpdate : BaseUpdate
    {
        /// <summary>
        /// Kiểu role
        /// 1 - Quản trị viên
        /// 2 - Thàn viên
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn chức vụ")]
        public int? Type { get; set; }

        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == 1 ? "Quản trị viên" : Type == 2 ? "Thành viên" : "";
            }
        }
    }
    public class NewsFeedGroupUpdate : BaseUpdate
    {
        /// <summary>
        /// Tên nhóm
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Hình nền
        /// </summary>
        public string BackGround { get; set; }
    }
    public class CartUpdate : BaseUpdate
    {
        public int? Quantity { get; set; }
    }
    public class RefundUpdate : BaseUpdate
    {
        /// <summary>
        /// 1 - Chờ duyệt
        /// 2 - Đã duyệt
        /// 3 - Hủy
        /// </summary>
        public int? Status { get; set; }
        [JsonIgnore]
        public string StatusName
        {
            get
            {
                return Status == 1 ? "Chờ duyệt"
                        : Status == 2 ? "Đã duyệt"
                        : Status == 3 ? "Hủy" : null;
            }
        }
    }
    public class ClassReserveUpdate : BaseUpdate
    {
        public string Note { get; set; }
        /// <summary>
        /// Ngày hết hạn bảo lưu
        /// </summary>
        public DateTime? Expires { get; set; }
    }
    public class ClassRegistrationUpdate : BaseUpdate
    {
        public string Note { get; set; }
    }
    public class StudentInClassUpdate : BaseUpdate
    {
        public bool? Warning { get; set; }
        public string Note { get; set; }
    }
    public class PaymentMethodUpdate : BaseUpdate
    {
        public bool? Active { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
    }
    public class ScheduleUpdate : BaseUpdate
    {
        public int? RoomId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? TeacherId { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// Dùng cho dạy kèm
        /// 1 - Mới đặt 
        /// 2 - Hủy
        /// 3 - Đã học 
        /// 4 - Giáo viên vắng mặt
        /// 5 - Sự cố kỹ thuật
        /// 6 - Giáo viên vào trễ
        /// 7 - Học viên vắng mặt
        /// </summary>
        public int? StatusTutoring { get; set; }
        /// <summary>
        /// Dùng cho dạy kèm
        /// </summary>
        [JsonIgnore]
        public string StatusTutoringName { get 
            {
                return StatusTutoring == 1 ? "Mới đặt"
                        : StatusTutoring == 2 ? "Hủy"
                        : StatusTutoring == 3 ? "Đã học"
                        : StatusTutoring == 4 ? "Giáo viên vắng mặt"
                        : StatusTutoring == 5 ? "Sự cố kỹ thuật"
                        : StatusTutoring == 6 ? "Giáo viên vào trễ"
                        : StatusTutoring == 7 ? "Học viên vắng mặt" : null;
            }
        }
        public double? TeachingFee { get; set; }
    }
    public class ClassUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public double? Price { get; set; }
        /// <summary>
        /// 1 - Sắp diễn ra
        /// 2 - Đang diễn ra
        /// 3 - Kết thúc
        /// </summary>
        public int? Status { get; set; }
        [JsonIgnore]
        public string StatusName
        {
            get
            {
                return Status == 1 ? "Sắp diễn ra"
                         : Status == 2 ? "Đang diễn ra"
                         : Status == 3 ? "Kết thúc" : null;
            }
        }
        /// <summary>
        /// Học vụ
        /// </summary>
        public int? AcademicId { get; set; }
        /// <summary>
        /// Giáo viên
        /// </summary>
        public int? TeacherId { get; set; }
        /// <summary>
        /// Số lượng tối đa
        /// </summary>
        public int? MaxQuantity { get; set; }
    }
    public class TeacherOffUpdate : BaseUpdate
    {
        /// <summary>
        /// 1 - Chờ duyệt
        /// 2 - Duyệt
        /// 3 - Không duyệt
        /// </summary>
        public int? Status { get; set; }
        [StringLength(20)]
        [JsonIgnore]
        public string StatusName
        {
            get
            {
                return Status == 1 ? "Chờ duyệt"
                    : Status == 2 ? "Duyệt"
                    : Status == 3 ? "Không duyệt" : null;
            }
        }
        /// <summary>
        /// ghi chú
        /// </summary>
        [StringLength(1000)]
        public string Note { get; set; }
    }
    public class StudyTimeUpdate : BaseUpdate
    {
        /// <summary>
        /// định dạng HH:mm
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// định dạng HH:mm
        /// </summary>
        public string EndTime { get; set; }
    }
    public class CurriculumDetailUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public int? Index { get; set; }
    }

    public class CurriculumUpdate : BaseUpdate
    {
        public string Name { get; set; }
        /// <summary>
        /// Số buổi
        /// </summary>
        public int? Lesson { get; set; }
        /// <summary>
        /// Thời gian
        /// </summary>
        public int? Time { get; set; }
    }
    public class ProgramUpdate : BaseUpdate
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }
        public string Description { get; set; }
    }
    public class GradeUpdate : BaseUpdate
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class TestAppointmentUpdate : BaseUpdate
    {
        public DateTime? Time { get; set; }
        /// <summary>
        /// 1 - Chưa kiểm tra
        /// 2 - Đã kiểm tra 
        /// </summary>
        public int? Status { get; set; }
        public string StatusName
        {
            get
            {
                return Status == 1 ? "Chưa kiểm tra"
                    : Status == 2 ? "Đã kiểm tra" : null;
            }
        }
        public int? TeacherId { get; set; }
        /// <summary>
        /// 1 - Tại trung tâm
        /// 2 - Làm bài trực tuyến
        /// </summary>
        public int? Type { get; set; }
        public string TypeName
        {
            get
            {
                return Type == 1 ? "Tại trung tâm"
                    : Type == 2 ? "Làm bài trực tuyến" : null;
            }
        }
        public string ListeningPoint { get; set; }
        public string SpeakingPoint { get; set; }
        public string ReadingPoint { get; set; }
        public string WritingPoint { get; set; }
        public string Vocab { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// Học phí tối đa, dùng để tư vấn khóa học
        /// </summary>
        public string Tuitionfee { get; set; }
        /// <summary>
        /// Đề
        /// </summary>
        public int? ExamId { get; set; }
    }
    public class CustomerUpdate : BaseUpdate
    {
        public int? LearningNeedId { get; set; }
        public int? CustomerStatusId { get; set; }
        public string FullName { get; set; }
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Mobile { get; set; }
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        /// <summary>
        /// Nguồn khách hàng
        /// </summary>
        public int? SourceId { get; set; }
        /// <summary>
        /// Tư vấn viên
        /// </summary>
        public int? SaleId { get; set; }
        /// <summary>
        /// Mục đích học
        /// </summary>
        public int? PurposeId { get; set; }
        public int? AreaId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string Address { get; set; }
        /// <summary>
        /// Trung tâm
        /// </summary>
        public int? BranchId { get; set; }
        public int? JobId { get; set; }
    }
    public class CustomerStatusUpdate : BaseUpdate
    {
        public string Name { get; set; }
    }
    public class FrequentlyQuestionUpdate : BaseUpdate
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
    public class TemplateUpdate
    {
        /// <summary>
        /// 1 - Hợp đồng
        /// 2 - Điều khoản
        /// 3 - Mẫu phiếu thu
        /// 4 - Mẫu phiếu chi
        /// </summary>
        public int? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == 1 ? "Hợp đồng"
                    : Type == 2 ? "Điều khoản"
                    : Type == 3 ? "Phiếu thu"
                    : Type == 4 ? "Phiếu chi" : "";
            }
        }
        public string Content { get; set; }
    }
    public class IdiomUpdate : BaseUpdate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Content { get; set; }
    }
    public class PurposeUpdate : BaseUpdate
    {
        public string Name { get; set; }
    }
    public class JobUpdate : BaseUpdate
    {
        public string Name { get; set; }
    }
    public class DayOffUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
    }

    public class SourceUpdate : BaseUpdate
    {
        public string Name { get; set; }
    }
    public class LearningNeedUpdate : BaseUpdate
    {
        public string Name { get; set; }
    }
    public class DiscountUpdate : BaseUpdate
    {
        public double? Value { get; set; }
        /// <summary>
        /// 1 - Đang diễn ra
        /// 2 - Đã kết thúc
        /// </summary>
        public int? Status { get; set; }
        public string StatusName
        {
            get
            {
                return Status == 1 ? "Đang diễn ra"
                    : Status == 2 ? "Đã kết thúc" : null;
            }
        }
        public string Note { get; set; }
        public DateTime? Expired { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        public int? Quantity { get; set; }
        /// <summary>
        /// Khuyến mãi tối đa
        /// </summary>
        public double? MaxDiscount { get; set; }
    }
    public class RoomUpdate : BaseUpdate
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class BranchUpdate : BaseUpdate
    {

        public string Code { get; set; }
        public string Name { get; set; }
        public int? AreaId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string Address { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
    public class SeminarRecordUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string VideoUrl { get; set; }
    }
    public class ExamSectionUpdate : BaseUpdate
    {
        public string Name { get; set; }
        /// <summary>
        /// Nội dung chú thích đáp án đúng của đoạn văn
        /// </summary>
        public string Explanations { get; set; }
    }
    public class CertificateUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
    public class ExerciseGroupUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string Paragraph { get; set; }
        public ExerciseLevel? Level { get; set; }
        [JsonIgnore]
        public string LevelName
        {
            get { return Level == ExerciseLevel.Easy ? "Dễ" : Level == ExerciseLevel.Normal ? "Trung bình" : Level == ExerciseLevel.Difficult ? "Khó" : ""; }
        }
        public string Tags { get; set; }
        public List<ExerciseUpdate> ExerciseUpdates { get; set; }
    }
    public class ExerciseUpdate : BaseUpdate
    {
        public string Content { get; set; }
        /// <summary>
        /// Áp dụng cho câu kéo thả và điền từ
        /// </summary>
        public string InputId { get; set; }
        public List<AnswerUpdate> AnswerUpdates { get; set; }
        public string DescribeAnswer { get; set; }
        public int? Index { get; set; }
        public bool? Enable { get; set; }
        public double? Point { get; set; }
    }
    public class AnswerUpdate : BaseUpdate
    {
        public string AnswerContent { get; set; }
        public bool? IsTrue { get; set; }
        public bool? Enable { get; set; }
        public int? Index { get; set; }
    }
    public class ExamUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Thời gian làm bài
        /// </summary>
        public int? Time { get; set; }
        /// <summary>
        /// Số điểm đạt
        /// </summary>
        public double? PassPoint { get; set; }
    }
    public class SeminarUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? VideoCourseId { get; set; }
        public int? LeaderId { get; set; }
        public int? Member { get; set; }
        public string Thumbnail { get; set; }
    }
    public class LessonVideoUpdate : BaseUpdate
    {
        public string Name { get; set; }
        /// <summary>
        /// File video
        /// </summary>
        public string VideoUrl { get; set; }
        /// <summary>
        /// Bài tập
        /// </summary>
        public int? ExamId { get; set; }
    }
    public class SectionUpdate : BaseUpdate
    {
        public string Name { get; set; }
    }
    public class ProductUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public int? BeforeCourseId { get; set; }
        public double? Price { get; set; }
    }
    public class ResetPasswordModel
    {
        public string Key { get; set; }
        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        [StringLength(128, ErrorMessage = "Mật khẩu phải có ít nhất 6 kí tự và tối đa 128 ký tự", MinimumLength = 6)]
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc nhập")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Xác nhận mật khẩu mới
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu mới")]
        [StringLength(128, ErrorMessage = "Mật khẩu xác nhận phải có ít nhất 6 kí tự và tối đa 128 ký tự", MinimumLength = 6)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không đúng")]
        public string ConfirmNewPassword { get; set; }
    }
    public class ChangePasswordModel
    {
        /// <summary>
        /// Mật khẩu cũ
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        [StringLength(128, ErrorMessage = "Mật khẩu phải có ít nhất 6 kí tự và tối đa 128 ký tự", MinimumLength = 6)]
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc nhập")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Xác nhận mật khẩu mới
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu mới")]
        [StringLength(128, ErrorMessage = "Mật khẩu xác nhận phải có ít nhất 6 kí tự và tối đa 128 ký tự", MinimumLength = 6)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không đúng")]
        public string ConfirmNewPassword { get; set; }
    }
    public class UserUpdate
    {
        [Key]
        public int UserInformationId { get; set; }
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public DateTime? DOB { get; set; }
        /// <summary>
        /// 0 - Khác
        /// 1 - Nam
        /// 2 - Nữ
        /// </summary>
        public int? Gender { get; set; }
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Mobile { get; set; }
        public string Address { get; set; }
        /// <summary>
        /// 0 - Hoạt động
        /// 1 - Khoá
        /// </summary>
        public int? StatusId { get; set; }
        public string Avatar { get; set; }
        public int? AreaId { get; set; } // tình/tp
        public int? DistrictId { get; set; }//quận/huyện
        public int? WardId { get; set; } // phường/xã 
        public string Password { get; set; }
        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        /// <summary>
        /// Trung tâm
        /// Mẫu 1,2
        /// </summary>
        public string BranchIds { get; set; }
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
        public int? SaleId { get; set; }
        /// <summary>
        /// Mục đích học
        /// </summary>
        public int? PurposeId { get; set; }
        public int? ParentId { get; set; }
        public string Extension { get; set; }
    }
    public class StudyRouteUpdate : BaseUpdate
    {
        [Required(ErrorMessage = "Họp viên không được bỏ trống")]
        public int? StudentId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn chương trình")]
        public int? ProgramId { get; set; }
        public string Note { get; set; }
        public int? Status { get; set; }
    }
    public class CertificateTemplateUpdate : BaseUpdate
    {
        [Required(ErrorMessage = "Tên chứng chỉ không được bỏ trống")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Nội dung không được bỏ trống")]
        public string Content { get; set; }
    }
}