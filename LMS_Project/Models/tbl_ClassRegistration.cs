namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_ClassRegistration : DomainEntity
    {
        public int? StudentId { get; set; }
        public int? BranchId { get; set; }
        public int? ProgramId { get; set; }
        public double? Price { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 1 - Chờ xếp lớp
        /// 2 - Đã xếp lớp
        /// 3 - Đã hoàn tiền
        /// </summary>
        public int? Status { get; set; }
        public string StatusName { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string UserCode { get; set; }
        [NotMapped]
        public string Avatar { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        public string ProgramName { get; set; }
        public tbl_ClassRegistration() : base() { }
        public tbl_ClassRegistration(object model) : base(model) { }
    }
    public class Get_ClassRegistration : DomainEntity
    {
        public int? StudentId { get; set; }
        public int? BranchId { get; set; }
        public int? ProgramId { get; set; }
        public double? Price { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 1 - Chờ xếp lớp
        /// 2 - Đã xếp lớp
        /// 3 - Đã hoàn tiền
        /// </summary>
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string Avatar { get; set; }
        public string BranchName { get; set; }
        public string ProgramName { get; set; }
        public int TotalRow { get; set; }
    }
}