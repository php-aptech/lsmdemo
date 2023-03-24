namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_CustomerNote : DomainEntity
    { 
        public int? CustomerId { get; set; }
        public string Note { get; set; }
        public tbl_CustomerNote() : base() { }
        public tbl_CustomerNote(object model) : base(model) { }
    }
}