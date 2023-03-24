namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_GeneralNotification : DomainEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Danh sách người nhận thông báo
        /// </summary>
        public string UserIds { get; set; }
        public bool? IsSendMail { get; set; }
        public tbl_GeneralNotification() : base() { }
        public tbl_GeneralNotification(object model) : base(model) { }
    }
}