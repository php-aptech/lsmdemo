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

namespace LMS_Project.Services
{
    public class MarkSalaryService
    {
        public static async Task<tbl_MarkSalary> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_MarkSalary.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_MarkSalary> InsertOrUpdate(MarkSalaryCreate itemModel, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_MarkSalary.FirstOrDefaultAsync(x => x.TeacherId == itemModel.TeacherId && x.Enable == true);
                if (entity == null)
                {
                    entity = new tbl_MarkSalary(itemModel);
                    entity.CreatedBy = entity.ModifiedBy = userLog.FullName;
                    db.tbl_MarkSalary.Add(entity);
                }
                else
                {
                    entity.Salary = itemModel.Salary ?? entity.Salary;
                }
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public static async Task<AppDomainResult> GetAll(MarkSalarySearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new MarkSalarySearch();
                string sql = $"Get_MarkSalary @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@FullName = N'{baseSearch.FullName}'," +
                    $"@UserCode = N'{baseSearch.UserCode}'";
                var data = await db.Database.SqlQuery<Get_MarkSalary>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_MarkSalary(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
    }
}