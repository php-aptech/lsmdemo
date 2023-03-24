namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_PaymentSession : DomainEntity
    {
        public int BranchId { get; set; }
        public int? UserId { get; set; }
        public double Value { get; set; }
        public string PrintContent { get; set; }
        public int? PaymentMethodId { get; set; }
        [StringLength(1000)]
        public string Reason { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        /// <summary>
        /// 1 - Thu
        /// 2 - Chi
        /// </summary>
        public int? Type { get; set; }
        [StringLength(20)]
        public string TypeName { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string UserCode { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        public string PaymentMethodName { get; set; }
        public tbl_PaymentSession() : base() { }
        public tbl_PaymentSession(object model) : base(model) { }
    }
    public class Get_PaymentSession : DomainEntity
    {
        public int BranchId { get; set; }
        public int? UserId { get; set; }
        public double Value { get; set; }
        public string PrintContent { get; set; }
        public int? PaymentMethodId { get; set; }
        [StringLength(1000)]
        public string Reason { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        /// <summary>
        /// 1 - Thu
        /// 2 - Chi
        /// </summary>
        public int? Type { get; set; }
        [StringLength(20)]
        public string TypeName { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string BranchName { get; set; }
        public string PaymentMethodName { get; set; }
        public double TotalRevenue { get; set; }
        public double TotalIncome { get; set; }
        public double TotalExpense { get; set; }
        public int TotalRow { get; set; }
        public double TotalValue { get; set; }
    }
}