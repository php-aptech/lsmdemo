namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_CertificateConfig : DomainEntity
    {
        public string Content { get; set; }
        public tbl_CertificateConfig() : base() { }
        public tbl_CertificateConfig(object model) : base(model) { }
    }
    public class CertificateConfigModel
    {
        public string Content { get; set; }
    }
}