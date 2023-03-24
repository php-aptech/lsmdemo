using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Models
{
    public class tbl_StudyRoute : DomainEntity
    {
        public int StudentId { get; set; }
        public int ProgramId { get; set; }
        public string Note { get; set; }
        public int Index { get; set; }
        public int Status { get; set; }
        
        public string StatusName { get; set; }
        public tbl_StudyRoute() : base()
        {
        }
        public tbl_StudyRoute(object model) : base(model) { }

    }
    public class Get_StudyRoute : DomainEntity
    {
        public int StudentId { get; set; }
        public int ProgramId { get; set; }
        public string Note { get; set; }
        public int Index { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string ProgramModel { get; set; }
    }
}