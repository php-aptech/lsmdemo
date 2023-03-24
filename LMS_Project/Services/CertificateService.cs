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
    public class CertificateService
    {
        public static async Task<AppDomainResult> GetAll(CertificateSearch baseSearch, tbl_UserInformation user)
        {

            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) return new AppDomainResult { TotalRow = 0, Data = null };
                if (user.RoleId == ((int)RoleEnum.student))
                    baseSearch.UserId = user.UserInformationId;
                string sql = $"Get_Certificate @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@UserId = {baseSearch.UserId}";
                var data = await db.Database.SqlQuery<Get_Certificate>(sql).ToListAsync();
                var myCourses = await db.tbl_VideoCourseStudent
                    .Where(x => x.UserId == user.UserInformationId && x.Enable == true).Select(x => x.VideoCourseId).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_Certificate(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public static async Task<tbl_Certificate> Update(CertificateUpdate model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Certificate.SingleOrDefaultAsync(x => x.Id == model.Id);
                if (entity == null)
                    throw new Exception("Không tìm thấy chứng chỉ");
                entity.Name = model.Name ?? entity.Name;
                entity.Content = model.Content ?? entity.Content;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public static async Task CreateCertificate(tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_Certificate.AnyAsync(x => x.UserId == user.UserInformationId);
                    if (entity)
                        throw new Exception("Bạn đã được cấp chứng chỉ");
                    var config = await db.tbl_CertificateConfig.FirstOrDefaultAsync();
                    if (config == null)
                        throw new Exception("Chưa tạo mẫu chứng chỉ, vui lòng liên hệ người quản trị");

                    var videoCoures = await db.tbl_Product.Where(x => x.Enable == true && x.Active == true)
                        .Select(x => new { x.Id, x.Name }).ToListAsync();
                    if (!videoCoures.Any())
                        throw new Exception("Không tìm thấy khoá học");
                    foreach (var item in videoCoures)
                    {
                        var lastSection = await db.tbl_Section.Where(x => x.VideoCourseId == item.Id && x.Enable == true)
                            .OrderByDescending(x => x.Index).Select(x => x.Id).FirstOrDefaultAsync();
                        var completed = await db.tbl_SectionCompleted.AnyAsync(x => x.SectionId == lastSection && x.Enable == true && x.UserId == user.UserInformationId);
                        if (!completed)
                            throw new Exception($"bạn chưa hoàn thành khoá học {item.Name}");
                    }
                    var model = new tbl_Certificate
                    {
                        Content = CertificateConfigService.ReplaceContent(config.Content, user),
                        CreatedBy = user.FullName,
                        CreatedOn = DateTime.Now,
                        Enable = true,
                        ModifiedBy = user.FullName,
                        ModifiedOn = DateTime.Now,
                        UserId = user.UserInformationId
                    };
                    db.tbl_Certificate.Add(model);
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<tbl_Certificate> Insert(CertificateProgramCreate request, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    tbl_UserInformation student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == request.UserId && x.RoleId == (int)RoleEnum.student && x.Enable == true);
                    if (student == null)
                    {
                        throw new Exception("Học viên không tồn tại");
                    }
                    var model = new tbl_Certificate
                    {
                        
                        Name = request.Name,
                        Content = request.Content,
                        CreatedBy = user.FullName,
                        CreatedOn = DateTime.Now,
                        Enable = true,
                        ModifiedBy = user.FullName,
                        ModifiedOn = DateTime.Now,
                        UserId = request.UserId
                    };
                    db.tbl_Certificate.Add(model);
                    await db.SaveChangesAsync();
                    return model;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public async static Task<tbl_Certificate> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Certificate.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                return entity;
            }
        }
        public async static Task<bool> Delete(int id, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                tbl_Certificate entity = await db.tbl_Certificate.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
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
    }
}