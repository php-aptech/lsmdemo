namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_Bill : DomainEntity
    {
        public string Code { get; set; }
        public int StudentId { get; set; }
        public double TotalPrice { get; set; }
        public int? DiscountId { get; set; }
        [NotMapped]
        public string DiscountCode { get; set; }
        /// <summary>
        /// Giảm
        /// </summary>
        public double Reduced { get; set; }
        /// <summary>
        /// Đã trả
        /// </summary>
        public double Paid { get; set; }
        /// <summary>
        /// Nợ
        /// </summary>
        public double Debt { get; set; }
        public int PaymentMethodId { get; set; }
        [NotMapped]
        public string PaymentMethodName { get; set; }
        /// <summary>
        /// Ngày hẹn thanh toán
        /// </summary>
        public DateTime? PaymentAppointmentDate { get; set; }
        /// <summary>
        /// Ngày hoàn thành
        /// </summary>
        public DateTime? CompleteDate { get; set; }
        public int BranchId { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 1 - Đăng ký học 
        /// 2 - Mua dịch vụ
        /// 3 - Đăng ký lớp dạy kèm
        /// 4 - Tạo thủ công
        /// </summary>
        public int Type { get; set; }
        public string TypeName { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string UserCode { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        public tbl_Bill() : base() { }
        public tbl_Bill(object model) : base(model) { }
    }
    public class Get_Bill : DomainEntity
    {
        public string Code { get; set; }
        public int StudentId { get; set; }
        public double TotalPrice { get; set; }
        public int? DiscountId { get; set; }
        public string DiscountCode { get; set; }
        public double Value { get; set; }
        /// <summary>
        /// Giảm
        /// </summary>
        public double Reduced { get; set; }
        /// <summary>
        /// Đã trả
        /// </summary>
        public double Paid { get; set; }
        /// <summary>
        /// Nợ
        /// </summary>
        public double Debt { get; set; }
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        /// <summary>
        /// Ngày hẹn thanh toán
        /// </summary>
        public DateTime? PaymentAppointmentDate { get; set; }
        /// <summary>
        /// Ngày hoàn thành
        /// </summary>
        public DateTime? CompleteDate { get; set; }
        public int BranchId { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 1 - Đăng ký học 
        /// 2 - Mua dịch vụ
        /// </summary>
        public int Type { get; set; }
        public string TypeName { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string BranchName { get; set; }
        public double SumTotalPrice { get; set; }
        public double SumPaid { get; set; }
        public double SumDebt { get; set; }
        public int TotalRow { get; set; }
    }
}