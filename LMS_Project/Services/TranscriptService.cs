using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class TranscriptService
    {
        public class TranscriptCreate
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public int? ClassId { get; set; }
        }
        public static async Task<tbl_Transcript> Insert(TranscriptCreate model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var _class = await db.tbl_Class.AnyAsync(x => x.Id == model.ClassId);
                    if (!_class)
                        throw new Exception("Không tìm thấy lớp học");
                    var data = new tbl_Transcript
                    {
                        Name = model.Name,
                        ClassId = model.ClassId,
                        Enable = true,
                        CreatedBy = user.FullName,
                        ModifiedBy = user.FullName,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now
                    };
                    db.tbl_Transcript.Add(data);
                    await db.SaveChangesAsync();
                    return data;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var data = await db.tbl_Transcript.SingleOrDefaultAsync(x => x.Id == id);
                    if (data == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    data.Enable = false;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public class TranscriptModel
        {
            public int Id { get; set; }
            public List<PointModel> Items { get; set; }
        }
        public class PointModel
        {
            public int StudentId { get; set; }
            public string Listening { get; set; }
            public string Speaking { get; set; }
            public string Reading { get; set; }
            public string Writing { get; set; }
            public string Medium { get; set; }
            public string Note { get; set; }
            public string StudentName { get; set; }
            public string StudentCode { get; set; }
        }
        
        public static async Task PointEdit(TranscriptModel model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var transcript = await db.tbl_Transcript.SingleOrDefaultAsync(x => x.Id == model.Id);
                        if (transcript == null)
                            throw new Exception("Không tìm thấy bảng điểm");
                        if (!model.Items.Any())
                            throw new Exception("Không tìm thấy dữ liệu");
                        foreach (var item in model.Items)
                        {
                            var point = await db.tbl_Point
                                .Where(x => x.StudentId == item.StudentId && x.TranscriptId == model.Id).FirstOrDefaultAsync();
                            if (point != null)
                            {
                                point.Listening = item.Listening ?? point.Listening;
                                point.Speaking = item.Speaking ?? point.Speaking;
                                point.Reading = item.Reading ?? point.Reading;
                                point.Writing = item.Writing ?? point.Writing;
                                point.Medium = item.Medium ?? point.Medium;
                                point.Note = item.Note ?? point.Note;
                            }
                            else
                            {
                                var student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == item.StudentId);
                                if (student == null)
                                    throw new Exception("Không tìm thấy học viên");
                                db.tbl_Point.Add(
                                    new tbl_Point
                                    {
                                        TranscriptId = model.Id,
                                        StudentId = item.StudentId,
                                        Listening = item.Listening,
                                        Speaking = item.Speaking,
                                        Reading = item.Reading,
                                        Writing = item.Writing,
                                        Medium = item.Medium,
                                        Note = item.Note,
                                        Enable = true,
                                        CreatedBy = user.FullName,
                                        ModifiedBy = user.FullName,
                                        CreatedOn = DateTime.Now,
                                        ModifiedOn = DateTime.Now
                                    });
                            }
                            await db.SaveChangesAsync();
                        }
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        throw e;
                    }
                }
            }
        }
        public static async Task<List<tbl_Transcript>> GetByClass(int classId)
        {
            using (var db = new lmsDbContext())
            {
                return await db.tbl_Transcript.Where(x => x.ClassId == classId && x.Enable == true).OrderByDescending(x => x.Id).ToListAsync();
            }
        }
        public static async Task<List<tbl_Point>> GetByStudentClass(int studentId, int classId)
        {
            using (var db = new lmsDbContext())
            {
                return await (from t in db.tbl_Transcript
                              join p in db.tbl_Point on t.Id equals p.TranscriptId into list
                              from l in list
                              where t.ClassId == classId && l.StudentId == studentId && l.Enable == true && t.Enable == true
                              select l).ToListAsync();
            }
        }
        public static async Task<List<PointModel>> GetPoint(int transcriptId)
        {
            using (var db = new lmsDbContext())
            {
                var transcript = await db.tbl_Transcript.SingleOrDefaultAsync(x => x.Id == transcriptId);
                if (transcript == null)
                    throw new Exception("Không tìm thấy bảng điểm");
                var points = await db.tbl_Point.Where(x => x.TranscriptId == transcript.Id && x.Enable == true).ToListAsync();
                var students = await db.tbl_StudentInClass.Where(x => x.ClassId == transcript.ClassId && x.Enable == true && x.StudentId > 0)
                    .Select(x => x.StudentId).Distinct().ToListAsync();
                var result = (from i in students
                              join p in points on i equals p.StudentId into pg
                              from n in pg.DefaultIfEmpty()
                              select new PointModel
                              {
                                  StudentId = i ?? 0,
                                  StudentName = Task.Run(()=>GetStudent(i ?? 0)).Result.FullName,
                                  StudentCode = Task.Run(() => GetStudent(i ?? 0)).Result.UserCode,
                                  Listening = n == null ? "" : n.Listening,
                                  Speaking = n == null ? "" : n.Speaking,
                                  Reading = n == null ? "" : n.Reading,
                                  Writing = n == null ? "" : n.Writing,
                                  Medium = n == null ? "" : n.Medium,
                                  Note = n == null ? "" : n.Note,
                              }).ToList();
                return result;
            }
        }
        public static async Task<tbl_UserInformation> GetStudent(int studentId)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == studentId);
                if (data == null)
                    return new tbl_UserInformation();
                return data;
            }
        }
        public static async Task<AppDomainResult> GetPointByStudent(PointSearch search)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null)
                {
                    search = new PointSearch();
                }
                string sql = $"Get_Point @PageIndex = {search.PageIndex}," +
                    $"@PageSize = {search.PageSize}," +                   
                    $"@ParentIds = '{search.ParentIds ?? ""}'," +
                    $"@ClassId = '{search.ClassId}'," +
                    $"@StudentIds = '{search.StudentIds ?? ""}'";
                var data = await db.Database.SqlQuery<Get_Point>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                return new AppDomainResult { TotalRow = totalRow, Data = data };
            }
        }
    }
}