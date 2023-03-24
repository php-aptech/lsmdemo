namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_Source : DomainEntity
    {
        public string Name { get; set; }
        public tbl_Source() : base() { }
        public tbl_Source(object model) : base(model) { }
    }
}