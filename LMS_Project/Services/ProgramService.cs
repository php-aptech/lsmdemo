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
    public class ProgramService
    {
        public static async Task<tbl_Program> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Program.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_Program> Insert(ProgramCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var checkCode = await db.tbl_Program.AnyAsync(x => x.Code.ToUpper() == itemModel.Code.ToUpper() && x.Enable == true);
                if (checkCode)
                    throw new Exception("Mã đã tồn tại");
                var model = new tbl_Program(itemModel);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_Program.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task<tbl_Program> Update(ProgramUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Program.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                if (itemModel.Code != null)
                {
                    var checkCode = await db.tbl_Program.AnyAsync(x => x.Code.ToUpper() == itemModel.Code.ToUpper() && x.Enable == true && x.Id != entity.Id);
                    if (checkCode)
                        throw new Exception("Mã đã tồn tại");
                }
                entity.Code = itemModel.Code ?? entity.Code;
                entity.Name = itemModel.Name ?? entity.Name;
                entity.Price = itemModel.Price ?? entity.Price;
                entity.Description = itemModel.Description ?? entity.Description;
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
                var entity = await db.tbl_Program.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public static async Task<AppDomainResult> GetAll(ProgramSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new ProgramSearch();
                string sql = $"Get_Program @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@Name = N'{baseSearch.Name ?? ""}'," +
                    $"@Code = N'{baseSearch.Code ?? ""}'," +
                    $"@Search = N'{baseSearch.Search ?? ""}'," +
                    $"@GradeId = N'{baseSearch.GradeId}'," +
                    $"@Sort = N'{baseSearch.Sort}'," +
                    $"@SortType = N'{(baseSearch.SortType ? 1 : 0)}'";
                var data = await db.Database.SqlQuery<Get_Program>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_Program(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public class TeacherInProgramModel
        {
            public int? TeacherId { get; set; }
            public string TeacherName { get; set; }
            public string TeacherCode { get; set; }
            public int? ProgramId { get; set; }
            public bool Allow { get; set; }
        }

        public static async Task<AppDomainResult> GetTeacherInProgram(TeacherInProgramSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new TeacherInProgramSearch();

                var l = await db.tbl_UserInformation.Where(x => x.Enable == true
                && x.RoleId == ((int)RoleEnum.teacher) && x.StatusId == ((int)AccountStatus.active))
                    .Select(x=>  new { x.UserInformationId , x.FullName,x.UserCode }).OrderBy(x => x.FullName).ToListAsync();
                if (!l.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                int totalRow = l.Count();
                var data = l.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList();
                var teacherInPrograms = await db.tbl_TeacherInProgram.Where(x => x.ProgramId == baseSearch.ProgramId && x.Enable == true)
                    .Select(x=>x.TeacherId).ToListAsync();
                var result = (from i in data
                              join t in teacherInPrograms on i.UserInformationId equals t into pg
                              from t in pg.DefaultIfEmpty()
                              select new TeacherInProgramModel
                              {
                                  TeacherId = i.UserInformationId,
                                  TeacherName = i.FullName,
                                  TeacherCode = i.UserCode,
                                  ProgramId = baseSearch.ProgramId,
                                  Allow = t == null ? false : true
                              }).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }
        public static async Task AllowTeacher(int teacherId, int programId,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_TeacherInProgram.Where(x => x.TeacherId == teacherId && x.ProgramId == programId).FirstOrDefaultAsync();
                if (entity != null)
                {
                    entity.Enable = !entity.Enable;
                    entity.ModifiedBy = user.FullName;
                    entity.ModifiedOn = DateTime.Now;
                    await db.SaveChangesAsync();
                }
                else
                {
                    entity = new tbl_TeacherInProgram
                    {
                        CreatedBy = user.FullName,
                        CreatedOn = DateTime.Now,
                        Enable = true,
                        ModifiedBy = user.FullName,
                        ModifiedOn = DateTime.Now,
                        ProgramId = programId,
                        TeacherId = teacherId
                    };
                    db.tbl_TeacherInProgram.Add(entity);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}