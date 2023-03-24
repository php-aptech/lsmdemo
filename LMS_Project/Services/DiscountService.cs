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
    public class DiscountService
    {
        public static async Task<tbl_Discount> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Discount.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_Discount> GetByCode(string code)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Discount.FirstOrDefaultAsync(x => x.Code.ToUpper() == code.ToUpper());
                if (data == null)
                    throw new Exception("Mã khuyến mãi không phù hợp");
                if (data.Status == 2)
                    throw new Exception("Khuyến mãi đã hết hạn");
                if (data.Quantity <= data.UsedQuantity)
                    throw new Exception("Khuyến mãi đã dùng hết");

                return data;
            }
        }
        public static async Task<tbl_Discount> Insert(DiscountCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var checkCode = await db.tbl_Discount.AnyAsync(x => x.Code.ToUpper() == itemModel.Code.ToUpper() && x.Enable == true);
                if (checkCode)
                    throw new Exception("Mã đã tồn tại");
                if(AssetCRM.CheckUnicode(itemModel.Code))
                    throw new Exception("Mã không hợp lệ");
                var model = new tbl_Discount(itemModel);
                if (model.Type == 1)
                    model.MaxDiscount = model.Value;
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_Discount.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }

        public static async Task<tbl_Discount> Update(DiscountUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Discount.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Value = itemModel.Value ?? entity.Value;
                if (entity.Type == 1)
                    entity.MaxDiscount = entity.Value;

                entity.Status = itemModel.Status ?? entity.Status;
                entity.StatusName = itemModel.StatusName ?? entity.StatusName;
                entity.Note = itemModel.Note ?? entity.Note;
                entity.Expired = itemModel.Expired ?? entity.Expired;
                entity.Quantity = itemModel.Quantity ?? entity.Quantity;
                entity.MaxDiscount = itemModel.MaxDiscount ?? entity.MaxDiscount;
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
                var entity = await db.tbl_Discount.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public static async Task<AppDomainResult> GetAll(DiscountSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new DiscountSearch();
                string sql = $"Get_Discount @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@Status = {baseSearch.Status}," +
                    $"@Code = N'{baseSearch.Code ?? ""}'";
                var data = await db.Database.SqlQuery<Get_Discount> (sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_Discount(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public static async Task Expired()
        {
            using (var db = new lmsDbContext())
            {
                var discounts = await db.tbl_Discount.Where(x => x.Status == 1 && x.Expired <= DateTime.Now && x.Enable == false).ToListAsync();
                if (discounts.Any())
                {
                    foreach (var item in discounts)
                    {
                        item.Status = 2;
                        item.StatusName = "Đã kết thúc";
                        await db.SaveChangesAsync();
                    }
                }
            }
        }
    }
}