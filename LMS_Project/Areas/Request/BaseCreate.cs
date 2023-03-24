using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using LMS_Project.LMS;
using static LMS_Project.Models.lmsEnum;
using System.IO;

namespace LMS_Project.Areas.Request
{
    public class BaseCreate
    {
        [JsonIgnore]
        public bool Enable { get; set; } = true;
        [JsonIgnore]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
    }
    public class SalaryCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn năm")]
        public int Year { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn tháng")]
        public int Month { get; set; }
        /// <summary>
        /// Lương cơ bản
        /// </summary>
        public double BasicSalary { get; set; }
        /// <summary>
        /// Lương dạy học
        /// </summary>
        public double TeachingSalary { get; set; }
        /// <summary>
        /// Khấu trừ
        /// </summary>
        public double Deduction { get; set; }
        /// <summary>
        /// Thưởng thêm
        /// </summary>
        public double Bonus { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        /// <summary>
        /// Tổng lương
        /// </summary>
        [JsonIgnore]
        public double TotalSalary { get 
            {
                return (BasicSalary + TeachingSalary + Bonus) - Deduction;
            } 
        }
        /// <summary>
        /// 1 - Chưa chốt
        /// 2 - Đã chốt
        /// 3 - Đã thanh toán
        /// </summary>
        [JsonIgnore]
        public int Status { get { return 1; } }
        [JsonIgnore]
        public string StatusName { get { return "Chưa chốt"; } }
    }
    public class CurriculumDetailInClassCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn tài liệu trong lớp")]
        public int? CurriculumIdInClass { get; set; }
        [JsonIgnore]
        public int CurriculumDetailId { get { return 0; } }
        public string Name { get; set; }
        [JsonIgnore]
        public bool? IsComplete { get { return false; } }
        [JsonIgnore]
        public double? CompletePercent { get { return 0; } }
    }
    public class TutoringFeeCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn giáo viên")]
        public int? TeacherId { get; set; }
        /// <summary>
        /// phí dạy kèm
        /// </summary>
        public double? Fee { get; set; }
        public string Note { get; set; }
    }
    public class StudentAssessmentCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn buổi học")]
        public int ScheduleId { get; set; }
        public string Listening { get; set; }
        public string Speaking { get; set; }
        public string Reading { get; set; }
        public string Writing { get; set; }
        public string Note { get; set; }
    }
    public class CurriculumInClassCreate : BaseCreate
    {
        public int? CurriculumId { get; set; }
        public int? ClassId { get; set; }
    }
    public class DocumentLibraryCreate : BaseCreate
    {
        /// <summary>
        /// Id chủ đề
        /// </summary>
        [Required(ErrorMessage ="Vui lòng chọn chủ đề tài liệu")]
        public int? DirectoryId { get; set; }
        public string Background { get; set; }
        /// <summary>
        /// Link file
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn file")]
        public string FileUrl { get; set; }
        public string Description { get; set; }
    }
    public class DocumentLibraryDirectoryCreate : BaseCreate
    {
        /// <summary>
        /// Tên chủ đề
        /// </summary>
        [Required(ErrorMessage =("Vui lòng nhập tên chủ đề"))]
        public string Name { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }
    }
    public class FeedbackReplyCreate : BaseCreate
    {
        [Required(ErrorMessage =("Vui lòng chọn phản hồi"))]
        public int? FeedbackId { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Content { get; set; }
    }
    public class FeedbackCreate : BaseCreate
    {
        [Required(ErrorMessage ="Vui lòng nhập tiêu đề")]
        public string Title { get; set; }
        /// <summary>
        /// Nội dung phản hồi
        /// </summary>
        [Required(ErrorMessage ="Vui lòng nhập nội dung")]
        public string Content { get; set; }
        /// <summary>
        /// Cờ ưu tiên
        /// </summary>
        public bool? IsPriority { get; set; }
        /// <summary>
        /// Cờ ẩn danh
        /// </summary>
        public bool? IsIncognito { get; set; }
    }
    public class ContractCreate : BaseCreate
    {
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public int? StudentId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung hợp đồng")]
        public string Content { get; set; }
    }
    public class NewsFeedReplyCreate : BaseCreate
    {
        [Required(ErrorMessage ="Vui lòng chọn bình luận")]
        public int? NewsFeedCommentId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Content { get; set; }
    }
    public class NewsFeedCommentCreate : BaseCreate
    {
        [Required(ErrorMessage ="Vui lòng chọn bản tin")]
        public int? NewsFeedId { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập nội dung")]
        public string Content { get; set; }
    }
    public class NewsFeedLikeCreate : BaseCreate
    {
        /// <summary>
        /// id bản tin
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn bản tin")]
        public int? NewsFeedId { get; set; }
    }
    public class ScheduleAvailableCreate : BaseCreate
    {
        public int? TeacherId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
        public DateTime? StartTime { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
        public DateTime? EndTime { get; set; }
        public string Note { get; set; }
    }
    public class VideoCourseStudentCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public int? UserId { get; set; }
        [Required(ErrorMessage ="Vui lòng chọn khóa học")]
        public int? VideoCourseId { get; set; }
    }
    public class PackageStudentCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn bộ đề")]
        public int? PackageId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public int? StudentId { get; set; }
    }
    public class MarkSalaryCreate : BaseCreate
    { 
        [Required(ErrorMessage = "Vui lòng chọn giáo viên")]
        public int? TeacherId { get; set; }
        /// <summary>
        /// Lương chấm bài
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập lương chấm bài")]
        public double? Salary { get; set; }
    }
    public class PaymentAllowCreates
    {
        [Required(ErrorMessage ="Vui lòng chọn học viên")]
        public List<int> UserIds { get; set; }
    }
    public class PackageSkillCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn phần")]
        public int PackageSectionId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên kỹ năng")]
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        /// <summary>
        /// đề thi
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn đề thi")]
        public int ExamId { get; set; }
    }
    public class PackageSectionCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn bộ đề")]
        public int PackageId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Số lượng đề
        /// </summary>
        [JsonIgnore]
        public int ExamQuatity { get { return 0; } }
    }


    public class NewsFeedCreate : BaseCreate
    {
        /// <summary>
        /// Nội dung bản tin
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Màu bản tin
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// Nền bản tin
        /// </summary>
        public string BackGroundUrl { get; set; }
        /// <summary>
        /// Nhóm
        /// </summary>
        public int? NewsFeedGroupId { get; set; }
        /// <summary>
        /// Danh sách chi nhánh
        /// </summary>
        public List<int> ListBranchId { get; set; }

        /// <summary>
        /// File => tbl_NewsFeedFile
        /// </summary>
        public List<NewsFeedFile> FileListCreate { get; set; }
    }

    public class NewsFeedFile
    {
        /// <summary>
        /// Đường dẫn file
        /// </summary>
        public string FileUrl { get; set; }       
    }

    public class UserInNFGroupCreate : BaseCreate
    {
        [Required(ErrorMessage ="Vui lòng chọn nhóm")]
        public int? NewsFeedGroupId { get; set; }
        public List<Members> Members { get; set; }
    }

    public class Members
    {
        /// <summary>
        /// id user
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn người dùng")]
        public int? UserId { get; set; }

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




    public class NewsFeedGroupCreate : BaseCreate
    {
        /// <summary>
        /// Tên nhóm
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập tên nhóm")]
        public string Name { get; set; }
        /// <summary>
        /// Hình nền
        /// </summary>
        public string BackGround { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        public int? ClassId { get; set; }
    }
    public class CartCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public int Quantity { get; set; }
    }
    public class TagCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int? TagCategoryId { get; set; }
    }
    public class TagCategoryCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
        /// <summary>
        /// 1 - Khóa Video
        /// 2 - Câu hỏi
        /// 3 - Bộ đề
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập loại")]
        public int Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == 1 ? "Khóa video" 
                    : Type == 2 ? "Câu hỏi"
                    : Type == 3 ? "Bộ đề" : "";
            }
        }
    }
    public class SalaryConfigCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập lương")]
        public double? Value { get; set; }
        public string Note { get; set; }
    }
    public class RefundCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn chi nhánh")]
        public int BranchId { get; set; }
        public double Price { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn hình thức thanh toán")]
        public int? PaymentMethodId { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        /// <summary>
        /// 1 - Hoàn tiền thủ công
        /// 2 - Hoàn tiền bảo lưu
        /// 3 - Hoàn tiền chờ xếp lớp
        /// 4 - Hoàn tiền thanh toán dư
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại hoàn tiền")]
        public int? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == 1 ? "Hoàn tiền thủ công"
                        : Type == 2 ? "Hoàn tiền bảo lưu"
                        : Type == 3 ? "Hoàn tiền chờ xếp lớp"
                        : Type == 4 ? "Hoàn tiền thanh toán dư" : "";
            }
        }
        /// <summary>
        /// 1 - Chờ duyệt
        /// 2 - Đã duyệt
        /// 3 - Hủy
        /// </summary>
        [JsonIgnore]
        public int? Status { get { return 1; } }
        [JsonIgnore]
        public string StatusName { get { return "Chờ duyệt"; } }
        /// <summary>
        /// truyền khi type 1
        /// </summary>
        public int StudentId { get; set; } = 0;
        /// <summary>
        /// truyền khi type 3
        /// </summary>
        public int ClassRegistrationId { get; set; } = 0;
        /// <summary>
        /// truyền khi type 2
        /// </summary>
        public int ClassReserveId { get; set; } = 0;
        /// <summary>
        /// truyền khi type 4
        /// </summary>
        public int BillId { get; set; } = 0;
    }
    public class ClassReserveCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn học viên trong lớp")]
        public int? StudentInClassId { get; set; }
        /// <summary>
        /// Ngày hết hạn bảo lưu
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn hạn bảo lưu")]
        public DateTime? Expires { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 1 - Đang bảo lưu
        /// 2 - Đã học lại
        /// 3 - Đã hoàn tiền
        /// 4 - Hết hạn bảo lưu
        /// </summary>
        [JsonIgnore]
        public int? Status { get { return 1; } }
        [JsonIgnore]
        public string StatusName { get { return "Đang bảo lưu"; } }
    }
    public class ClassChangeCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn học viên trong lớp")]
        public int? StudentInClassId { get; set; }
        /// <summary>
        /// Lớp mới
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn lớp mới")]
        public int? NewClassId { get; set; }
        public string Note { get; set; }
    }
    public class NotificationInClassCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn lớp học")]
        public int? ClassId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsSendMail { get; set; }
    }
    public class TimeLineCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn lớp học")]
        public int? ClassId { get; set; }
        public string Note { get; set; }
    }
    public class RollUpCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public int? StudentId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn buổi học")]
        public int? ScheduleId { get; set; }
        /// <summary>
        ///    co = 1,
        ///    vang_co_phep = 2,
        ///    vang_khong_phep = 3,
        ///    di_muon = 4,
        ///    ve_som = 5,
        ///    nghi_le = 6
        /// </summary>
        public int? Status { get; set; }
        [JsonIgnore]
        public string StatusName
        {
            get
            {
                string result = "";
                switch (Status)
                {
                    case 1: result = "Có"; break;
                    case 2: result = "Văng có phép"; break;
                    case 3: result = "Văng không phép"; break;
                    case 4: result = "Đi muộn"; break;
                    case 5: result = "Về sớm"; break;
                    case 6: result = "Nghỉ lễ"; break;
                }
                return result;
            }
        }
        /// <summary>
        ///    gioi = 1,
        ///    kha = 2,
        ///    trung_binh = 3,
        ///    kem = 4,
        ///    theo_doi_dac_biet = 5,
        ///    co_co_gang = 6,
        ///    khong_co_gang = 7,
        ///    khong_nhan_xet = 8
        /// </summary>
        public int? LearningStatus { get; set; }
        [JsonIgnore]
        public string LearningStatusName
        {
            get
            {
                string result = "";
                switch (LearningStatus)
                {
                    case 1: result = "Giỏi"; break;
                    case 2: result = "Khá"; break;
                    case 3: result = "Trung bình"; break;
                    case 4: result = "Kém"; break;
                    case 5: result = "Theo dỏi đặc biệt"; break;
                    case 6: result = "Có cố gắng"; break;
                    case 7: result = "Không cố gắng"; break;
                    case 8: result = "Không nhận xét"; break;
                }
                return result;
            }
        }
        public string Note { get; set; }
    }
    public class StudentInClassCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        public int? ClassId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public int? StudentId { get; set; }
        [JsonIgnore]
        public bool? Warning { get { return false; } }
        public string Note { get; set; }
        /// <summary>
        /// 1 - Chính thức
        /// 2 - Học thử
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại")]
        public int? Type { get; set; }
        public string TypeName { get
            {
                return Type == 1 ? "Chính thức"
                        : Type == 2 ? "Học thử"
                        : null;
            }
        }
    }
    public class PaymentSessionCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn trung tâm")]
        public int? BranchId { get; set; }
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        public double Value { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public int? PaymentMethodId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập lý do tạo phiếu")]
        [StringLength(1000)]
        public string Reason { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        /// <summary>
        /// 1 - Thu
        /// 2 - Chi
        /// </summary>
        public int? Type { get; set; }
        [StringLength(20)]
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == 1 ? "Thu" : Type == 2 ? "Chi" : "";
            }
        }
    }
    public class BillCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public int? StudentId { get; set; }
        public int? DiscountId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public int? PaymentMethodId { get; set; }
        /// <summary>
        /// Ngày hẹn thanh toán
        /// </summary>
        public DateTime? PaymentAppointmentDate { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn trung tâm")]
        public int? BranchId { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 1 - Đăng ký học 
        /// 2 - Mua dịch vụ
        /// 3 - Đăng ký lớp dạy kèm
        /// 4 - Tạo thủ công
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại")]
        public int Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == 1 ? "Đăng ký học"
                        : Type == 2 ? "Mua dịch vụ"
                        : Type == 3 ? "Đăng ký lớp dạy kèm"
                        : Type == 4 ? "Tạo thủ công" : "";
            }
        }
        /// <summary>
        /// Đã trả
        /// </summary>
        public double Paid { get; set; }
        /// <summary>
        /// Giá - truyền vào khi tạo thanh toán thủ công
        /// </summary>
        public double Price { get; set; }
        public List<BillDetailCreate> Details { get; set; }
    }
    public class BillDetailCreate : BaseCreate
    {
        /// <summary>
        /// Lớp
        /// </summary>
        public int? ClassId { get; set; }
        /// <summary>
        /// Chương trình học
        /// </summary>
        public int? ProgramId { get; set; }
        /// <summary>
        /// Giáo trình - chọn đối với lớp dạy kèm
        /// </summary>
        public int? CurriculumId { get; set; }
        /// <summary>
        /// Giỏ hàng
        /// </summary>
        public int? CartId { get; set; }
    }
    public class ScheduleCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        public int? ClassId { get; set; }
        public int RoomId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
        public DateTime? StartTime { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
        public DateTime? EndTime { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn giáo viên")]
        public int? TeacherId { get; set; }
        public string Note { get; set; }
        public double TeachingFee { get; set; }
    }
    public class TeacherOffCreate : BaseCreate
    {
        /// <summary>
        /// thời gian bắt đầu
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// thời gian kết thúc
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// lý do nghỉ
        /// </summary>
        [StringLength(1000)]
        [Required(ErrorMessage = "Vui lòng nhập lý do nghỉ")]
        public string Reason { get; set; }
        /// <summary>
        /// 1 - Chờ duyệt
        /// 2 - Duyệt
        /// 3 - Không duyệt
        /// </summary>
        [JsonIgnore]
        public int? Status { get { return 1; } }
        [StringLength(20)]
        [JsonIgnore]
        public string StatusName { get { return "Chờ duyệt"; } }
        /// <summary>
        /// ghi chú
        /// </summary>
        [StringLength(1000)]
        [JsonIgnore]
        public string Note { get { return ""; } }
    }
    public class ClassCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên lớp học")]
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn trung tâm")]
        public int? BranchId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn chuyên ngành")]
        public int? GradeId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn chương trình")]
        public int? ProgramId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn giáo trình")]
        public int? CurriculumId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
        public DateTime? StartDay { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá lớp học")]
        public double? Price { get; set; }
        /// <summary>
        /// 1 - Sắp diễn ra
        /// 2 - Đang diễn ra
        /// 3 - Kết thúc
        /// </summary>
        [JsonIgnore]
        public int? Status { get { return 1; } }
        [JsonIgnore]
        public string StatusName { get { return "Sắp diễn ra"; } }
        /// <summary>
        /// Hoc vụ
        /// </summary>
        public int? AcademicId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn giáo viên")]
        public int? TeacherId { get; set; }
        /// <summary>
        /// 1 - Offline
        /// 2 - Online
        /// 3 - Dạy kèm
        /// </summary>
        public int? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == 1 ? "Offline"
                        : Type == 2 ? "Online"
                        : Type == 3 ? "Dạy kèm" : "";
            }
        }
        /// <summary>
        /// Lương trên buổi dạy
        /// </summary>
        public double TeachingFee { get; set; } = 0;
        /// <summary>
        /// Số lượng tối đa
        /// </summary>
        public int MaxQuantity { get; set; } = 20;
        public List<ScheduleCreates> schedules { get; set; }
    }
    public class ScheduleCreates
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomCode { get; set; }
        public int? TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string TeacherCode { get; set; }
        public string Note { get; set; }
    }
    public class StudyTimeCreate : BaseCreate
    {
        /// <summary>
        /// định dạng HH:mm
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
        public string StartTime { get; set; }
        /// <summary>
        /// định dạng HH:mm
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
        public string EndTime { get; set; }
    }
    public class FileInCurriculumDetailCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int? CurriculumDetailId { get; set; }
        [JsonIgnore]
        public string FileName { get; set; }
        [JsonIgnore]
        public string FileUrl { get; set; }
    }
    public class CurriculumDetailCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn giáo trình")]
        public int? CurriculumId { get; set; }
        public string Name { get; set; }
    }
    public class CurriculumCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn chương trình học")]
        public int? ProgramId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên giáo trình")]
        public string Name { get; set; }
        /// <summary>
        /// Số buổi
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập số buổi học")]
        public int Lesson { get; set; } = 0;
        /// <summary>
        /// Thời gian
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập thời gian học")]
        public int Time { get; set; } = 0;
    }
    public class ProgramCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn chuyên môn")]
        public int? GradeId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
    }
    public class GradeCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập mã")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
    }
    public class TestAppointmentNoteCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn lịch hẹn")]
        public int? TestAppointmentId { get; set; }
        public string Note { get; set; }
    }
    public class TestAppointmentCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn trung tâm")]
        public int? BranchId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public int? StudentId { get; set; }
        public DateTime? Time { get; set; }
        /// <summary>
        /// 1 - Chưa kiểm tra
        /// 2 - Đã kiểm tra 
        /// </summary>
        [JsonIgnore]
        public int? Status { get { return 1; } }
        [JsonIgnore]
        public string StatusName { get { return "Chưa kiểm tra"; } }
        public int? TeacherId { get; set; }
        /// <summary>
        /// 1 - Tại trung tâm
        /// 2 - Làm bài trực tuyến
        /// </summary>
        public int? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == 1 ? "Tại trung tâm"
                    : Type == 2 ? "Làm bài trực tuyến" : "";
            }
        }
        public int? ExamId { get; set; }
    }
    public class CustomerCreate : BaseCreate
    {
        public int? LearningNeedId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
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
        [Required(ErrorMessage = "Vui lòng chọn trung tâm")]
        public int? BranchId { get; set; }
        public int? JobId { get; set; }
    }
    public class CustomerStatusCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
    }
    public class GeneralNotificationCreate : BaseCreate
    {
        public string Title { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Danh sách người nhận thông báo
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn người nhận")]
        public string UserIds { get; set; }
        public bool IsSendMail { get; set; }
    }
    public class FrequentlyQuestionCreate : BaseCreate
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
    public class IdiomCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Content { get; set; }
    }
    public class PurposeCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
    }
    public class JobCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
    }
    public class DayOffCreate : BaseCreate
    {
        public string Name { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
    }
    public class SourceCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
    }
    public class LearningNeedCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
    }
    public class DiscountCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập mã")]
        public string Code { get; set; }
        /// <summary>
        /// 1 - Giảm tiền 
        /// 2 - Giảm phần trăm
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại")]
        public int? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == 1 ? "Giảm tiền"
                    : Type == 2 ? "Giảm phần trăm" : "";
            }
        }
        /// <summary>
        /// 1 - Gói lẻ
        /// 2 - Gói combo
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn gói")]
        public int? PackageType { get; set; }
        [JsonIgnore]
        public string PackageTypeName
        {
            get
            {
                return PackageType == 1 ? "Gói lẻ"
                    : PackageType == 2 ? "Gói combo" : "";
            }
        }
        [Required(ErrorMessage = "Vui lòng nhập giá trị khuyến mãi")]
        public double Value { get; set; }
        /// <summary>
        /// 1 - Đang khả dụng
        /// 2 - Đã kết thúc
        /// </summary>
        [JsonIgnore]
        public int? Status { get { return 1; } }
        [JsonIgnore]
        public string StatusName { get { return "Đang khả dụng"; } }
        public string Note { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày hết hạn")]
        public DateTime? Expired { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn số lượng")]
        public int? Quantity { get; set; }
        /// <summary>
        /// Số lượng đã dùng
        /// </summary>
        [JsonIgnore]
        public int? UsedQuantity { get { return 0; } }
        /// <summary>
        /// Khuyến mãi tối đa
        /// </summary>
        public double MaxDiscount { get; set; } = 0;
    }
    public class RoomCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn trung tâm")]
        public int? BranchId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
    }
    public class CustomerNoteCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public int? CustomerId { get; set; }
        public string Note { get; set; }
    }
    public class BranchCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập mã")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
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
    public class SeminarRecordCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn hội thảo")]
        public int? SeminarId { get; set; }
        public string Name { get; set; }
        public string VideoUrl { get; set; }
    }
    public class NotificationInVideoCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn khoá học")]
        public int? VideoCourseId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Content { get; set; }
        [JsonIgnore]
        public bool IsSend { get { return false; } }
    }
    public class ExerciseGroupInExamCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn phần trong đề thi")]
        public int? ExamSectionId { get; set; }
        public string Content { get; set; }
        public string Paragraph { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cấp độ")]
        public ExerciseLevel? Level { get; set; }
        [JsonIgnore]
        public string LevelName
        {
            get { return Level == ExerciseLevel.Easy ? "Dễ" : Level == ExerciseLevel.Normal ? "Trung bình" : Level == ExerciseLevel.Difficult ? "Khó" : ""; }
        }
        [Required(ErrorMessage = "Vui lòng chọn loại")]
        public ExerciseType? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == ExerciseType.MultipleChoice ? "Trắc nghiệp"
                    : Type == ExerciseType.DragDrop ? "Kéo thả"
                    : Type == ExerciseType.FillInTheBlank ? "Điền vào ô trống" : "";
                //: Type == ExerciseType.Essay ? "Tự luận" : "";
            }
        }
        public List<ExerciseInExamCreate> ExerciseInExamCreates { get; set; }
    }
    public class ExerciseInExamCreate : BaseCreate
    {
        public string Content { get; set; }
        /// <summary>
        /// Áp dụng cho câu kéo thả và điền từ
        /// </summary>
        public string InputId { get; set; }
        public List<AnswerInExamCreate> AnswerInExamCreates { get; set; }
        public string DescribeAnswer { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập điểm")]
        public double? Point { get; set; }
    }
    public class AnswerInExamCreate : BaseCreate
    {
        public string AnswerContent { get; set; }
        public bool? IsTrue { get; set; }
    }
    public class ExamSectionCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn đề")]
        public int? ExamId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Nội dung chú thích đáp án đúng của đoạn văn
        /// </summary>
        public string Explanations { get; set; }
    }
    public class ExerciseGroupCreate : BaseCreate
    {

        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn phần trong đề thi")]
        public int? ExamSectionId { get; set; }
        public string Content { get; set; }
        public string Paragraph { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cấp độ")]
        public ExerciseLevel? Level { get; set; }
        [JsonIgnore]
        public string LevelName
        {
            get { return Level == ExerciseLevel.Easy ? "Dễ" : Level == ExerciseLevel.Normal ? "Trung bình" : Level == ExerciseLevel.Difficult ? "Khó" : ""; }
        }
        [Required(ErrorMessage = "Vui lòng chọn loại")]
        public ExerciseType? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == ExerciseType.MultipleChoice ? "Trắc nghiệm"
                    : Type == ExerciseType.DragDrop ? "Kéo thả"
                    : Type == ExerciseType.FillInTheBlank ? "Điền vào ô trống" 
                    : Type == ExerciseType.Mindmap ? "Ghép đáp án" 
                    : Type == ExerciseType.Sort ? "Sắp xếp câu" 
                    : Type == ExerciseType.TrueOrFalse ? "Chọn đúng/sai" 
                    : Type == ExerciseType.Write ? "Viết" 
                    : Type == ExerciseType.Speak ? "Nói" : "";
            }
        }
        public string Tags { get; set; }
        public List<ExerciseCreate> ExerciseCreates { get; set; }
    }
    public class ExerciseCreate : BaseCreate
    {
        public string Content { get; set; }
        /// <summary>
        /// Áp dụng cho câu kéo thả và điền từ
        /// </summary>
        public string InputId { get; set; }
        public List<AnswerCreate> AnswerCreates { get; set; }
        public string DescribeAnswer { get; set; }
        public double? Point { get; set; }
    }
    public class AnswerCreate : BaseCreate
    {
        public string AnswerContent { get; set; }
        public bool? IsTrue { get; set; }
        /// <summary>
        /// 1 - Chữ 
        /// 2 - Hình ảnh
        /// 3 - Tệp âm thanh
        /// </summary>
        public AnswerType Type { get; set; }
        public int Index { get; set; }
    }

    public class ExamCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đề")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã đề")]
        public string Code { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public int NumberExercise { get { return 0; } }
        /// <summary>
        /// Thời gian làm bài
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập thời gian làm bài")]
        public int Time { get; set; }
        [JsonIgnore]
        public int? DifficultExercise { get { return 0; } }
        [JsonIgnore]
        public int? NormalExercise { get { return 0; } }
        [JsonIgnore]
        public int? EasyExercise { get { return 0; } }
        /// <summary>
        /// Số điểm đạt
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập số điểm đạt")]
        public double PassPoint { get; set; }
    }
    public class ZoomConfigCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập AccountId")]
        public string AccountId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ClientId")]
        public string ClientId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ClientSecret")]
        public string ClientSecret { get; set; }
        [JsonIgnore]
        public bool Active { get { return false; } }
    }
    public class SeminarCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên buổi hội thảo")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
        public DateTime? StartTime { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
        public DateTime? EndTime { get; set; }
        public int VideoCourseId { get; set; } = 0;
        public int LeaderId { get; set; }
        [JsonIgnore]
        public int Status { get { return 1; } }
        [JsonIgnore]
        public string StatusName { get { return "Chưa diễn ra"; } }
        /// <summary>
        /// Số lượng người tham gia, bỏ trống là không giới hạn
        /// </summary>
        public int Member { get; set; } = 0;
        public string Thumbnail { get; set; }
    }
    public class CertificateProgramCreate : BaseCreate
    {
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public int? UserId { get; set; }
        public string Content { get; set; }
    }
    public class CertificateCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn khoá học")]
        public int? VideoCourseId { get; set; }
        public string Content { get; set; }
    }
    public class AnswerInVideoCreate : BaseCreate
    {
        [Required(ErrorMessage = "Không tìm thấy câu hỏi")]
        public int? QuestionInVideoId { get; set; }
        public string Content { get; set; }
    }
    public class QuestionInVideoCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn khoá học")]
        public int? VideoCourseId { get; set; }
        public string Content { get; set; }
    }
    public class FileInVideoCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn bài học")]
        public int? LessonVideoId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }
    public class LessonVideoCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn phần")]
        public int? SectionId { get; set; }
        /// <summary>
        /// 1 - Video
        /// 2 - Bài tập
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại bài học")]
        public int? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get { return Type == ((int)LessonType.Video) ? "Video" : Type == ((int)LessonType.BaiTap) ? "Bài tập" : ""; }
        }
        public string Name { get; set; }
        /// <summary>
        /// File video
        /// </summary>
        public string VideoUrl { get; set; }
        /// <summary>
        /// Bài tập
        /// </summary>
        public int? ExamId { get; set; }
        public LessonFileType? FileType { get; set; }
    }
    public class SectionCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn khoá học")]
        public int? VideoCourseId { get; set; }
        public string Name { get; set; }
    }
    public class ProductCreate : BaseCreate
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        /// <summary>
        /// Mẫu 1,2,3
        /// </summary>
        public string Tags { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Điều kiện đã học khoá này
        /// </summary>
        public int BeforeCourseId { get; set; } = 0;
        /// <summary>
        /// Giá bán
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        public double? Price { get; set; }
        /// <summary>
        /// 1 - Khóa video
        /// 2 - Bộ đề
        /// 3 - Lượt chấm bài
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        public int? Type { get; set; }
        /// <summary>
        /// Dùng cho lượt chấm
        /// </summary>
        public int MarkQuantity { get; set; }
    }
    public class RegisterModel : BaseCreate
    {
        [Required(ErrorMessage = "Fullname is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Mobile { get; set; }
        [JsonIgnore]
        public int? StatusId
        {
            get { return ((int)AccountStatus.active); }
        }
        [JsonIgnore]
        public int RoleId { get { return ((int)RoleEnum.student); } }
        [JsonIgnore]
        public string RoleName { get { return "Học viên"; } }
        public string Password { get; set; }
        /// <summary>
        /// 1 - Mới
        /// 2 - Đang học
        /// 3 - Học xong
        /// </summary>
        [JsonIgnore]
        public int LearningStatus { get { return 1; } }
        [JsonIgnore]
        public string LearningStatusName { get { return "Mới"; } }
    }
    public class UserCreate : BaseCreate
    {
        [Required(ErrorMessage = "Fullname is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        public DateTime? DOB { get; set; }
        /// <summary>
        /// 0 - Khác
        /// 1 - Nam
        /// 2 - Nữ
        /// </summary>
        public int Gender { get; set; } = ((int)GenderEnum.khac);
        /// <summary>
        /// Số điện thoại
        /// </summary>
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Mobile { get; set; }
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        public string Address { get; set; }
        [JsonIgnore]
        public int? StatusId
        {
            get { return ((int)AccountStatus.active); }
        }
        [Required(ErrorMessage = "Vui lòng chọn quyền")]
        public int RoleId { get; set; }
        [JsonIgnore]
        public string RoleName
        {
            get
            {
                return RoleId == ((int)RoleEnum.admin) ? "Admin"
                  : RoleId == ((int)RoleEnum.teacher) ? "Giáo viên"
                  : RoleId == ((int)RoleEnum.student) ? "Học viên"
                  : RoleId == ((int)RoleEnum.manager) ? "Quản lý"
                  : RoleId == ((int)RoleEnum.sale) ? "Tư vấn viên"
                  : RoleId == ((int)RoleEnum.accountant) ? "Kế toán"
                  : RoleId == ((int)RoleEnum.academic) ? "Học vụ"
                  : RoleId == ((int)RoleEnum.parents) ? "Phụ huynh"
                  : "";
            }
        }
        public string Avatar { get; set; }
        public int? AreaId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập khẩu")]
        public string Password { get; set; }
        /// <summary>
        /// Trung tâm
        /// Mẫu 1,2
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn trung tâm")]
        public string BranchIds { get; set; }
        /// <summary>
        /// 1 - Mới
        /// 2 - Đang học
        /// 3 - Học xong
        /// </summary>
        [JsonIgnore]
        public int LearningStatus { get { return 1; } }
        [JsonIgnore]
        public string LearningStatusName { get { return "Mới"; } }
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
        public int? CustomerId { get; set; }
        public int? ParentId { get; set; }
        /// <summary>
        /// Giới thiệu thêm
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Chương trình dạy
        /// </summary>
        public string ProgramIds { get; set; }
    }
    public class StudentRollUpQrCodeCreate : BaseCreate
    {
        /// <summary>
        /// học viên
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public int? StudentId { get; set; }
        /// <summary>
        /// buổi học
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn buổi học")]
        public int? ScheduleId { get; set; }
    }
    public class StudyRouteCreate : BaseCreate
    {
        /// <summary>
        /// học viên
        /// </summary>
        [Required(ErrorMessage = "Họp viên không được bỏ trống")]
        public int? StudentId { get; set; }
        /// <summary>
        /// chương trình
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn chương trình")]
        public int? ProgramId { get; set; }
        /// <summary>
        /// ghi chú
        /// </summary>
        public string Note { get; set; }
    }
    public class CertificateTemplateCreate : BaseCreate
    {
        [Required(ErrorMessage = "Tên chứng chỉ không được bỏ trống")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Nội dung không được bỏ trống")]
        public string Content { get; set; }
    }
}