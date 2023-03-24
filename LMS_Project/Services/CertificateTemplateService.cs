using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class CertificateTemplateService
    {
        public class Guide
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }
        public static async Task<List<string>> GetGuide()
        {
            return new List<string> {
                "{TenHocVien} : Tên học viên",
                "{MaHocVien} : Mã học viên",
                "{NgayHoanThanh} : Ngày hoàn thành",
                "{Ngay} : Ngày hiện tại",
                "{Thang} : Tháng hiện tại",
                "{Nam} : Năm hiện tại",
            };
        }
        public static string ReplaceContent(string content, tbl_UserInformation user)
        {
            content = content.Replace("{TenHocVien}", user.FullName);
            content = content.Replace("{MaHocVien}", user.UserCode);
            content = content.Replace("{NgayHoanThanh}", DateTime.Now.ToString("dd/MM/yyyy"));
            content = content.Replace("{Ngay}", DateTime.Now.Day.ToString());
            content = content.Replace("{Thang}", DateTime.Now.Month.ToString());
            content = content.Replace("{Nam}", DateTime.Now.Year.ToString());
            return content;
        }
        public async static Task<tbl_CertificateTemplate> Insert(CertificateTemplateCreate request, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                tbl_CertificateTemplate entity = new tbl_CertificateTemplate(request);
                entity.CreatedBy = user.FullName;
                entity.CreatedOn = DateTime.Now;
                db.tbl_CertificateTemplate.Add(entity);
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public async static Task<tbl_CertificateTemplate> Update(CertificateTemplateUpdate request, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                tbl_CertificateTemplate entity = await db.tbl_CertificateTemplate.SingleOrDefaultAsync(x => x.Id == request.Id && x.Enable == true);
                if (entity == null)
                {
                    throw new Exception("Chứng chỉ không tồn tại");
                }
                entity.Name = request.Name ?? entity.Name;
                entity.Content = request.Content ?? entity.Content;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public async static Task<bool> Delete(int id, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                tbl_CertificateTemplate entity = await db.tbl_CertificateTemplate.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                if (entity == null)
                {
                    throw new Exception("Chứng chỉ không tồn tại");
                }
                entity.Enable = false;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                return true;
            }
        }
        public async static Task<AppDomainResult> Get(CertificateTemplateSearch search)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null)
                {
                    search = new CertificateTemplateSearch();
                }
                var listResult = db.tbl_CertificateTemplate.Where(x => x.Enable == true).AsQueryable();
                List<tbl_CertificateTemplate> listEntity = new List<tbl_CertificateTemplate>();
                if (!string.IsNullOrEmpty(search.Name))
                {
                    listResult = listResult.Where(x => x.Name.Contains(search.Name));
                }
                listEntity = await listResult.ToListAsync();
                return new AppDomainResult { Data = listEntity.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList(), TotalRow = listEntity.Count(), Success = true };
            }
        }
        public async static Task<string> GetTemplate(int studentId, int certificateTemplateId)
        {
            using (var db = new lmsDbContext())
            {
                tbl_UserInformation student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == studentId && x.RoleId == (int)RoleEnum.student && x.Enable == true);
                if (student == null)
                {
                    throw new Exception("Học viên không tồn tại");
                }
                tbl_CertificateTemplate template = await db.tbl_CertificateTemplate.SingleOrDefaultAsync(x => x.Id == certificateTemplateId && x.Enable == true);
                if (student == null)
                {
                    throw new Exception("Chứng chỉ không tồn tại");
                }
                return ReplaceContent(template.Content, student);
            }
        }
        public async static Task<tbl_CertificateTemplate> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_CertificateTemplate.SingleOrDefaultAsync(x=>x.Id == id && x.Enable == true);
                return entity;
            }
        }
    }
}