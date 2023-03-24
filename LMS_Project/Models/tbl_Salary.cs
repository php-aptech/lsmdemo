namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Reflection;

    public class tbl_Salary : DomainEntity
    {
        public int UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        /// <summary>
        /// Lương cơ bản
        /// </summary>
        public double BasicSalary { get; set; }
        /// <summary>
        /// Lương dạy học
        /// </summary>
        public double TeachingSalary { get; set; }
        /// <summary>
        /// Khấu trừ
        /// </summary>
        public double Deduction { get; set; }
        /// <summary>
        /// Thưởng thêm
        /// </summary>
        public double Bonus { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        /// <summary>
        /// Tổng lương
        /// </summary>
        public double TotalSalary { get; set; }
        /// <summary>
        /// 1 - Chưa chốt
        /// 2 - Đã chốt
        /// 3 - Đã thanh toán
        /// </summary>
        public int Status { get; set; }
        public string StatusName { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string UserCode { get; set; }
        [NotMapped]
        public int RoleId { get; set; }
        [NotMapped]
        public string RoleName { get; set; }
        public tbl_Salary() : base() { }
        public tbl_Salary(object model) : base(model) { }
    }
    public class Get_Salary : DomainEntity
    {
        public int UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        /// <summary>
        /// Lương cơ bản
        /// </summary>
        public double BasicSalary { get; set; }
        /// <summary>
        /// Lương dạy học
        /// </summary>
        public double TeachingSalary { get; set; }
        /// <summary>
        /// Khấu trừ
        /// </summary>
        public double Deduction { get; set; }
        /// <summary>
        /// Thưởng thêm
        /// </summary>
        public double Bonus { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        /// <summary>
        /// Tổng lương
        /// </summary>
        public double TotalSalary { get; set; }
        /// <summary>
        /// 1 - Chưa chốt
        /// 2 - Đã chốt
        /// 3 - Đã thanh toán
        /// </summary>
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int TotalRow { get; set; }
    }
    public class Get_TeachingDetail
    {
        public int Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ClassName { get; set; }
        public string RoomName { get; set; }
        public double? TeachingFee { get; set; }
        public int TotalRow { get; set; }
    }
    public class TeachingDetailModel
    {
        public int Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ClassName { get; set; }
        public string RoomName { get; set; }
        public double? TeachingFee { get; set; }
        public TeachingDetailModel() { }
        public TeachingDetailModel(object model)
        {
            foreach (PropertyInfo me in this.GetType().GetProperties())
            {
                foreach (PropertyInfo item in model.GetType().GetProperties())
                {
                    if (me.Name == item.Name)
                    {
                        me.SetValue(this, item.GetValue(model));
                    }
                }
            }
        }
    }
}