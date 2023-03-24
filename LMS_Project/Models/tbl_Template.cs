namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_Template : DomainEntity
    {
        /// <summary>
        /// 1 - Hợp đồng
        /// 2 - Điều khoản
        /// 3 - Phiếu thu
        /// 4 - Phiếu chi
        /// </summary>
        public int? Type { get; set; }
        public string TypeName { get; set; }
        public string Content { get; set; }
        public tbl_Template() : base() { }
        public tbl_Template(object model) : base(model) { }
    }
}