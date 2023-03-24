namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_DayOff : DomainEntity
    {
        public string Name { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
        public tbl_DayOff() : base() { }
        public tbl_DayOff(object model) : base(model) { }
    }
}