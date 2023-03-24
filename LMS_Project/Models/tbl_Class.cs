namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_Class : DomainEntity
    {
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public int? BranchId { get; set; }
        public int? GradeId { get; set; }
        public int? ProgramId { get; set; }
        public int? CurriculumId { get; set; }
        public DateTime? StartDay { get; set; }
        public DateTime? EndDay { get; set; }
        public double Price { get; set; }
        /// <summary>
        /// 1 - Sắp diễn ra
        /// 2 - Đang diễn ra
        /// 3 - Kết thúc
        /// </summary>
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public int? AcademicId { get; set; }
        public int? TeacherId { get; set; }
        /// <summary>
        /// 1 - Offline
        /// 2 - Online
        /// 3 - Dạy kèm
        /// </summary>
        public int? Type { get; set; }
        public string TypeName { get; set; }
        /// <summary>
        /// Số lượng tối đa
        /// </summary>
        public int? MaxQuantity { get; set; }
        /// <summary>
        /// Số lượng học viên
        /// </summary>
        [NotMapped]
        public int? TotalStudent { get; set; }
        /// <summary>
        /// Tổng buổi học
        /// </summary>
        [NotMapped]
        public int? TotalLesson { get; set; }
        /// <summary>
        /// Số buổi hoàn thành
        /// </summary>
        [NotMapped]
        public int? LessonCompleted { get; set; }
        [NotMapped]
        public string ProgramName { get; set; }
        [NotMapped]
        public string GradeName { get; set; }
        [NotMapped]
        public string CurriculumName { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        public string TeacherName { get; set; }
        [NotMapped]
        public string AcademicName { get; set; }
        public tbl_Class() : base() { }
        public tbl_Class(object model) : base(model) { }
    }
    public class Get_Class : DomainEntity
    {
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public int? BranchId { get; set; }
        public int? GradeId { get; set; }
        public int? ProgramId { get; set; }
        public int? CurriculumId { get; set; }
        public DateTime? StartDay { get; set; }
        public DateTime? EndDay { get; set; }
        public double Price { get; set; }
        /// <summary>
        /// 1 - Sắp diễn ra
        /// 2 - Đang diễn ra
        /// 3 - Kết thúc
        /// </summary>
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public int? AcademicId { get; set; }
        public int? TeacherId { get; set; }
        /// <summary>
        /// 1 - Offline
        /// 2 - Online
        /// 3 - Dạy kèm
        /// </summary>
        public int? Type { get; set; }
        public string TypeName { get; set; }
        /// <summary>
        /// Lương trên buổi dạy
        /// </summary>
        public double? TeachingFee { get; set; }
        /// <summary>
        /// Số lượng tối đa
        /// </summary>
        public int? MaxQuantity { get; set; }
        /// <summary>
        /// Số lượng học viên
        /// </summary>
        [NotMapped]
        public int? TotalStudent { get; set; }
        /// <summary>
        /// Tổng buổi học
        /// </summary>
        [NotMapped]
        public int? TotalLesson { get; set; }
        /// <summary>
        /// Số buổi hoàn thành
        /// </summary>
        [NotMapped]
        public int? LessonCompleted { get; set; }
        public string ProgramName { get; set; }
        public string GradeName { get; set; }
        public string CurriculumName { get; set; }
        public string BranchName { get; set; }
        public string TeacherName { get; set; }
        public string AcademicName { get; set; }
        public int TotalRow { get; set; }
    }
}