namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_BillDetail : DomainEntity
    {
        public int BillId { get; set; }
        public int StudentId { get; set; }
        /// <summary>
        /// Lớp
        /// </summary>
        public int? ClassId { get; set; }
        /// <summary>
        /// Chương trình học
        /// </summary>
        public int? ProgramId { get; set; }
        /// <summary>
        /// Giáo trình học
        /// </summary>
        public int? CurriculumId { get; set; }
        /// <summary>
        /// Giỏ hàng
        /// </summary>
        public int? CartId { get; set; }

        /// <summary>
        /// Sản phẩm
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        [NotMapped]
        public string ClassName { get; set; }
        [NotMapped]
        public string ProgramName { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public string CurriculumName { get; set; }
        public tbl_BillDetail() : base() { }
        public tbl_BillDetail(object model) : base(model) { }
    }
    public class Get_BillDetail : DomainEntity
    {
        public int BillId { get; set; }
        public int StudentId { get; set; }
        /// <summary>
        /// Lớp
        /// </summary>
        public int? ClassId { get; set; }
        /// <summary>
        /// Chương trình học
        /// </summary>
        public int? ProgramId { get; set; }
        /// <summary>
        /// Giáo trình học
        /// </summary>
        public int? CurriculumId { get; set; }
        /// <summary>
        /// Giỏ hàng
        /// </summary>
        public int? CartId { get; set; }
        /// <summary>
        /// Sản phẩm
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public string ClassName { get; set; }
        public string ProgramName { get; set; }
        public string ProductName { get; set; }
        public string CurriculumName { get; set; }
    }
}