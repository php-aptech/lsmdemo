namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_Program : DomainEntity
    { 
        public int? GradeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public string GradeCode { get; set; }
        [NotMapped]
        public string GradeName { get; set; }
        public tbl_Program() : base() { }
        public tbl_Program(object model) : base(model) { }
    }
    public class Get_Program : DomainEntity
    {
        public int? GradeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string GradeCode { get; set; }
        public string GradeName { get; set; }
        public int TotalRow { get; set; }
    }
}