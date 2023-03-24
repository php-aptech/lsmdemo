﻿namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_PackageSection : DomainEntity
    {
        /// <summary>
        /// ProductId
        /// </summary>
        public int PackageId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Số lượng đề
        /// </summary>
        public int ExamQuatity { get; set; }
        public tbl_PackageSection() : base() { }
        public tbl_PackageSection(object model) : base(model) { }
    }
}