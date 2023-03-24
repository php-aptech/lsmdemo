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
    public class PaymentMethodService
    {
        public static async Task<tbl_PaymentMethod> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_PaymentMethod.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_PaymentMethod> Update(PaymentMethodUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_PaymentMethod.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Thumbnail = itemModel.Thumbnail ?? entity.Thumbnail;
                entity.Active = itemModel.Active ?? entity.Active;
                entity.Description = itemModel.Description ?? entity.Description;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public static async Task<AppDomainResult> GetAll(SearchOptions baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new SearchOptions();
                var l = await db.tbl_PaymentMethod.Where(x => x.Enable == true).OrderBy(x => x.Name).ToListAsync();
                if (!l.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                int totalRow = l.Count();
                var result = l.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }
    }
}