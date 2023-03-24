namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_Certificate : DomainEntity
    {
        public string Name { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; }
        [NotMapped]
        public string UserCode { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string Mobile { get; set; } // số điện thoại
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public string Avatar { get; set; }
        public tbl_Certificate() : base() { }
        public tbl_Certificate(object model) : base(model) { }
    }
    public class Get_Certificate : DomainEntity
    {
        public string Name { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; }
        public string UserCode { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; } // số điện thoại
        public string Email { get; set; }
        public string Avatar { get; set; }
        public int TotalRow { get; set; }
    }
}