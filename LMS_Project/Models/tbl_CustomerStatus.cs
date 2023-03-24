namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_CustomerStatus : DomainEntity
    {
        /// <summary>
        /// 1 - Cần tư vấn
        /// 2 - Khác
        /// 3 - Đã chuyển học viên
        /// </summary>
        public int? Type { get; set; }
        public string Name { get; set; }
        public tbl_CustomerStatus() : base() { }
        public tbl_CustomerStatus(object model) : base(model) { }
    }
}