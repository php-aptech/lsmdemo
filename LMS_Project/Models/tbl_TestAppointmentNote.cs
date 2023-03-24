namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_TestAppointmentNote : DomainEntity
    { 
        public int? TestAppointmentId { get; set; }
        public string Note { get; set; }
        public tbl_TestAppointmentNote() : base() { }
        public tbl_TestAppointmentNote(object model) : base(model) { }
    }
}