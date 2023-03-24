using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class ScheduleService
    {
        public static async Task<tbl_Schedule> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Schedule.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_Schedule> Insert(ScheduleCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == itemModel.ClassId);
                if (_class == null)
                    throw new Exception("Không tìm thấy lớp học");
                if (itemModel.EndTime <= itemModel.StartTime)
                    throw new Exception("Thời gian không phù hợp");
                var model = new tbl_Schedule(itemModel);
                if (_class.Type == 3)
                {
                    var curriculum = await db.tbl_Curriculum.SingleOrDefaultAsync(x => x.Id == _class.CurriculumId);
                    if (curriculum == null)
                        throw new Exception("Không tìm thấy giáo trình");
                    var countSchedule = await db
                        .tbl_Schedule.CountAsync(x => x.ClassId == itemModel.ClassId
                        && x.Enable == true
                        && x.StatusTutoring != 2
                        && x.StatusTutoring != 4
                        && x.StatusTutoring != 5);
                    if (countSchedule >= curriculum.Lesson)
                        throw new Exception($"Bạn đã học hết số buổi theo giáo trình {curriculum.Name}");

                    int openTime = 0;
                    var openTutoring = await db.tbl_Config.FirstOrDefaultAsync(x => x.Code == "OpenTutoring");
                    if (openTutoring != null)
                        openTime = int.Parse(openTutoring.Value);

                    var timeNow = DateTime.Now.AddHours(-openTime);
                    if (itemModel.StartTime < timeNow)
                        throw new Exception($"Vui lòng đặt lịch trước {openTime} tiếng");

                    model.StatusTutoring = 1;
                    model.StatusTutoringName = "Mới đặt";
                    var checkScheduleAvailable = await db.tbl_ScheduleAvailable
                        .AnyAsync(x => x.StartTime <= itemModel.StartTime && x.EndTime >= itemModel.EndTime && x.Enable == true);
                    if (!checkScheduleAvailable)
                        throw new Exception("Giáo viên không mở lịch thời gian này");

                    var tutoringFee = await db.tbl_TutoringFee
                        .FirstOrDefaultAsync(x => x.TeacherId == model.TeacherId && x.Enable == true);
                    if (tutoringFee == null)
                        model.TeachingFee = 0;
                    else model.TeachingFee = tutoringFee.Fee;

                    //Kiểm tra lịch học của học viên khi tạo lịch
                    var myClass = await db.tbl_StudentInClass.Where(x => x.StudentId == user.UserInformationId && x.Enable == true).Select(x => x.ClassId).ToListAsync();
                    var checkMySchedule = await db.tbl_Schedule
                        .FirstOrDefaultAsync(x => x.Enable == true && x.StartTime < itemModel.EndTime && x.EndTime > itemModel.StartTime && myClass.Contains(x.ClassId)
                            && x.StatusTutoring != 2 //Lớp dạy kèm bỏ qua buổi đã hủy
                            && x.StatusTutoring != 4
                            && x.StatusTutoring != 5);
                    if (checkMySchedule != null)
                        throw new Exception($"Bạn đã có lịch học từ {checkMySchedule.StartTime} đến {checkMySchedule.EndTime}");
                }
                if (model.RoomId != 0)
                {
                    var checkRoom = await ClassService.CheckRoom(0, model.RoomId, model.StartTime.Value, model.EndTime.Value);
                    if (checkRoom.Value == false)
                        throw new Exception(checkRoom.Note);
                }
                var checkTeacher = await ClassService.CheckTeacher(0, model.TeacherId, model.StartTime.Value, model.EndTime.Value);
                if (checkTeacher.Value == false)
                    throw new Exception(checkTeacher.Note);

                var checkSchedule = await db.tbl_Schedule
                    .FirstOrDefaultAsync(x => x.Enable == true && x.StartTime < itemModel.EndTime && x.EndTime > itemModel.StartTime && x.ClassId == model.ClassId);
                if (checkSchedule != null)
                    throw new Exception($"Trùng lịch từ {checkSchedule.StartTime} đến {checkSchedule.EndTime}");

                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_Schedule.Add(model);
                await db.SaveChangesAsync();
                await UpdateEndDayClass(model.ClassId.Value);

                string contentTeacher = $"Bạn đã được thêm một buổi dạy mới lớp {_class.Name} " +
                    $"vào lúc {(model.StartTime.Value.ToString("dd/MM/yyyy HH:mm"))} " +
                    $"vui lòng kiểm tra lịch.";
                string contentStudent = $"Bạn đã được thêm một buổi học mới lớp {_class.Name} " +
                    $"vào lúc {(model.StartTime.Value.ToString("dd/MM/yyyy HH:mm"))} " +
                    $"vui lòng kiểm tra lịch.";
                string title = "Thay đổi lịch";
                var studentInClass = await db.tbl_StudentInClass.Where(x => x.ClassId == model.ClassId && x.Enable == true).ToListAsync();
                Thread sendNoti = new Thread(async () =>
                {
                    await NotificationService.Send(new tbl_Notification
                    {
                        Title = title,
                        Content = contentTeacher,
                        UserId = model.TeacherId
                    }, user);
                    if (studentInClass.Any())
                    {
                        foreach (var item in studentInClass)
                        {
                            await NotificationService.Send(new tbl_Notification
                            {
                                Title = title,
                                Content = contentStudent,
                                UserId = item.StudentId
                            }, user);
                        }
                    }
                });
                sendNoti.Start();

                return model;
            }
        }
        public static async Task UpdateEndDayClass(int classId)
        {
            using (var db = new lmsDbContext())
            {
                var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == classId);
                if (_class != null)
                {
                    var lastSchedule = await db.tbl_Schedule
                        .Where(x => x.ClassId == classId && x.Enable == true).OrderByDescending(x => x.EndTime).FirstOrDefaultAsync();
                    if (lastSchedule != null)
                    {
                        _class.EndDay = lastSchedule.EndTime;
                        await db.SaveChangesAsync();
                    }
                }
            }
        }
        public static async Task<tbl_Schedule> Update(ScheduleUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Schedule.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                if (entity.TeacherAttendanceId != 0)
                    throw new Exception("Đã học không thể chỉnh sửa");
                if (entity.StatusTutoring == 2)
                    throw new Exception("Buổi học đã hủy, không thể cập nhật");
                if (entity.StatusTutoring == 4 || entity.StatusTutoring == 5)
                    throw new Exception("Không thể cập nhật buổi này");
                if (entity.SalaryId.HasValue && entity.SalaryId != 0 && entity.TeachingFee != itemModel.TeachingFee && itemModel.TeachingFee.HasValue)
                    throw new Exception("Buổi học này đã được tính lương, không thể cập nhật lương giảng dạy");
                entity.StatusTutoring = itemModel.StatusTutoring ?? entity.StatusTutoring;
                entity.StatusTutoringName = itemModel.StatusTutoringName ?? entity.StatusTutoringName;
                entity.RoomId = itemModel.RoomId ?? entity.RoomId;
                entity.StartTime = itemModel.StartTime ?? entity.StartTime;
                entity.EndTime = itemModel.EndTime ?? entity.EndTime;
                entity.TeacherId = itemModel.TeacherId ?? entity.TeacherId;
                entity.Note = itemModel.Note ?? entity.Note;
                entity.TeachingFee = itemModel.TeachingFee ?? entity.TeachingFee;

                if (entity.RoomId != 0)
                {
                    var checkRoom = await ClassService.CheckRoom(entity.Id, entity.RoomId, entity.StartTime.Value, entity.EndTime.Value);
                    if (checkRoom.Value == false)
                        throw new Exception(checkRoom.Note);
                }
                var checkTeacher = await ClassService.CheckTeacher(entity.Id, entity.TeacherId, entity.StartTime.Value, entity.EndTime.Value);
                if (checkTeacher.Value == false)
                    throw new Exception(checkTeacher.Note);

                var checkSchedule = await db.tbl_Schedule
                    .FirstOrDefaultAsync(x => x.Enable == true && x.StartTime < entity.EndTime && x.EndTime > entity.StartTime && x.Id != entity.Id && x.ClassId == entity.ClassId);
                if (checkSchedule != null)
                    throw new Exception($"Trùng lịch từ {checkSchedule.StartTime} đến {checkSchedule.EndTime}");

                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                await UpdateEndDayClass(entity.ClassId.Value);

                var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == entity.ClassId);
                string contentTeacher = $"Lịch dạy lớp {_class?.Name} có thay đổi, vui lòng kiểm tra.";
                string contentStudent = $"Lịch học lớp {_class?.Name} có thay đổi, vui lòng kiểm tra.";
                string title = "Thay đổi lịch";
                var studentInClass = await db.tbl_StudentInClass.Where(x => x.ClassId == entity.ClassId && x.Enable == true).ToListAsync();
                Thread sendNoti = new Thread(async () =>
                {
                    await NotificationService.Send(new tbl_Notification
                    {
                        Title = title,
                        Content = contentTeacher,
                        UserId = entity.TeacherId
                    }, user);
                    if (studentInClass.Any())
                    {
                        foreach (var item in studentInClass)
                        {
                            await NotificationService.Send(new tbl_Notification
                            {
                                Title = title,
                                Content = contentStudent,
                                UserId = item.StudentId
                            }, user);
                            tbl_UserInformation student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == item.StudentId);
                            if (student.ParentId.HasValue)
                            {
                                await NotificationService.Send(new tbl_Notification
                                {
                                    Title = title,
                                    Content = contentStudent,
                                    UserId = student.ParentId
                                }, user);
                            }
                        }
                    }
                });
                sendNoti.Start();
                return entity;
            }
        }
        /// <summary>
        /// Hủy lịch
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        public static async Task<tbl_Schedule> TutoringCancel(int scheduleId, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                var schedule = await db.tbl_Schedule.SingleOrDefaultAsync(x => x.Id == scheduleId);
                if (schedule == null)
                    throw new Exception("Không tìm thấy buổi học này");
                var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == schedule.ClassId);
                if (_class == null)
                    throw new Exception("Không tìm thấy lớp học");
                if (_class.Type != 3)
                    throw new Exception("Chỉ có thể hủy lịch lớp dạy kèm");

                int cancelTime = 0;
                var cancelTutoring = await db.tbl_Config.FirstOrDefaultAsync(x => x.Code == "CancelTutoring");
                if (cancelTutoring != null)
                    cancelTime = int.Parse(cancelTutoring.Value);

                var timeNow = DateTime.Now.AddHours(-cancelTime);
                if (schedule.StartTime < timeNow)
                    throw new Exception($"Vui lòng hủy lịch trước {cancelTime} tiếng");

                schedule.StatusTutoring = 2;
                schedule.StatusTutoringName = "Hủy";
                schedule.ModifiedBy = userLog.FullName;
                schedule.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();

                string content = $"Buổi học vào lúc {(schedule.StartTime.Value.ToString("dd/MM/yyyy HH:mm"))} lớp {_class.Name} đã hủy " +
                    $"vui lòng kiểm tra lịch.";
                string title = "Hủy lịch";
                var studentInClass = await db.tbl_StudentInClass.Where(x => x.ClassId == schedule.ClassId && x.Enable == true).ToListAsync();
                Thread sendNoti = new Thread(async () =>
                {
                    await NotificationService.Send(new tbl_Notification
                    {
                        Title = title,
                        Content = content,
                        UserId = schedule.TeacherId
                    }, userLog);
                    if (studentInClass.Any())
                    {
                        foreach (var item in studentInClass)
                        {
                            await NotificationService.Send(new tbl_Notification
                            {
                                Title = title,
                                Content = content,
                                UserId = item.StudentId
                            }, userLog);
                        }
                    }
                });
                sendNoti.Start();
                return schedule;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Schedule.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy lớp");
                if (entity.TeacherAttendanceId != 0)
                    throw new Exception("Đã học không thể xóa");
                entity.Enable = false;
                await db.SaveChangesAsync();
                await UpdateEndDayClass(entity.ClassId.Value);
            }
        }
        public static async Task<AppDomainResult> GetAll(ScheduleSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new ScheduleSearch();
                if (user.RoleId == ((int)RoleEnum.student))
                    baseSearch.StudentId = user.UserInformationId;
                if (user.RoleId == ((int)RoleEnum.teacher))
                    baseSearch.TeacherIds = user.UserInformationId.ToString();
                string sql = $"Get_Schedule @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@ClassId = {baseSearch.ClassId}," +
                    $"@StudentId = {baseSearch.StudentId}," +
                    $"@BranchIds = N'{baseSearch.BranchIds ?? ""}'," +
                    $"@ParentId = N'{baseSearch.ParentId}'," +
                    $"@From = N'{(!baseSearch.From.HasValue ? "" : baseSearch.From.Value.ToString("yyyy-MM-dd HH:mm:ss"))}'," +
                    $"@To = N'{(!baseSearch.To.HasValue ? "" : baseSearch.To.Value.ToString("yyyy-MM-dd HH:mm:ss"))}'," +
                    $"@TeacherIds = N'{baseSearch.TeacherIds ?? ""}'";
                var data = await db.Database.SqlQuery<Get_Schedule>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_Schedule(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public class RateTeacherModel
        {
            [Required(ErrorMessage = "Vui lòng chọn buổi học")]
            public int ScheduleId { get; set; }
            /// <summary>
            /// Đánh giá từ 1 đến 5 sao
            /// </summary>
            public int? RateTeacher { get; set; }
            public string RateTeacherComment { get; set; }
        }
        public static async Task RateTeacher(RateTeacherModel itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Schedule.SingleOrDefaultAsync(x => x.Id == itemModel.ScheduleId);
                if (data == null)
                    throw new Exception("Không tìm thấy buổi học");
                data.RateTeacher = itemModel.RateTeacher ?? data.RateTeacher;
                data.RateTeacherComment = itemModel.RateTeacherComment ?? data.RateTeacherComment;
                data.ModifiedBy = user.FullName;
                data.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
            }
        }
        #region Chức năng zoom
        /// Đến giờ dậy (trước mà sau thời gian ca học 15p) FE sẽ hiện nút Tạo phòng
        /// Nếu đã tạo rồi sẽ hiện nút Tạo lại, có IsRoomStart = true thì thêm tham gia và kết thúc
        public class ZoomReponse
        {
            public string ZoomId { get; set; }
            public string ZoomPass { get; set; }
            public string StartUrl { get; set; }
            public string JoinUrl { get; set; }
        }
        /// <summary>
        /// Nếu phòng đang hoạt động mà vẫn muốn tạo thì lấy tài khoản đang hoạt động tạo lại phòng mới
        /// </summary>
        /// <param name="courseScheduleId"></param>
        /// <param name="userLog"></param>
        /// <returns></returns>
        public static async Task<ZoomReponse> CreateZoom(int scheduleId, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var result = new ZoomReponse();
                        var schedule = await db.tbl_Schedule.SingleOrDefaultAsync(x => x.Id == scheduleId);
                        if (schedule == null)
                            throw new Exception("Không tìm thấy buổi học");

                        var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == schedule.ClassId);
                        if (_class == null)
                            throw new Exception("Không tìm thấy lớp học");

                        var zoomConfig = await db.tbl_ZoomConfig
                            .Where(x => x.Active == false && x.Enable == true)
                            .FirstOrDefaultAsync();

                        if (schedule.IsOpenZoom == true)
                        {
                            zoomConfig = await db.tbl_ZoomConfig.SingleOrDefaultAsync(x => x.Id == schedule.ZoomConfigId);
                        }

                        if (zoomConfig == null)
                            throw new Exception("Không có tài khoản Zoom nào đang trống, vui lòng thêm tài khoản");

                        var tokenOject = JObject.Parse(AccessTokenWithServerToServer(zoomConfig.AccountId, zoomConfig.ClientId, zoomConfig.ClientSecret));
                        
                        string tokenString = tokenOject["access_token"].ToString();
                        var client = new RestClient("https://api.zoom.us/v2/users/me/meetings");
                        var request = new RestRequest(Method.POST);
                        request.RequestFormat = DataFormat.Json;

                        var dataBody = new
                        {
                            topic = _class.Name,
                            type = "1",
                            settings = new
                            {
                                waiting_room = false
                            }
                        };

                        request.AddJsonBody(dataBody);
                        request.AddHeader("Authorization", String.Format("Bearer {0}", tokenString));
                        request.AddHeader("Content-Type", "application/json");

                        IRestResponse restResponse = client.Execute(request);
                        HttpStatusCode statusCode = restResponse.StatusCode;
                        int numericStatusCode = (int)statusCode;
                        var jObject = JObject.Parse(restResponse.Content);


                        if (numericStatusCode == 201)
                        {
                            if (string.IsNullOrEmpty(jObject["id"].ToString()) || string.IsNullOrEmpty(jObject["encrypted_password"].ToString()))
                            {
                                throw new Exception("Tạo phòng không thành công");
                            }
                            else
                            {
                                result.ZoomId = jObject["id"].ToString();
                                result.ZoomPass = jObject["password"].ToString();
                                result.StartUrl = jObject["start_url"].ToString();
                                result.JoinUrl = jObject["join_url"].ToString();
                            }
                        }
                        else
                            throw new Exception("Tạo phòng không thành công");

                        zoomConfig.Active = true;
                        schedule.ZoomId = result.ZoomId;
                        schedule.ZoomPass = result.ZoomPass;
                        schedule.StartUrl = result.StartUrl;
                        schedule.JoinUrl = result.JoinUrl;
                        schedule.IsOpenZoom = true;
                        schedule.TeacherAttendanceId = schedule.TeacherId;
                        schedule.ModifiedBy = userLog.FullName;
                        schedule.ModifiedOn = DateTime.Now;
                        schedule.ZoomConfigId = zoomConfig.Id;
                        await db.SaveChangesAsync();
                        tran.Commit();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }
        /// <summary>
        /// AccessToken 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public static string AccessTokenWithServerToServer(string acountId,string clientId, string clientSecret)
        {
            var client = new RestClient("https://zoom.us/oauth/token?grant_type=account_credentials&account_id=" + acountId);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            var authenticationString = $"{(clientId.Trim())}:{(clientSecret.Trim())}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
            request.AddHeader("Authorization", "Basic " + base64EncodedAuthenticationString);

            IRestResponse response = client.Execute(request);
            return response.Content;
        }
        /// <summary>
        /// Kết thúc phòng học
        /// </summary>
        /// <param name="courseScheduleId"></param>
        /// <param name="userLog"></param>
        /// <returns></returns>
        public static async Task CloseZoom(int scheduleId, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (userLog.RoleId == ((int)RoleEnum.student) || userLog.RoleId == ((int)RoleEnum.parents))
                            throw new Exception("Bạn không thể đóng phòng học");

                        var schedule = await db.tbl_Schedule.SingleOrDefaultAsync(x => x.Id == scheduleId);
                        if (schedule == null)
                            throw new Exception("Không tìm thấy phòng học");
                        if (schedule.IsOpenZoom == false)
                            throw new Exception("Phòng học này đã đóng");

                        if (!string.IsNullOrEmpty(schedule.ZoomId))
                        {
                            var zoomConfig = await db.tbl_ZoomConfig.SingleOrDefaultAsync(x => x.Id == schedule.ZoomConfigId);
                            if (zoomConfig != null)
                            {
                                zoomConfig.Active = false;
                                await db.SaveChangesAsync();
                            }

                            var tokenOject = JObject.Parse(AccessTokenWithServerToServer(zoomConfig.AccountId, zoomConfig.ClientId, zoomConfig.ClientSecret));
                            string tokenString = tokenOject["access_token"].ToString();

                            var client = new RestClient($"https://api.zoom.us/v2/meetings/{schedule.ZoomId}/status");
                            var request = new RestRequest(Method.PUT);
                            request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
                            request.AddParameter("application/json", "{\"action\":\"end\"}", ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);
                            HttpStatusCode statusCode = response.StatusCode;
                            int numericStatusCode = (int)statusCode;

                        }

                        schedule.IsOpenZoom = false;
                        schedule.ModifiedBy = userLog.FullName;
                        schedule.ModifiedOn = DateTime.Now;

                        await db.SaveChangesAsync();

                        //Nếu chưa điểm danh hệ thống sẽ tự động đánh giá vắng không phép
                        var studentInClass = await db.tbl_StudentInClass.Where(x => x.ClassId == schedule.ClassId && x.Enable == true)
                            .Select(x=>x.StudentId).ToListAsync();
                        if (studentInClass.Any())
                        {
                            foreach (var item in studentInClass)
                            {
                                var checkRollUp = await db.tbl_RollUp
                                    .AnyAsync(x => x.ClassId == schedule.ClassId && x.ScheduleId == schedule.Id && x.StudentId == item && x.Enable == true);
                                if (!checkRollUp)
                                {
                                    db.tbl_RollUp.Add(new tbl_RollUp
                                    {
                                        ClassId = schedule.ClassId,
                                        CreatedBy = userLog.FullName,
                                        CreatedOn = DateTime.Now,
                                        Enable = true,
                                        LearningStatus = 8,
                                        LearningStatusName = "Không nhận xét",
                                        ModifiedBy = userLog.FullName,
                                        ModifiedOn = DateTime.Now,
                                        Note = "",
                                        ScheduleId = schedule.Id,
                                        Status = 3,
                                        StatusName = "Vắng không phép",
                                        StudentId = item
                                    });
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public class recording_files
        {
            public string id { get; set; }
            public string meeting_id { get; set; }
            public string download_url { get; set; }
            public string file_type { get; set; }
            public string play_url { get; set; }
            public string recording_start { get; set; }
            public string recording_end { get; set; }
            public string file_size { get; set; }
            public string file_extension { get; set; }
            public string recording_type { get; set; }
        }
        public static async Task<List<recording_files>> GetRecording(int scheduleId)
        {
            try
            {

                using (var db = new lmsDbContext())
                {
                    var schedule = await db.tbl_Schedule.SingleOrDefaultAsync(x => x.Id == scheduleId);
                    if (schedule == null) throw new Exception("Không tìm thấy buổi học");

                    var zoomConfig = await db.tbl_ZoomConfig.SingleOrDefaultAsync(x => x.Id == schedule.ZoomConfigId);
                    if (zoomConfig == null) throw new Exception("Không tìm thấy tài khoản zoom");

                    List<recording_files> rFile = new List<recording_files>();
                    var tokenOject = JObject.Parse(AccessTokenWithServerToServer(zoomConfig.AccountId, zoomConfig.ClientId, zoomConfig.ClientSecret));
                    string tokenString = tokenOject["access_token"].ToString();
                    var client = new RestClient($"https://api.zoom.us/v2/meetings/{schedule.ZoomId}/recordings");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
                    IRestResponse response = await client.ExecuteAsync(request);
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;

                    //chỗ này hứng data nè
                    if (numericStatusCode == 200)
                    {
                        var jObject = JObject.Parse(response.Content);
                        if (jObject != null)
                        {
                            rFile = JsonConvert.DeserializeObject<List<recording_files>>(jObject["recording_files"].ToString());
                        }
                    }
                    if (rFile.Count() == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return rFile;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public static async Task AutoCloseZoom()
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var timeEnd = DateTime.Now.AddMinutes(-15);
                    var schedules = await db.tbl_Schedule
                        .Where(x => x.Enable == true && x.IsOpenZoom == true && x.EndTime <= timeEnd)
                        .ToListAsync();
                    if (schedules.Any())
                        foreach (var item in schedules)
                        {
                            ///Tắt mấy cái phòng còn chạy khi quá giờ 15p
                            await CloseZoom(item.Id, new tbl_UserInformation { RoleId = ((int)RoleEnum.admin), FullName = "Tự động" });
                        }
                }
                catch (Exception e)
                {
                    AssetCRM.Writelog("Schedule", "AutoCloseZoom", 1, e.Message + e.InnerException);
                }
            }
        }
        public class Get_ZoomRoom
        {
            public int Id { get; set; }
            public int? ClassId { get; set; }
            public string ClassName { get; set; }
            public int? BranchId { get; set; }
            public string BranchName { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public string ZoomId { get; set; }
			public string ZoomPass { get; set; }
			public int? ZoomConfigId { get; set; }
            public bool IsOpenZoom { get; set; }
            public string StartUrl { get; set; }
            public string JoinUrl { get; set; }
            public int? TeacherId { get; set; }
            public string TeacherName { get; set; }
            public int TotalRow { get; set; }
			  
        }
        public static async Task<AppDomainResult> GetZoomRoom(SearchOptions baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new SearchOptions();

                string sql = $"Get_ZoomRoom @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}";

                var data = await db.Database.SqlQuery<Get_ZoomRoom>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new
                {
                    Id = i.Id,
                    ClassId = i.ClassId,
                    ClassName = i .ClassName,
                    BranchId = i.BranchId,
                    BranchName = i.BranchName,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    ZoomId = i.ZoomId,
                    ZoomPass = i.ZoomPass,
                    ZoomConfigId = i.ZoomConfigId,
                    IsOpenZoom = i.IsOpenZoom,
                    StartUrl = i.StartUrl,
                    JoinUrl = i.JoinUrl,
                    TeacherId = i.TeacherId,
                    TeacherName = i.TeacherName
                }).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        #endregion
    }
}