namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_ClassReserve : DomainEntity
    {
        public int? StudentId { get; set; }
        /// <summary>
        /// Lớp cũ
        /// </summary>
        public int? ClassId { get; set; }
        public double? Price { get; set; }
        public int? BranchId { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 1 - Đang bảo lưu
        /// 2 - Đã học lại
        /// 3 - Đã hoàn tiền
        /// 4 - Hết hạn bảo lưu
        /// </summary>
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public int? StudentInClassId { get; set; }
        /// <summary>
        /// Ngày hết hạn bảo lưu
        /// </summary>
        public DateTime? Expires { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string UserCode { get; set; }
        [NotMapped]
        public string Avatar { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        public tbl_ClassReserve() : base() { }
        public tbl_ClassReserve(object model) : base(model) { }
    }
    public class Get_ClassReserve : DomainEntity
    {

        public int? StudentId { get; set; }
        /// <summary>
        /// Lớp cũ
        /// </summary>
        public int? ClassId { get; set; }
        public double? Price { get; set; }
        public int? BranchId { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 1 - Đang bảo lưu
        /// 2 - Đã học lại
        /// 3 - Đã hoàn tiền
        /// 4 - Hết hạn bảo lưu
        /// </summary>
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public int? StudentInClassId { get; set; }
        /// <summary>
        /// Ngày hết hạn bảo lưu
        /// </summary>
        public DateTime? Expires { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string Avatar { get; set; }
        public string BranchName { get; set; }
        public int TotalRow { get; set; }
    }
}