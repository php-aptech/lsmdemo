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
    public class DayOffService
    {
        public static async Task<tbl_DayOff> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_DayOff.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_DayOff> Insert(DayOffCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                itemModel.sDate = itemModel.sDate.Value.Date;
                itemModel.eDate = itemModel.eDate.Value.Date;
                var model = new tbl_DayOff(itemModel);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_DayOff.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task<tbl_DayOff> Update(DayOffUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_DayOff.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                if(itemModel.sDate.HasValue)
                    itemModel.sDate = itemModel.sDate.Value.Date;
                if (itemModel.eDate.HasValue)
                    itemModel.eDate = itemModel.eDate.Value.Date;

                entity.Name = itemModel.Name ?? entity.Name;
                entity.eDate = itemModel.eDate ?? entity.eDate;
                entity.sDate = itemModel.sDate ?? entity.sDate;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_DayOff.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public static async Task<AppDomainResult> GetAll(SearchOptions baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new SearchOptions();

                var l = await db.tbl_DayOff.Where(x => x.Enable == true).OrderByDescending(x => x.Id).ToListAsync();

                if (!l.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                int totalRow = l.Count();
                var result = l.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }
     
    }
}