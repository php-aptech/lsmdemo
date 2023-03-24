using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class PaymentSessionService
    {
        public static async Task<tbl_PaymentSession> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_PaymentSession.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_PaymentSession> Insert(PaymentSessionCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var branch = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == itemModel.BranchId);
                if (branch == null)
                    throw new Exception("Không tìm thấy trung tâm");
                var model = new tbl_PaymentSession(itemModel);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_PaymentSession.Add(model);
                model.PrintContent = Task.Run(() => GetPrintContent(
                    model.Type ?? 0,
                    model.UserId ?? 0,
                    model.Reason,
                    model.Value,
                    user.FullName
                    )).Result;
                await db.SaveChangesAsync();
                return model;
            }
        }

        public static async Task<tbl_PaymentSession> Update(PaymentSessionUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_PaymentSession.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.PrintContent = itemModel.PrintContent ?? entity.PrintContent;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public static async Task<string> GetPrintContent(int type,int studentId, string reason,double value, string createBy,string fullName = "", string userCode = "")
        {
            using (var db = new lmsDbContext())
            {
                string result = "";
                int typeTemplate = type + 2;
                var template = await db.tbl_Template.FirstOrDefaultAsync(x => x.Type == typeTemplate && x.Enable == true);
                if (template != null)
                    result = template.Content;

                if (fullName == "" && userCode == "")
                {
                    var student = await db.tbl_UserInformation.FirstOrDefaultAsync(x => x.UserInformationId == studentId);
                    if (student != null)
                    {
                        fullName = student.FullName;
                        userCode = student.UserCode;
                    }
                }
                result = result.Replace("{HoVaTen}", fullName);
                result = result.Replace("{MaHocVien}", userCode);
                result = result.Replace("{Ngay}", DateTime.Now.Day.ToString());
                result = result.Replace("{Thang}", DateTime.Now.Month.ToString());
                result = result.Replace("{Nam}", DateTime.Now.Year.ToString());
                result = result.Replace("{LyDo}", reason);
                result = result.Replace("{SoTienThu}", String.Format("{0:0,0}", value));
                result = result.Replace("{SoTienChi}", String.Format("{0:0,0}", value));
                result = result.Replace("{NguoiThu}", createBy);
                result = result.Replace("{NguoiChi}", createBy);
                return result;
            }
        }
        //public static async Task Delete(int id)
        //{
        //    using (var db = new lmsDbContext())
        //    {
        //        var entity = await db.tbl_PaymentSession.SingleOrDefaultAsync(x => x.Id == id);
        //        if (entity == null)
        //            throw new Exception("Không tìm thấy dữ liệu");
        //        entity.Enable = false;
        //        await db.SaveChangesAsync();
        //    }
        //}
        public class PaymentSessionResult : AppDomainResult
        {
            public double TotalRevenue { get; set; }
            public double TotalIncome { get; set; }
            public double TotalExpense { get; set; }
        }
        public static async Task<PaymentSessionResult> GetAll(PaymentSessionSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new PaymentSessionSearch();
                string myBranchIds = "";
                if (user.RoleId != ((int)RoleEnum.admin))
                    myBranchIds = user.BranchIds;
                string sql = $"Get_PaymentSession @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@Type = {baseSearch.Type}," +
                    $"@BranchIds = N'{baseSearch.BranchIds ?? ""}'," +
                    $"@MyBranchIds = N'{myBranchIds ?? ""}',"+
                    $"@FromDate = N'{baseSearch.FromDate ?? ""}'," +
                    $"@ToDate = N'{baseSearch.ToDate ?? ""}'";

                var data = await db.Database.SqlQuery<Get_PaymentSession>(sql).ToListAsync();
                if (!data.Any()) return new PaymentSessionResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var totalRevenue = data[0].TotalRevenue;
                var totalIncome = data[0].TotalIncome;
                var totalExpense = data[0].TotalExpense;
                var totalValue = data[0].TotalValue;
                var result = data.Select(i => new tbl_PaymentSession(i)).ToList();
                return new PaymentSessionResult { 
                    TotalRow = totalRow, 
                    Data =  result, 
                    TotalRevenue = totalRevenue,
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                };
            }
        }
    }
}