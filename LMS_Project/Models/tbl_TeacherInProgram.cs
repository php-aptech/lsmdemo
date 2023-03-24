namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_TeacherInProgram : DomainEntity
    {
        public int? TeacherId { get; set; }
        public int? ProgramId { get; set; }
    }
}