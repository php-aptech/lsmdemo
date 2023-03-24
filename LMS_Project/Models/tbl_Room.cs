namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_Room : DomainEntity
    {
        public int? BranchId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public tbl_Room() : base() { }
        public tbl_Room(object model) : base(model) { }
    }
}