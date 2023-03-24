namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_Grade : DomainEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public tbl_Grade() : base() { }
        public tbl_Grade(object model) : base(model) { }
    }
}