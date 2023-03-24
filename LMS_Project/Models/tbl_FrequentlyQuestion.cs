namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_FrequentlyQuestion : DomainEntity
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public tbl_FrequentlyQuestion() : base() { }
        public tbl_FrequentlyQuestion(object model) : base(model) { }
    }
}