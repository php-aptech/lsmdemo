namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_Customer : DomainEntity
    {
        public string Code { get; set; }
        /// <summary>
        /// Nhu cầu học
        /// </summary>
        public int? LearningNeedId { get; set; }
        public int? CustomerStatusId { get; set; }
        public string FullName { get; set; }
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Mobile { get; set; }
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        /// <summary>
        /// Nguồn khách hàng
        /// </summary>
        public int? SourceId { get; set; }
        /// <summary>
        /// Tư vấn viên
        /// </summary>
        public int? SaleId { get; set; }
        /// <summary>
        /// Mục đích học
        /// </summary>
        public int? PurposeId { get; set; }
        public int? AreaId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string Address { get; set; }
        public int? JobId { get; set; }
        /// <summary>
        /// Trung tâm
        /// </summary>
        public int? BranchId { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        public string CustomerStatusName { get; set; }
        [NotMapped]
        public string SaleName { get; set; }
        public tbl_Customer() : base() { }
        public tbl_Customer(object model) : base(model) { }
    }
    public class Get_Customer : DomainEntity
    {
        public string Code { get; set; }
        /// <summary>
        /// Nhu cầu học
        /// </summary>
        public int? LearningNeedId { get; set; }
        public int? CustomerStatusId { get; set; }
        public string FullName { get; set; }
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Mobile { get; set; }
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        /// <summary>
        /// Nguồn khách hàng
        /// </summary>
        public int? SourceId { get; set; }
        /// <summary>
        /// Tư vấn viên
        /// </summary>
        public int? SaleId { get; set; }
        /// <summary>
        /// Mục đích học
        /// </summary>
        public int? PurposeId { get; set; }
        public int? AreaId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string Address { get; set; }
        public int? JobId { get; set; }
        /// <summary>
        /// Trung tâm
        /// </summary>
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
		public string CustomerStatusName { get; set; }
	    public string SaleName { get; set; }
        public int TotalRow { get; set; }
    }
}