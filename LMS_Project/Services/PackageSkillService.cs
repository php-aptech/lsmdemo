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
    public class PackageSkillService
    {
        public static async Task<tbl_PackageSkill> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_PackageSkill.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_PackageSkill> Insert(PackageSkillCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var packageSection = await db.tbl_PackageSection.SingleOrDefaultAsync(x => x.Id == itemModel.PackageSectionId);
                if (packageSection == null)
                    throw new Exception("Không tìm thấy phần");
                var exam = await db.tbl_Exam.SingleOrDefaultAsync(x => x.Id == itemModel.ExamId);
                if (exam == null)
                    throw new Exception("Không tìm thấy đề");

                var model = new tbl_PackageSkill(itemModel);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_PackageSkill.Add(model);

                packageSection.ExamQuatity += 1;

                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task<tbl_PackageSkill> Update(PackageSkillUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_PackageSkill.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Name = itemModel.Name ?? entity.Name;
                entity.Thumbnail = itemModel.Thumbnail ?? entity.Thumbnail;
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
                var entity = await db.tbl_PackageSkill.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;

                var packageSection = await db.tbl_PackageSection.SingleOrDefaultAsync(x => x.Id == entity.PackageSectionId);
                if (packageSection == null)
                    throw new Exception("Không tìm thấy phần");

                packageSection.ExamQuatity -= 1;

                await db.SaveChangesAsync();
            }
        }
        public static async Task<AppDomainResult> GetAll(PackageSkillSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new PackageSkillSearch();

                var l = await db.tbl_PackageSkill.Where(x => x.Enable == true
                && x.PackageSectionId == baseSearch.PackageSectionId).OrderBy(x => x.Name).ToListAsync();
                if (!l.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                int totalRow = l.Count();
                var result = l.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }
    }
}