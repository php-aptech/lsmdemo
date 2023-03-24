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
    public class ZoomConfigService
    {
        public static async Task<tbl_ZoomConfig> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_ZoomConfig.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_ZoomConfig> Insert(ZoomConfigCreate zoomConfigCreate,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var model = new tbl_ZoomConfig(zoomConfigCreate);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_ZoomConfig.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_ZoomConfig.SingleOrDefaultAsync(x => x.Id == id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    entity.Enable = false;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<AppDomainResult> GetAll(SearchOptions baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new SearchOptions();

                var l = await db.tbl_ZoomConfig.Where(x => x.Enable == true
                && (x.Name.Contains(baseSearch.Search) || string.IsNullOrEmpty(baseSearch.Search))).OrderByDescending(x => x.Id).ToListAsync();

                int totalRow = l.Count();
                var result = l.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }
    }
}