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
    public class SalaryConfigService
    {
        public static async Task<tbl_SalaryConfig> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_SalaryConfig.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_SalaryConfig> InsertOrUpdate(SalaryConfigCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var userInformation = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == itemModel.UserId);
                if (userInformation == null)
                    throw new Exception("Không tìm thấy nhân viên");
                var entity = await db.tbl_SalaryConfig.FirstOrDefaultAsync(x => x.UserId == itemModel.UserId && x.Enable == true);
                if (entity == null)
                {
                    entity = new tbl_SalaryConfig(itemModel);
                    entity.CreatedBy = entity.ModifiedBy = user.FullName;
                    db.tbl_SalaryConfig.Add(entity);
                }
                else
                {
                    entity.Value = itemModel.Value ?? entity.Value;
                    entity.Note = itemModel.Note ?? entity.Note;
                }
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_SalaryConfig.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public static async Task<AppDomainResult> GetAll(SalaryConfigSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new SalaryConfigSearch();
                string sql = $"Get_SalaryConfig @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@FullName = N'{baseSearch.FullName ?? ""}'," +
                    $"@UserCode = N'{baseSearch.UserCode ?? ""}'";
                var data = await db.Database.SqlQuery<Get_SalaryConfig>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_SalaryConfig(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public class Get_UserAvailable_SalaryConfig
        {
            public int UserInformationId { get; set; }
            public string FullName { get; set; }
            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public string UserCode { get; set; }
        }
        public static async Task<List<Get_UserAvailable_SalaryConfig>> GetUserAvailable()
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_UserAvailable_SalaryConfig";
                var data = await db.Database.SqlQuery<Get_UserAvailable_SalaryConfig>(sql).ToListAsync();
                return data;
            }
        }
    }
}