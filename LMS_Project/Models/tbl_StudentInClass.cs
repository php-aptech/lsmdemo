namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_StudentInClass : DomainEntity
    {
        public int? BranchId { get; set; }
        public int? ClassId { get; set; }
        public int? StudentId { get; set; }
        public bool? Warning { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 1 - Chính thức
        /// 2 - Học thử
        /// </summary>
        public int? Type { get; set; }
        public string TypeName { get; set; }
        [NotMapped]
        public string ClassName { get; set; }
        [NotMapped]
        public int? ClassType { get; set; }
        [NotMapped]
        public string ClassTypeName { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string UserCode { get; set; }
        [NotMapped]
        public string Avatar { get; set; }
        [NotMapped]
        public string Mobile { get; set; }
        [NotMapped]
        public string Email { get; set; }
        public tbl_StudentInClass() : base() { }
        public tbl_StudentInClass(object model) : base(model) { }
    }
    public class Get_StudentInClass : DomainEntity
    {
        public int? BranchId { get; set; }
        public int? ClassId { get; set; }
        public int? StudentId { get; set; }
        public bool? Warning { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 1 - Chính thức
        /// 2 - Học thử
        /// </summary>
        public int? Type { get; set; }
        public string TypeName { get; set; }
        public string ClassName { get; set; }
        public int? ClassType { get; set; }
        public string ClassTypeName { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string Avatar { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int TotalRow { get; set; }
    }
}