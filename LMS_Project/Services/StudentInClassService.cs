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
using System.Threading;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class StudentInClassService
    {
        public static async Task<tbl_StudentInClass> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_StudentInClass.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        /// <summary>
        /// Thêm học thử
        /// </summary>
        /// <param name="itemModel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<tbl_StudentInClass> Insert(StudentInClassCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == itemModel.ClassId);
                if (_class == null)
                    throw new Exception("Không tìm thấy lớp");
                if (_class.Type == 3)
                    throw new Exception("Không thể thêm học viên vào lớp dạy kèm");
                var student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == itemModel.StudentId && x.RoleId == 3);
                if (student == null)
                    throw new Exception("Không tìm thấy học viên");
                var model = new tbl_StudentInClass(itemModel);
                model.BranchId = _class.BranchId;
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_StudentInClass.Add(model);
                await db.SaveChangesAsync();
                List<tbl_StudyRoute> studyRoutes = await db.tbl_StudyRoute.Where(x => x.Enable == true && x.StudentId == student.UserInformationId && x.ProgramId == _class.ProgramId && x.Status == (int)StudyRouteStatus.ChuaHoc).ToListAsync();
                if (studyRoutes.Any())
                {
                    foreach (var item in studyRoutes)
                    {
                        item.Status = (int)StudyRouteStatus.DangHoc;
                        item.StatusName = ListStudyRouteStatus().Where(x => x.Key == item.Status).FirstOrDefault().Value;
                    }
                    await db.SaveChangesAsync();
                }
                return model;
            }
        }
        /// <summary>
        /// Ghi chú và cảnh báo
        /// </summary>
        /// <param name="itemModel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<tbl_StudentInClass> Update(StudentInClassUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_StudentInClass.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");

                entity.Warning = itemModel.Warning ?? entity.Warning;
                entity.Note = itemModel.Note ?? entity.Note;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                //học viên bị cảnh báo thì thông báo cho học vụ và tư vấn viên
                tbl_Class _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == entity.ClassId);
                tbl_UserInformation student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == entity.StudentId);
                if (entity.Warning.HasValue && entity.Warning.Value)
                {
                    string title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " thông báo về học viên bị cảnh báo";
                    string content = "Học viên " + student.FullName + " của lớp " + _class.Name + " đã bị cảnh báo vì lí do: \"" + entity.Note + "\"";
                    //thông báo cho học vụ
                    if (_class.AcademicId.HasValue)
                    {
                        Thread sendNoti = new Thread(async () =>
                        {
                            await NotificationService.Send(new tbl_Notification
                            {
                                Title = title,
                                Content = content,
                                UserId = _class.AcademicId
                            }, user);
                        });
                        sendNoti.Start();
                    }
                    //thông báo cho tư vấn viên
                    Thread sendSale = new Thread(async () =>
                    {
                        await NotificationService.Send(new tbl_Notification
                        {
                            Title = title,
                            Content = content,
                            UserId = student.SaleId
                        }, user);
                    });
                    sendSale.Start();
                }
                return entity;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_StudentInClass.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == entity.ClassId);
                if (_class == null)
                    throw new Exception("Không tìm thấy lớp học");
                if (_class.Type == 3)
                    throw new Exception("Không thể xóa học viên ở lớp dạy kèm");
                entity.Enable = false;
                await db.SaveChangesAsync();
                List<tbl_StudyRoute> studyRoutes = await (from c in db.tbl_Class
                                                          join sr in db.tbl_StudyRoute on c.ProgramId equals sr.ProgramId into list
                                                          from l in list.DefaultIfEmpty()
                                                          where l.Enable == true && l.StudentId == entity.StudentId && l.Status == (int)StudyRouteStatus.DangHoc
                                                          select l).ToListAsync();
                if (studyRoutes.Any())
                {
                    foreach (var item in studyRoutes)
                    {
                        item.Status = (int)StudyRouteStatus.ChuaHoc;
                        item.StatusName = ListStudyRouteStatus().Where(x => x.Key == item.Status).FirstOrDefault().Value;
                    }
                    await db.SaveChangesAsync();
                }
            }
        }
        public class NewClassModel
        {
            public int ClassId { get; set; }
            public bool Fit { get; set; }
            public string Note { get; set; }
        }
        public static async Task<AppDomainResult> GetAll(StudentInClassSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new StudentInClassSearch();
                string myBranchIds = "";
                if (user.RoleId != ((int)RoleEnum.admin))
                    myBranchIds = user.BranchIds;
                int mySaleId = 0;
                if (user.RoleId == ((int)RoleEnum.sale))
                    mySaleId = user.UserInformationId;
                if (user.RoleId == ((int)RoleEnum.student))
                    baseSearch.StudentIds = user.UserInformationId.ToString();
                string warning = baseSearch.Warning.HasValue ? baseSearch.Warning.Value.ToString() : "null";
                string sql = $"Get_StudentInClass @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@ClassId = N'{baseSearch.ClassId}'," +
                    $"@MyBranchIds = N'{myBranchIds ?? ""}'," +
                    $"@SaleId = N'{mySaleId}'," +
                    $"@Warning = {warning}," +
                    $"@Sort = N'{baseSearch.Sort}'," +
                    $"@ParentIds = N'{baseSearch.ParentIds ?? ""}'," +
                    $"@StudentIds = N'{baseSearch.StudentIds ?? ""}'," +
                    $"@SortType = N'{(baseSearch.SortType ? 1 : 0)}'," +
                     $"@disable = N'{(baseSearch.disable ? 0 : 1)}'";
                var data = await db.Database.SqlQuery<Get_StudentInClass>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_StudentInClass(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }


        public class Get_StudentAvailable_StudentInClass
        {
            public int UserInformationId { get; set; }
            public string FullName { get; set; }
            public string UserCode { get; set; }
        }
        public static async Task<List<Get_StudentAvailable_StudentInClass>> GetStudentAvailable(int classId)
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_StudentAvailable_StudentInClass @classId = {classId}";
                var data = await db.Database.SqlQuery<Get_StudentAvailable_StudentInClass>(sql).ToListAsync();
                return data;
            }
        }
    }
}