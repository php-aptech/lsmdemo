namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Reflection;

    public class tbl_RollUp : DomainEntity
    {
        public int? ClassId { get; set; }
        public int? StudentId { get; set; }
        public int? ScheduleId { get; set; }
        /// <summary>
        ///    co,
        ///    vang_co_phep,
        ///    vang_khong_phep,
        ///    di_muon,
        ///    ve_som,
        ///    nghi_le
        /// </summary>
        public int? Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        ///    gioi,
        ///    kha,
        ///    trung_binh,
        ///    kem,
        ///    theo_doi_dac_biet,
        ///    co_co_gang,
        ///    khong_co_gang,
        ///    khong_nhan_xet
        /// </summary>
        public int? LearningStatus { get; set; }
        public string LearningStatusName { get; set; }
        public string Note { get; set; }
        public tbl_RollUp() : base() { }
        public tbl_RollUp(object model) : base(model) { }
    }
    public class Get_RollUp 
    {
        public int? ClassId { get; set; }
        public int? StudentId { get; set; }
        public int? ScheduleId { get; set; }
        /// <summary>
        ///    co,
        ///    vang_co_phep,
        ///    vang_khong_phep,
        ///    di_muon,
        ///    ve_som,
        ///    nghi_le
        /// </summary>
        public int? Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        ///    gioi,
        ///    kha,
        ///    trung_binh,
        ///    kem,
        ///    theo_doi_dac_biet,
        ///    co_co_gang,
        ///    khong_co_gang,
        ///    khong_nhan_xet
        /// </summary>
        public int? LearningStatus { get; set; }
        public string LearningStatusName { get; set; }
        public string Note { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public int TotalRow { get; set; }
        public string ScheduleModel { get; set; }
    }
    public class RollUpModel
    {
        public int? ClassId { get; set; }
        public int? StudentId { get; set; }
        public int? ScheduleId { get; set; }
        /// <summary>
        ///    co,
        ///    vang_co_phep,
        ///    vang_khong_phep,
        ///    di_muon,
        ///    ve_som,
        ///    nghi_le
        /// </summary>
        public int? Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        ///    gioi,
        ///    kha,
        ///    trung_binh,
        ///    kem,
        ///    theo_doi_dac_biet,
        ///    co_co_gang,
        ///    khong_co_gang,
        ///    khong_nhan_xet
        /// </summary>
        public int? LearningStatus { get; set; }
        public string LearningStatusName { get; set; }
        public string Note { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string ScheduleModel { get; set; }
        public RollUpModel() { }
        public RollUpModel(object model)
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