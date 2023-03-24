namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_TimeLine : DomainEntity
    {
        public int? ClassId { get; set; }
        public string Note { get;set; }
        public tbl_TimeLine() : base() { }
        public tbl_TimeLine(object model) : base(model) { }
    }
}