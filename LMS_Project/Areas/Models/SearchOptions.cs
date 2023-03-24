using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Areas.Models
{
    public class ScheduleTeacherOffSearch : SearchOptions
    {
        public int TeacherOffId { get; set; }
    }
    public class CertificateTemplateSearch : SearchOptions
    {
        public string Name { get; set; }
    }
    public class ChartSearch
    {
        public string BranchIds { get; set; }
        public int? Year { get; set; }
    }

    public class OverviewFilter
    {
        public string BranchIds { get; set; }
        public int? UserId { get; set; }
    }
    public class StudyRouteSearch : SearchOptions
    {
        public int? StudentId { get; set; }
    }
    public class PointSearch : SearchOptions
    {
        public string ParentIds { get; set; }
        public string StudentIds { get; set; }
        public int ClassId { get; set; } = 0;
    }
    public class CurriculumDetailInClassSearch
    {
        public int? CurriculumIdInClassId { get; set; }
    }
    public class StudentAssessmentSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        public int? ClassId { get; set; }
    }
    public class FilesCurriculumInClassSearch
    {
        public int? CurriculumDetailInClassId { get; set; }
    }
    public class DocumentLibrarySearch : SearchOptions
    {
        public int? DirectoryId { get; set; }
        /// <summary>
        /// 0 - Ngày tạo
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; } = false;
    }
    public class DocumentLibraryDirectorySearch
    {
        public string Name { get; set; }
        /// <summary>
        /// 0 - Ngày tạo
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; } = false;
    }
    public class FeedbackReplySearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn phản hồi")]
        public int? FeedbackId { get; set; }
        /// <summary>
        /// 0 - Ngày tạo
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; } = false;

    }
    public class FeedbackSearch : SearchOptions
    {
        /// <summary>
        /// Lọc theo trạng thái
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 0 - Ngày tạo
        /// 1 - Số sao đánh giá 
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; } = false;
        public string UserIds { get; set; }

    }
    public class ContractSearch : SearchOptions
    {
        public int StudentId { get; set; } = 0;
    }
    public class NewsFeedReplySearch : SearchOptions
    {
        [Required(ErrorMessage = ("Vui lòng chọn bình luận"))]
        public int? NewsFeedCommentId { get; set; }
    }
    public class NewsFeedCommentSearch : SearchOptions
    {
        /// <summary>
        /// Lọc theo id bản tin
        /// </summary>
        [Required(ErrorMessage = ("Vui lòng chọn bản tin"))]
        public int? NewsFeedId { get; set; }
        /// <summary>
        /// 0 : Ngày tạo
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
    }
    public class ScheduleAvailableSearch : SearchOptions
    {
        public override int PageSize { get; set; } = 999999;
        public int TeacherId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
        public DateTime? From { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
        public DateTime? To { get; set; }
    }
    public class HistoryDonateSearch : SearchOptions
    {
        public int Type { get; set; } = 0;
    }
    public class MarkQuantitySearch : SearchOptions
    {
        public string FullName { get; set; }
        public string UserCode { get; set; }
    }
    public class MarkSalarySearch : SearchOptions
    {
        public string FullName { get; set; }
        public string UserCode { get; set; }
    }
    public class PaymentApproveSearch : SearchOptions
    {
        public int Status { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class PackageStudentSearch : SearchOptions
    {
        /// <summary>
        /// mẫu : 1,2,3
        /// </summary>
        public string Tags { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// 2 - Giá
        /// </summary>
        public int Sort { get; set; } = 0;
        public int StudentId { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
    }
    public class PackageSkillSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn phần")]
        public int PackageSectionId { get; set; }
    }
    public class PackageSectionSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn bộ đề")]
        public int? PackageId { get; set; }
    }

    public class NewsFeedSearch : SearchOptions
    {

        /// <summary>
        /// Lọc theo nhóm
        /// </summary>
        public int? NewsFeedGroupId { get; set; }


        /// <summary>
        /// Lọc theo
        /// </summary>
        public int? BranchIds { get; set; }

    }
    public class UserInNFGroupSearch : SearchOptions
    {
        [Required]
        public int? NewsFeedGroupId { get; set; }
        /// <summary>
        /// Lọc theo tên thành viên
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Lọc theo role gốc của thành viên
        /// Giáo viên
        /// Học vụ
        /// Học viên
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// Lọc theo kiểu thành viên nhóm
        /// Quản trị viên
        /// Thành viên
        /// </summary>
        public int? Type { get; set; }
        /// <summary>
        /// 0 - Ngày tạo
        /// 1 - Tên thành viên
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; } = false;
    }

    public class NewsFeedGroupSearch : SearchOptions
    {
        public string Name { get; set; }
        public int? ClassId { get; set; }
        /// <summary>
        /// 0 - Ngày tạo
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; } = false;
    }
    public class VideoActiveCodeSearch : SearchOptions
    {
        public int BillDetailId { get; set; }
        public int StudentId { get; set; }
    }
    public class TagSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn loại từ khóa")]
        public int? Type { get; set; }
        public int? TagCategoryId { get; set; }
    }
    public class TagCategorySearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn loại")]
        public int? Type { get; set; }
    }
    public class TeachingDetailSearch : SearchOptions
    {
        public int SalaryId { get; set; }
    }
    public class SalarySearch : SearchOptions
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
    }
    public class SalaryConfigSearch : SearchOptions
    {
        public string FullName { get; set; }
        public string UserCode { get; set; }
    }
    public class RefundSearch : SearchOptions
    {
        public string BranhIds { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class ClassReserveSearch : SearchOptions
    {
        public string BranhIds { get; set; }
        public string Status { get; set; }
    }
    public class ClassRegistrationSearch : SearchOptions
    {
        public string BranhIds { get; set; }
        public string ProgramIds { get; set; }
        public string Status { get; set; }
    }
    public class ClassChangeSearch : SearchOptions
    {
        public string BranhIds { get; set; }
    }
    public class NotificationInClassSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        public int? ClassId { get; set; }
    }
    public class TimeLineSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        public int? ClassId { get; set; }
    }
    public class RollUpTeacherSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        public int? ClassId { get; set; }
    }
    public class RollUpSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        public int? ClassId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn buổi")]
        public int? ScheduleId { get; set; }
        public string StudentIds { get; set; }
        public string ParentIds { get; set; }
    }
    public class StudentInClassSearch : SearchOptions
    {
        public int ClassId { get; set; } = 0;
        /// <summary>
        /// Lấy danh sách học viên cảnh báo
        /// </summary>
	    public bool? Warning { get; set; }
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
        public string ParentIds { get; set; }
        public string StudentIds { get; set; }
        public bool disable { get; set; }
    }
    public class PaymentSessionSearch : SearchOptions
    {
        public string BranchIds { get; set; }
        /// <summary>
        /// 1 - Thu 
        /// 2 - Chi
        /// </summary>
        public int Type { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class BillSearch : SearchOptions
    {
        public string StudentIds { get; set; }
        public string BranchIds { get; set; }
        public string ParentIds { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class ScheduleSearch : SearchOptions
    {
        public override int PageSize { get; set; } = 999999;
        public int ClassId { get; set; } = 0;
        public int StudentId { get; set; } = 0;
        /// <summary>
        /// mẫu: 1,2,3
        /// </summary>
	    public string BranchIds { get; set; }
        /// <summary>
        /// mẫu: 1,2,3
        /// </summary>
	    public string TeacherIds { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int ParentId { get; set; } = 0;
    }
    public class ClassSearch : SearchOptions
    {
        public string Name { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string BranchIds { get; set; }
        public string Types { get; set; }
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// 2 - Status
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
        public int StudentId { get; set; }
    }
    public class TeacherOffSearch : SearchOptions
    {
        public string FullName { get; set; }
        public string UserCode { get; set; }
        /// <summary>
        /// 1 - Chờ duyệt
        /// 2 - Duyệt
        /// 3 - Không duyệt
        /// mẫu 1,2
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// id nghỉ phép
        /// </summary>
        public int? teacherOffId { get; set; }
    }
    public class TeacherInProgramSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn chương trình")]
        public int? ProgramId { get; set; }
    }
    public class CurriculumDetailSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn giáo trình")]
        public int? CurriculumId { get; set; }
    }
    public class CurriculumSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn chương trình")]
        public int? ProgramId { get; set; }
    }
    public class ProgramSearch : SearchOptions
    {

        public string Code { get; set; }
        public string Name { get; set; }
        public string Search { get; set; }
        public int GradeId { get; set; } = 0;
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
    }
    public class GradeSearch : SearchOptions
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class TestAppointmentNoteSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn lịch hẹn")]
        public int? TestAppointmentId { get; set; }
    }
    public class TestAppointmentSearch : SearchOptions
    {

        public string FullName { get; set; }
        public string UserCode { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string BranchIds { get; set; }
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
        public int StudentId { get; set; }
    }
    public class CustomerNoteSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public int? CustomerId { get; set; }
    }
    public class CustomerSearch : SearchOptions
    {
        public string FullName { get; set; }
        public string Code { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string CustomerStatusIds { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string BranchIds { get; set; }
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
    }
    public class DiscountSearch : SearchOptions
    {
        public string Code { get; set; }
        public int Status { get; set; }
    }
    public class RoomSearch : SearchOptions
    {
        public int BranchId { get; set; } = 0;
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class BranchSearch : SearchOptions
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class OverviewSearch : SearchOptions
    {
        public string Search { get; set; }
    }
    public class NotificationInVideoSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn khoá học")]
        public int? VideoCourseId { get; set; }
    }
    public class CertificateSearch : SearchOptions
    {
        public int UserId { get; set; } = 0;
    }
    public class ExamResultSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn đề")]
        public int? ExamId { get; set; }
        public int StudentId { get; set; } = 0;
        public int VideoCourseId { get; set; } = 0;
    }
    public class ExerciseGroupSearch : SearchOptions
    {
        public int Id { get; set; } = 0;
        /// <summary>
        /// Kiểm tra tồn tại trong đề, truyền vào ExamId
        /// </summary>
        public int NotExistInExam { get; set; }
        public string Tags { get; set; }
        public ExerciseLevel? Level { get; set; }
        public ExerciseType? Type { get; set; }
    }
    public class ExamSearch : SearchOptions
    {
    }
    public class QuestionInVideoSearch : SearchOptions
    {
        [Required]
        public int VideoCourseId { get; set; }
    }
    public class SeminarSearch : SearchOptions
    {
        public string Name { get; set; }
        public SeminarStatus? Status { get; set; }
    }
    public class VideoCourseStudentSearch : SearchOptions
    {
        /// <summary>
        /// mẫu : code,be
        /// </summary>
        public string Stags { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
    }
    public class StudentInVideoCourseSearch : SearchOptions
    {
        [Required]
        public int VideoCourseId { get; set; }
        public string FullName { get; set; }
        /// <summary>
        /// yyyy/MM/dd
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// yyyy/MM/dd
        /// </summary>
        public DateTime? ToDate { get; set; }
    }
    public class ProductSearch : SearchOptions
    {
        /// <summary>
        /// 1 - Khóa video
        /// 2 - Bộ đề
        /// 3 - Lượt chấm bài
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        public int? Type { get; set; }
        /// <summary>
        /// mẫu : 1,2,3
        /// </summary>
        public string Tags { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// 2 - Giá
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
    }
    public class ChangeInfo : SearchOptions
    {
        public string UserCode { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string Statuss { get; set; }
    }
    public class UserSearch : SearchOptions
    {
        public override int PageSize { get; set; } = 999999;
        /// <summary>
        /// 0 - Ngày tạo
        /// 1 - Tên 
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; } = false;
        public string FullName { get; set; }
        public string UserCode { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string RoleIds { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string BranchIds { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string Genders { get; set; }
        /// <summary>
        /// id phụ huynh
        /// </summary>
        public string ParentIds { get; set; }
        /// <summary>
        /// id học sinh
        /// </summary>
        public string StudentIds { get; set; }
    }
    public class WardSearch : SearchOptions
    {
        public int? DistrictId { get; set; }
        public string Name { get; set; }
    }
    public class DistrictSearch : SearchOptions
    {
        public int? AreaId { get; set; }
        public string Name { get; set; }
    }
    public class AreaSearch : SearchOptions
    {
        public string Name { get; set; }
    }
    public class SearchOptions
    {
        public virtual int PageSize { get; set; } = 20;
        public virtual int PageIndex { get; set; } = 1;
        public virtual string Search { get; set; }
    }

}
