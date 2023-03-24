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
    public class TeacherOffService
    {
        public static async Task<tbl_TeacherOff> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_TeacherOff.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_TeacherOff> Insert(TeacherOffCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (itemModel.EndTime <= itemModel.StartTime)
                    throw new Exception("Thời gian đăng ký không phù hợp");
                var model = new tbl_TeacherOff(itemModel);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                model.TeacherId = user.UserInformationId;
                db.tbl_TeacherOff.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task<tbl_TeacherOff> Update(TeacherOffUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_TeacherOff.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                
                entity.Status = itemModel.Status ?? entity.Status;
                entity.StatusName = itemModel.StatusName ?? entity.StatusName;
                entity.Note = itemModel.Note ?? entity.Note;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                //nếu trạng thái duyệt/không duyệ thì thông báo đến cho giáo viên
                if (entity.Status == 2 || entity.Status == 3)
                {
                    string content = "Đơn xin nghỉ phép của bạn từ ngày " + entity.StartTime.Value.ToString("dd/MM/yyyy HH:mm") + " đến ngày " + entity.EndTime.Value.ToString("dd/MM/yyyy HH:mm");
                    if (entity.Status == 2)
                    {
                        content = " đã được duyệt";
                    }
                    else//status = 3
                    {
                        content = " không được duyệt";
                    }

                    Thread sendNoti = new Thread(async () =>
                    {
                        await NotificationService.Send(new tbl_Notification
                        {
                            Title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " thông báo về đơn nghỉ phép của bạn.",
                            Content = content,
                            UserId = entity.TeacherId
                        }, user);
                    });
                    sendNoti.Start();
                }
                return entity;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_TeacherOff.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public static async Task<AppDomainResult> GetAll(TeacherOffSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new TeacherOffSearch();
                int teacherId = 0;
                if (user.RoleId == ((int)RoleEnum.teacher))
                    teacherId = user.UserInformationId;
                string sql = $"Get_TeacherOff @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@FullName = N'{baseSearch.FullName ?? ""}'," +
                    $"@UserCode = N'{baseSearch.UserCode ?? ""}'," +
                    $"@TeacherId = {teacherId}," +
                    $"@Status = N'{baseSearch.Status ?? ""}'";
                var data = await db.Database.SqlQuery<Get_TeacherOff>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_TeacherOff(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public static async Task<AppDomainResult> GetScheduleTeacherOff(ScheduleTeacherOffSearch search)
        {
            using (var db = new lmsDbContext())
            {
                var teacherOff = await db.tbl_TeacherOff.SingleOrDefaultAsync(x => x.Id == search.TeacherOffId);
                if(teacherOff == null)
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                string sql = $"Get_Schedule @Search = '', @PageIndex = {search.PageIndex}," +
                    $"@PageSize = {search.PageSize}," +
                    $"@TeacherIds = {teacherOff.TeacherId}," +
                    $"@From = N'{(!teacherOff.StartTime.HasValue ? "" : teacherOff.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss"))}'," +
                    $"@To = N'{(!teacherOff.EndTime.HasValue ? "" : teacherOff.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss"))}'";
                var data = await db.Database.SqlQuery<Get_Schedule>(sql).ToListAsync();

                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_Schedule(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
    }
}