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
    public class TemplateService
    {
        public class Guide
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }
        public static async Task<List<Guide>> GetGuide(int type)
        {
            var result = new List<Guide>();
            if (type == 1)//Hợp đồng
            {
                result.Add(new Guide { Code = "{HoVaTen}", Name = "Họ và tên" });
                result.Add(new Guide { Code = "{MaHocVien}", Name = "Mã học viên" });
                result.Add(new Guide { Code = "{Ngay}", Name = "Ngày" });
                result.Add(new Guide { Code = "{Thang}", Name = "Tháng" });
                result.Add(new Guide { Code = "{Nam}", Name = "Năm" });
            }
            else if (type == 2)//Điều khoản
            {
                result.Add(new Guide { Code = "{TenCongTy}", Name = "Tên công ty" });
            }
            else if (type == 3)//Mẫu phiếu thu
            {
                result.Add(new Guide { Code = "{HoVaTen}", Name = "Họ và tên" });
                result.Add(new Guide { Code = "{MaHocVien}", Name = "Mã học viên" });
                result.Add(new Guide { Code = "{Ngay}", Name = "Ngày" });
                result.Add(new Guide { Code = "{Thang}", Name = "Tháng" });
                result.Add(new Guide { Code = "{Nam}", Name = "Năm" });
                result.Add(new Guide { Code = "{LyDo}", Name = "Lý do" });
                result.Add(new Guide { Code = "{SoTienThu}", Name = "Số tiền thu" });
                result.Add(new Guide { Code = "{NguoiThu}", Name = "Người thu" });
            }
            else if (type == 4)//Mẫu phiếu chi
            {
                result.Add(new Guide { Code = "{HoVaTen}", Name = "Họ và tên" });
                result.Add(new Guide { Code = "{MaHocVien}", Name = "Mã học viên" });
                result.Add(new Guide { Code = "{Ngay}", Name = "Ngày" });
                result.Add(new Guide { Code = "{Thang}", Name = "Tháng" });
                result.Add(new Guide { Code = "{Nam}", Name = "Năm" });
                result.Add(new Guide { Code = "{LyDo}", Name = "Lý do" });
                result.Add(new Guide { Code = "{SoTienChi}", Name = "Số tiền chi" });
                result.Add(new Guide { Code = "{NguoiChi}", Name = "Người Chi" });
            }
            return result;
        }
        public static async Task<tbl_Template> Update(TemplateUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Template.FirstOrDefaultAsync(x => x.Type == itemModel.Type);
                if (entity == null)
                {
                    entity = new tbl_Template(itemModel);
                    entity.CreatedBy = entity.ModifiedBy = user.FullName;
                    entity.CreatedOn = entity.ModifiedOn = DateTime.Now;
                    entity.Enable = true;
                    db.tbl_Template.Add(entity);
                    await db.SaveChangesAsync();
                }
                else
                {
                    entity.Content = itemModel.Content ?? entity.Content;
                    entity.ModifiedBy = user.FullName;
                    entity.ModifiedOn = DateTime.Now;
                    await db.SaveChangesAsync();
                }
                return entity;
            }
        }
        public static async Task<tbl_Template> GetByType(int type)
        {
            var data = await GetAll();
            return data.FirstOrDefault(x => x.Type == type);
        }
        public static async Task<List<tbl_Template>> GetAll()
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Template.Where(x => x.Enable == true).ToListAsync();
                if (!data.Any(x => x.Type == 1))
                    data.Add(new tbl_Template { Type = 1, TypeName = "Hợp đồng", Content = "", Enable = true });
                if (!data.Any(x => x.Type == 2))
                    data.Add(new tbl_Template { Type = 2, TypeName = "Điều khoản", Content = "", Enable = true });
                if (!data.Any(x => x.Type == 3))
                    data.Add(new tbl_Template { Type = 3, TypeName = "Phiếu thu", Content = "", Enable = true });
                if (!data.Any(x => x.Type == 4))
                    data.Add(new tbl_Template { Type = 4, TypeName = "Phiếu chi", Content = "", Enable = true });
                return data;
            }
        }
    }
}