using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    public class tbl_CertificateTemplate : DomainEntity
    {
        /// <summary>
        /// tên chứng chỉ
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// nội dung
        /// </summary>
        public string Content { get; set; }
        public tbl_CertificateTemplate() : base() { }
        public tbl_CertificateTemplate(object model) : base(model) { }
    }
}