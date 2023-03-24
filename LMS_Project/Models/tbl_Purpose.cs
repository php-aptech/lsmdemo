namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_Purpose : DomainEntity
    {
        public string Name { get; set; }
        public tbl_Purpose() : base() { }
        public tbl_Purpose(object model) : base(model) { }
    }
}