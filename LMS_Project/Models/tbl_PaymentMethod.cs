namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_PaymentMethod : DomainEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public tbl_PaymentMethod() : base() { }
        public tbl_PaymentMethod(object model) : base(model) { }
    }
}