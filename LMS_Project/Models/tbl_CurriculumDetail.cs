namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_CurriculumDetail : DomainEntity
    {
        public int? CurriculumId { get; set; }
        public int? Index { get; set; }
        public string Name { get; set; }
        public tbl_CurriculumDetail() : base() { }
        public tbl_CurriculumDetail(object model) : base(model) { }
    }
}