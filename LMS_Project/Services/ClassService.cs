using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
    public class ClassService
    {
        ///Tải tiết học khi tạo lớp
        public class LessonSearch
        {
            [Required(ErrorMessage = "Vui lòng chọn giáo trình")]
            public int? CurriculumId { get; set; }
            [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
            public DateTime? StartDay { get; set; }
            /// <summary>
            /// Phòng học
            /// </summary>
            public int RoomId { get; set; } = 0;
            public List<TimeModel> TimeModels { get; set; }
            [Required(ErrorMessage = "Vui lòng chọn Trung tâm")]
            public int? BranchId { get; set; }
            [Required(ErrorMessage = "Vui lòng chọn giáo viên")]
            public int? TeacherId { get; set; }
        }
        public class TimeModel
        {
            /// <summary>
            /// Ngày trong tuần
            /// </summary>
            public int DayOfWeek { get; set; }
            /// <summary>
            /// Ca học
            /// </summary>
            public int StudyTimeId { get; set; }
        }
        /// <summary>
        /// Tải lịch học khi tạo lớp
        /// </summary>
        /// <returns></returns>
        //public static async Task<List<ScheduleCreates>> GetLessonWhenCreate(LessonSearch itemModel)
        //{
        //    using (var db = new lmsDbContext())
        //    {
        //        if (!itemModel.TimeModels.Any())
        //            throw new Exception("Không tìm thấy ngày học");
        //        var curriculum = await db.tbl_Curriculum.SingleOrDefaultAsync(x => x.Id == itemModel.CurriculumId);
        //        if (curriculum == null)
        //            throw new Exception("Không tìm thấy giáo trình");
        //        var branch = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == itemModel.BranchId);
        //        if (branch == null)
        //            throw new Exception("Không tìm thấy trung tâm");
        //        var teacher = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == itemModel.TeacherId);
        //        if (teacher == null)
        //            throw new Exception("Không tìm thấy giáo viên");
        //        var branchIds = teacher.BranchIds.Split(',');
        //        if (!branchIds.Any(x => x == branch.Id.ToString()))
        //            throw new Exception("Giáo viên không thuộc trung tâm này");
        //        var teacherInProgram = await db.tbl_TeacherInProgram.FirstOrDefaultAsync(x => x.TeacherId == itemModel.TeacherId && x.ProgramId == curriculum.ProgramId && x.Enable == true);
        //        if (teacherInProgram == null)
        //            throw new Exception("Giáo viên không được phép dạy chương trình này");
        //        var room = new tbl_Room { Id = 0, Name = "" };
        //        if (itemModel.RoomId != 0)
        //        {
        //            room = await db.tbl_Room.SingleOrDefaultAsync(x => x.Id == itemModel.RoomId);
        //            if (room == null)
        //                throw new Exception("Không tìm thấy phòng");
        //        }
        //        var result = new List<ScheduleCreates>();
        //        var date = itemModel.StartDay.Value.AddDays(-1).Date;
        //        do
        //        {
        //            date = date.AddDays(1);
        //            var dayOff = await db.tbl_DayOff
        //                .AnyAsync(x => x.Enable == true && x.sDate <= date && x.eDate >= date);
        //            if (dayOff)
        //                continue;
        //            var teacherOff = await db.tbl_TeacherOff
        //                .AnyAsync(x => x.Enable == true && x.StartTime <= date && x.EndTime >= date && x.Status == 2 && x.TeacherId == itemModel.TeacherId);
        //            if (teacherOff)
        //                continue;
        //            foreach (var item in itemModel.TimeModels)
        //            {
        //                if (item.DayOfWeek == ((int)date.Date.DayOfWeek))
        //                {
        //                    var studyTime = await db.tbl_StudyTime.SingleOrDefaultAsync(x => x.Id == item.StudyTimeId);
        //                    if (studyTime == null)
        //                        continue;
        //                    var stimes = studyTime.StartTime.Split(':');
        //                    DateTime st = new DateTime(date.Year, date.Month, date.Day, int.Parse(stimes[0]), int.Parse(stimes[1]), 0);
        //                    var etimes = studyTime.EndTime.Split(':');
        //                    DateTime et = new DateTime(date.Year, date.Month, date.Day, int.Parse(etimes[0]), int.Parse(etimes[1]), 0);

        //                    var check = await db.tbl_Schedule
        //                        .AnyAsync(x => x.Enable == true && (x.RoomId == itemModel.RoomId || x.TeacherId == itemModel.TeacherId)
        //                            && x.StartTime < et && x.EndTime > st);
        //                    if (check)
        //                        continue;
        //                    result.Add(new ScheduleCreates
        //                    {
        //                        StartTime = st,
        //                        EndTime = et,
        //                        RoomId = room.Id,
        //                        RoomName = room.Name,
        //                        RoomCode = room.Code,
        //                        TeacherId = teacher.UserInformationId,
        //                        TeacherName = teacher.FullName,
        //                        TeacherCode = teacher.UserCode,
        //                        Note = ""
        //                    });
        //                }
        //            }
        //        } while (result.Count() < curriculum.Lesson);
        //        return result;
        //    }
        //}
        public static async Task<List<ScheduleCreates>> GetLessonWhenCreate(LessonSearch itemModel)
        {
            using (var db = new lmsDbContext())
            {
                if (!itemModel.TimeModels.Any())
                    throw new Exception("Không tìm thấy ngày học");
                var curriculum = await db.tbl_Curriculum.SingleOrDefaultAsync(x => x.Id == itemModel.CurriculumId);
                if (curriculum == null)
                    throw new Exception("Không tìm thấy giáo trình");
                var branch = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == itemModel.BranchId);
                if (branch == null)
                    throw new Exception("Không tìm thấy trung tâm");
                var teacher = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == itemModel.TeacherId);
                if (teacher == null)
                    throw new Exception("Không tìm thấy giáo viên");
                var branchIds = teacher.BranchIds.Split(',');
                if (!branchIds.Any(x => x == branch.Id.ToString()))
                    throw new Exception("Giáo viên không thuộc trung tâm này");
                var teacherInProgram = await db.tbl_TeacherInProgram.FirstOrDefaultAsync(x => x.TeacherId == itemModel.TeacherId && x.ProgramId == curriculum.ProgramId && x.Enable == true);
                if (teacherInProgram == null)
                    throw new Exception("Giáo viên không được phép dạy chương trình này");
                var room = new tbl_Room { Id = 0, Name = "" };
                if (itemModel.RoomId != 0)
                {
                    room = await db.tbl_Room.SingleOrDefaultAsync(x => x.Id == itemModel.RoomId);
                    if (room == null)
                        throw new Exception("Không tìm thấy phòng");
                }
                var result = new List<ScheduleCreates>();
                var date = itemModel.StartDay.Value.AddDays(-1).Date;
                do
                {
                    date = date.AddDays(1);
                    var dayOff = await db.tbl_DayOff
                        .AnyAsync(x => x.Enable == true && x.sDate <= date && x.eDate >= date);
                    if (dayOff)
                        continue;
                    var teacherOff = await db.tbl_TeacherOff
                        .AnyAsync(x => x.Enable == true && x.StartTime <= date && x.EndTime >= date && x.Status == 2 && x.TeacherId == itemModel.TeacherId);
                    if (teacherOff)
                        continue;
                    foreach (var item in itemModel.TimeModels)
                    {
                        if (item.DayOfWeek == ((int)date.Date.DayOfWeek))
                        {
                            var studyTime = await db.tbl_StudyTime.SingleOrDefaultAsync(x => x.Id == item.StudyTimeId);
                            if (studyTime == null)
                                continue;
                            var stimes = studyTime.StartTime.Split(':');
                            DateTime st = new DateTime(date.Year, date.Month, date.Day, int.Parse(stimes[0]), int.Parse(stimes[1]), 0);
                            var etimes = studyTime.EndTime.Split(':');
                            DateTime et = new DateTime(date.Year, date.Month, date.Day, int.Parse(etimes[0]), int.Parse(etimes[1]), 0);

                            var checkRoom = await db.tbl_Schedule
                                .AnyAsync(x => x.Enable == true && x.RoomId == itemModel.RoomId
                                    && x.StartTime < et && x.EndTime > st);

                            var checkTeacher = await db.tbl_Schedule
                                .AnyAsync(x => x.Enable == true && x.TeacherId == itemModel.TeacherId
                                    && x.StartTime < et && x.EndTime > st);

                            result.Add(new ScheduleCreates
                            {
                                StartTime = st,
                                EndTime = et,
                                RoomId = checkRoom ? 0 : room.Id,
                                RoomName = checkRoom ? "" : room.Name,
                                RoomCode = checkRoom ? "" : room.Code,
                                TeacherId = checkTeacher ? 0 : teacher.UserInformationId,
                                TeacherName = checkTeacher ? "" : teacher.FullName,
                                TeacherCode = checkTeacher ? "" : teacher.UserCode,
                                Note = ""
                            });
                        }
                    }
                } while (result.Count() < curriculum.Lesson);
                return result;
            }
        }
        public class TeacherModel
        {
            public int TeacherId { get; set; }
            public string TeacherName { get; set; }
            public string TeacherCode { get; set; }
            public string TeacherAvatar { get; set; }
            public string Extension { get; set; }
        }
        public class TeacherSearch
        {
            public int BranchId { get; set; }
            public int ProgramId { get; set; }
        }
        public static async Task<List<TeacherModel>> GetTeacherWhenCreate(TeacherSearch itemModel)
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_TeacherWhenCreateClass @BranchId = {itemModel.BranchId}, @ProgramId = {itemModel.ProgramId}";
                var data = await db.Database.SqlQuery<TeacherModel>(sql).ToListAsync();
                return data;
            }
        }
        #region tối ưu chọn giáo viên tạo lịch
        //public class TeacherWhenCreateModel : TeacherModel
        //{
        //    /// <summary>
        //    /// true - phù hợp
        //    /// </summary>
        //    public bool Fit { get; set; } = true;
        //    /// <summary>
        //    /// Danh sách lịch bận
        //    /// </summary>
        //    public List<string> Conflicts { get; set; }
        //}
        //public class TeacherWhenCreateSearch
        //{
        //    public int BranchId { get; set; }
        //    public int ProgramId { get; set; }
        //    public int CurriculumId { get; set; }
        //    public DateTime? StartDay { get; set; }
        //    public List<TimeModel> TimeModels { get; set; }

        //}
        //public static async Task<List<TeacherModel>> GetTeacherWhenCreate(TeacherWhenCreateSearch itemModel)
        //{
        //    using (var db = new lmsDbContext())
        //    {
        //        string sql = $"Get_TeacherWhenCreateClass @BranchId = {itemModel.BranchId}, @ProgramId = {itemModel.ProgramId}";
        //        var data = await db.Database.SqlQuery<TeacherModel>(sql).ToListAsync();
        //        var curriculum = await db.tbl_Curriculum.SingleOrDefaultAsync(x => x.Id == itemModel.CurriculumId);
        //        if (curriculum == null)
        //            throw new Exception("Không tìm thấy giáo trình");
        //        int totalLesson = curriculum.Lesson ?? 0;
        //        var schedules = new List<ScheduleCreates>();



        //        var result = new List<TeacherWhenCreateModel>();

        //        return data;
        //    }
        //}
        #endregion
        public class CheckModel
        {
            /// <summary>
            /// false - trùng lịch
            /// </summary>
            public bool Value { get; set; }
            public string Note { get; set; }
        }
        /// <summary>
        /// true - trùng
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        public static async Task<CheckModel> CheckRoom(int scheduleId, int roomId, DateTime stime, DateTime etime)
        {
            using (var db = new lmsDbContext())
            {
                var schedule = await db.tbl_Schedule
                    .FirstOrDefaultAsync(x => x.Enable == true && x.RoomId == roomId
                    && x.Id != scheduleId && x.StartTime < etime && x.EndTime > stime
                        && x.StatusTutoring != 2  //Lớp dạy kèm bỏ qua buổi đã hủy
                        && x.StatusTutoring != 4
                        && x.StatusTutoring != 5
                        );
                var room = await db.tbl_Room.SingleOrDefaultAsync(x => x.Id == roomId);
                if (schedule != null)
                {
                    var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == schedule.ClassId);
                    return new CheckModel
                    {
                        Value = false,
                        Note = $"Phòng {room?.Name} trùng lịch với lớp {_class?.Name}, từ {schedule.StartTime} đến {schedule.EndTime}"
                    };
                }
                return new CheckModel
                {
                    Value = true,
                    Note = ""
                };
            }
        }
        public static async Task<CheckModel> CheckTeacher(int scheduleId, int teacherId, DateTime stime, DateTime etime)
        {
            using (var db = new lmsDbContext())
            {
                var schedule = await db.tbl_Schedule
                    .FirstOrDefaultAsync(x => x.Enable == true && x.TeacherId == teacherId
                    && x.Id != scheduleId && x.StartTime < etime && x.EndTime > stime
                        && x.StatusTutoring != 2 //Lớp dạy kèm bỏ qua buổi đã hủy
                        && x.StatusTutoring != 4
                        && x.StatusTutoring != 5
                        );
                var teacher = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == teacherId);
                if (schedule != null)
                {
                    var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == schedule.ClassId);
                    if (_class.Type == 3)// lớp dạy kèm kiểm tra giáo viên mở lịch
                    {
                        var checkScheduleAvailable = await db.tbl_ScheduleAvailable
                            .AnyAsync(x => x.StartTime <= stime && x.EndTime >= etime && x.Enable == true);
                        if (!checkScheduleAvailable)
                            return new CheckModel
                            {
                                Value = false,
                                Note = "Giáo viên không mở lịch thời gian này"
                            };
                    }
                    return new CheckModel
                    {
                        Value = false,
                        Note = $"Giáo viên {teacher?.FullName} trùng lịch với lớp {_class?.Name}, từ {schedule.StartTime} đến {schedule.EndTime}"
                    };
                }
                return new CheckModel
                {
                    Value = true,
                    Note = ""
                };
            }
        }
        public class TeacherAvailableModel
        {
            public int TeacherId { get; set; }
            public string TeacherName { get; set; }
            public string TeacherCode { get; set; }
            public bool Fit { get; set; }
            public string Note { get; set; }
        }
        public class TeacherAvailableSearch
        {
            public int ScheduleId { get; set; } = 0;
            [Required(ErrorMessage = "Vui lòng chọn trung tâm")]
            public int? BranchId { get; set; }
            [Required(ErrorMessage = "Vui lòng chọn giáo trình")]
            public int? CurriculumId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }
        public static async Task<List<TeacherAvailableModel>> GetTeacherAvailable(TeacherAvailableSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                var branch = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == baseSearch.BranchId);
                if (branch == null)
                    throw new Exception("Không tìm thấy trung tâm");
                var curriculum = await db.tbl_Curriculum.SingleOrDefaultAsync(x => x.Id == baseSearch.CurriculumId);
                if (curriculum == null)
                    throw new Exception("Không tìm thấy giáo trình");
                var result = new List<TeacherAvailableModel>();
                var teachers = await db.tbl_TeacherInProgram.Where(x => x.ProgramId == curriculum.ProgramId && x.Enable == true).ToListAsync();
                if (teachers.Any())
                {
                    foreach (var item in teachers)
                    {
                        var teacher = await db.tbl_UserInformation
                            .FirstOrDefaultAsync(x => x.UserInformationId == item.TeacherId && x.Enable == true);
                        if (teacher != null)
                        {
                            var checkBranch = teacher.BranchIds.Split(',').Any(x => x == baseSearch.BranchId.ToString());
                            if (checkBranch)
                            {
                                var check = await CheckTeacher(baseSearch.ScheduleId, item.TeacherId.Value, baseSearch.StartTime, baseSearch.EndTime);
                                result.Add(new TeacherAvailableModel
                                {
                                    Fit = check.Value,
                                    Note = check.Note,
                                    TeacherCode = teacher.UserCode,
                                    TeacherId = teacher.UserInformationId,
                                    TeacherName = teacher.FullName
                                });
                            }
                        }
                    }
                }
                return result;
            }
        }
        public class RoomAvailableModel
        {
            public int RoomId { get; set; }
            public string RoomName { get; set; }
            public string RoomCode { get; set; }
            public bool Fit { get; set; }
            public string Note { get; set; }
        }
        public class RoomAvailableSearch
        {
            public int ScheduleId { get; set; } = 0;
            [Required(ErrorMessage = "Vui lòng chọn trung tâm")]
            public int? BranchId { get; set; }
            [Required(ErrorMessage = "Vui lòng chọn giáo trình")]
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }
        public static async Task<List<RoomAvailableModel>> GetRoomAvailable(RoomAvailableSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                var branch = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == baseSearch.BranchId);
                if (branch == null)
                    throw new Exception("Không tìm thấy trung tâm");
                var rooms = await db.tbl_Room.Where(x => x.BranchId == baseSearch.BranchId && x.Enable == true).ToListAsync();
                var result = new List<RoomAvailableModel>();
                if (rooms.Any())
                {
                    foreach (var item in rooms)
                    {
                        var check = await CheckRoom(baseSearch.ScheduleId, item.Id, baseSearch.StartTime, baseSearch.EndTime);
                        result.Add(new RoomAvailableModel
                        {
                            Fit = check.Value,
                            Note = check.Note,
                            RoomCode = item.Code,
                            RoomId = item.Id,
                            RoomName = item.Name
                        });
                    }
                }
                return result;
            }
        }
        public static async Task<tbl_Class> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_Class> Insert(ClassCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var model = new tbl_Class(itemModel);
                        var branch = await BranchService.GetById(model.BranchId.Value);
                        if (branch == null)
                            throw new Exception("Không tìm thấy trung tâm");
                        var grade = await GradeService.GetById(model.GradeId.Value);
                        if (grade == null)
                            throw new Exception("Không tìm thấy chuyên môn");
                        var program = await ProgramService.GetById(model.ProgramId.Value);
                        if (program == null)
                            throw new Exception("Không tìm thấy chương trình");
                        var curriculum = await CurriculumService.GetById(model.CurriculumId.Value);
                        if (curriculum == null)
                            throw new Exception("Không tìm thấy giáo trình");
                        var teacher = await UserInformation.GetById(model.TeacherId.Value);
                        if (teacher == null)
                            throw new Exception("Vui lòng chọn đầy đủ giáo viên");

                        model.CreatedBy = model.ModifiedBy = user.FullName;
                        db.tbl_Class.Add(model);
                        await db.SaveChangesAsync();
                        if (itemModel.schedules.Any())
                        {
                            DateTime endDay = model.StartDay.Value;
                            foreach (var item in itemModel.schedules)
                            {
                                var checkRoom = await CheckRoom(0, item.RoomId.Value, item.StartTime, item.EndTime);
                                if (!checkRoom.Value && item.RoomId.Value != 0)
                                    throw new Exception(checkRoom.Note);

                                var itemTeacher = await UserInformation.GetById(item.TeacherId.Value);
                                if (itemTeacher == null)
                                    throw new Exception("Vui lòng chọn đầy đủ giáo viên");

                                var checkTecaher = await CheckTeacher(0, item.TeacherId.Value, item.StartTime, item.EndTime);
                                if (!checkTecaher.Value)
                                    throw new Exception(checkTecaher.Note);
                                var schedule = new tbl_Schedule
                                {
                                    BranchId = itemModel.BranchId.Value,
                                    ClassId = model.Id,
                                    CreatedBy = user.FullName,
                                    CreatedOn = DateTime.Now,
                                    Enable = true,
                                    EndTime = item.EndTime,
                                    ModifiedBy = user.FullName,
                                    ModifiedOn = DateTime.Now,
                                    RoomId = item.RoomId.Value,
                                    StartTime = item.StartTime,
                                    TeacherAttendanceId = 0,
                                    TeacherId = item.TeacherId.Value,
                                    TeachingFee = itemModel.TeachingFee,
                                    Note = item.Note,
                                };
                                endDay = item.EndTime;
                                db.tbl_Schedule.Add(schedule);
                            }
                            model.EndDay = endDay;
                            await db.SaveChangesAsync();
                        }

                        //thông báo cho giáo viên được chỉ định dạy lớp này
                        tbl_Notification noti = new tbl_Notification()
                        {
                            Title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " thông báo lớp học mới",
                            Content = "Bạn được chỉ định dạy lớp " + itemModel.Name,
                            UserId = teacher.UserInformationId,
                            IsSeen = false,
                            CreatedBy = user.FullName,
                            CreatedOn = DateTime.Now,
                            Enable = true
                        };
                        db.tbl_Notification.Add(noti);
                        await db.SaveChangesAsync();
                        Thread thread1 = new Thread(() =>
                        {
                            AssetCRM.OneSignalPushNotifications(noti.Title, noti.Content, teacher.OneSignal_DeviceId);
                        });
                        thread1.Start();
                        //AssetCRM.SendMail(teacher.Email, noti.Title, noti.Content);
                        //thông báo cho học vụ hỗ trợ lớp này
                        if (itemModel.AcademicId.HasValue)
                        {
                            tbl_UserInformation academic = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == itemModel.AcademicId);

                            noti.Title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " thông báo lớp học mới";
                            noti.Content = "Bạn được chỉ định hỗ trợ lớp " + itemModel.Name;
                            noti.UserId = academic.UserInformationId;
                            db.tbl_Notification.Add(noti);
                            await db.SaveChangesAsync();
                            Thread thread2 = new Thread(() =>
                            {
                                AssetCRM.OneSignalPushNotifications(noti.Title, noti.Content, academic.OneSignal_DeviceId);
                            });
                            thread2.Start();
                            //AssetCRM.SendMail(academic.Email, noti.Title, noti.Content);
                        }
                        //thông báo cho admin
                        List<tbl_UserInformation> admins = await db.tbl_UserInformation.Where(x => x.RoleId == (int)RoleEnum.admin && x.Enable == true).ToListAsync();
                        noti.Title = "Lớp học mới được tạo";
                        noti.Content = user.FullName + " đã tạo lớp " + model.Name + ". Vui lòng kiểm tra.";
                        foreach (var ad in admins)
                        {
                            noti.UserId = ad.UserInformationId;
                            db.tbl_Notification.Add(noti);
                            await db.SaveChangesAsync();
                            Thread thread3 = new Thread(() =>
                            {
                                AssetCRM.OneSignalPushNotifications(noti.Title, noti.Content, ad.OneSignal_DeviceId);
                            });
                            thread3.Start();

                            //AssetCRM.SendMail(ad.Email, noti.Title, noti.Content);
                        }

                        //Tạo giáo trình cho lớp học
                        if (itemModel.CurriculumId != null)
                        {
                            var curriculumInClass = new tbl_CurriculumInClass
                            {
                                ClassId = model.Id,
                                CurriculumId = model.CurriculumId,
                                Name = curriculum.Name,
                                IsComplete = false,
                                CompletePercent = 0,
                                CreatedBy = user.FullName,
                                CreatedOn = DateTime.Now,
                                Enable = true
                            };
                            db.tbl_CurriculumInClass.Add(curriculumInClass);
                            await db.SaveChangesAsync();
                            //Thêm các chương của giáo trình
                            var curriculumDetails = await db.tbl_CurriculumDetail.Where(x => x.Enable == true && x.CurriculumId == curriculumInClass.CurriculumId).ToListAsync();
                            if (curriculumDetails.Any())
                            {
                                foreach (var itemCurDetail in curriculumDetails)
                                {
                                    var curDetailInClass = new tbl_CurriculumDetailInClass
                                    {
                                        CurriculumIdInClass = curriculumInClass.Id,
                                        CurriculumDetailId = itemCurDetail.Id,
                                        IsComplete = false,
                                        Name = itemCurDetail.Name,
                                        Index = itemCurDetail.Index,
                                        CompletePercent = 0,
                                        Enable = true,
                                        CreatedBy = user.FullName,
                                        CreatedOn = DateTime.Now,
                                        ModifiedBy = user.FullName,
                                        ModifiedOn = DateTime.Now,
                                    };
                                    db.tbl_CurriculumDetailInClass.Add(curDetailInClass);
                                    await db.SaveChangesAsync();
                                    //Thêm cái file vào chương
                                    var file = await db.tbl_FileInCurriculumDetail.Where(x => x.Enable == true && x.CurriculumDetailId == itemCurDetail.Id).ToListAsync();
                                    if (file.Any())
                                    {
                                        foreach (var itemFile in file)
                                        {
                                            var fileCreate = new tbl_FileCurriculumInClass
                                            {
                                                CurriculumDetailId = curDetailInClass.Id,
                                                FileCurriculumId = itemFile.Id,
                                                IsComplete = false,
                                                IsHide = false,
                                                FileName = itemFile.FileName,
                                                FileUrl = itemFile.FileUrl,
                                                Index = itemFile.Index,
                                                ClassId = model.Id,
                                                Enable = true,
                                                CreatedBy = user.FullName,
                                                CreatedOn = DateTime.Now,
                                                ModifiedBy = user.FullName,
                                                ModifiedOn = DateTime.Now
                                            };
                                            db.tbl_FileCurriculumInClass.Add(fileCreate);
                                            await db.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                        }
                        tran.Commit();
                        return model;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }
        public static async Task<tbl_Class> Update(ClassUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == itemModel.Id);
                if (entity == null)
                    throw new Exception("Không tìm thấy lớp này");
                //cập nhật trạng thái những học viên trong lớp
                if (entity.Status == 3 && itemModel.Status.HasValue && itemModel.Status != 3)
                {
                    var studentIds = await db.tbl_StudentInClass.Where(x => x.ClassId == entity.Id && x.Enable == true).Select(x => x.StudentId).ToListAsync();
                    if (studentIds.Any())
                    {
                        foreach (var s in studentIds)
                        {
                            var student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == s);
                            student.LearningStatus = 2;
                            student.LearningStatusName = "Đang học";
                            await db.SaveChangesAsync();
                            //cập nhật trạng thái "Đang học" lộ trình học của học viên -> lộ trình có chương trình tương đương với chương trình của lớp học
                            List<tbl_StudyRoute> studyRoute = await db.tbl_StudyRoute
                                       .Where(x =>
                                           x.StudentId == s
                                           && x.ProgramId == entity.ProgramId
                                           && x.Status == (int)StudyRouteStatus.ChuaHoc
                                           && x.Enable == true).ToListAsync();
                            if (studyRoute.Any())
                            {
                                studyRoute.ForEach(x =>
                                {
                                    x.Status = (int)StudyRouteStatus.DangHoc;
                                    x.StatusName = ListStudyRouteStatus().Where(y => y.Key == x.Status).FirstOrDefault().Value;
                                });
                                await db.SaveChangesAsync();
                            }
                        }
                    }
                }
                else if (itemModel.Status == 3)
                {
                    var studentIds = await db.tbl_StudentInClass.Where(x => x.ClassId == entity.Id && x.Enable == true).Select(x => x.StudentId).ToListAsync();
                    if (studentIds.Any())
                    {
                        foreach (var s in studentIds)
                        {
                            var classIds = await db.tbl_StudentInClass.Where(x => x.StudentId == s && x.Enable == true && x.ClassId != entity.Id).Select(x => x.ClassId).ToListAsync();
                            var checkClass = await db.tbl_Class.Where(x => classIds.Contains(x.Id) && x.Status != 3 && x.Enable == true).AnyAsync();
                            if (!checkClass)
                            {
                                var student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == s);
                                student.LearningStatus = 3;
                                student.LearningStatusName = "Học xong";
                                await db.SaveChangesAsync();
                            }
                            //cập nhật trạng thái "Hoàn thành" lộ trình học của học viên -> lộ trình có chương trình tương đương với chương trình của lớp học
                            List<tbl_StudyRoute> studyRoute = await db.tbl_StudyRoute
                                .Where(x =>
                                    x.StudentId == s
                                    && x.ProgramId == entity.ProgramId
                                    && x.Status == (int)StudyRouteStatus.ChuaHoc
                                    && x.Status == (int)StudyRouteStatus.DangHoc
                                    && x.Enable == true).ToListAsync();
                            if (studyRoute.Any())
                            {
                                studyRoute.ForEach(x =>
                                {
                                    x.Status = (int)StudyRouteStatus.HoanThanh;
                                    x.StatusName = ListStudyRouteStatus().Where(y => y.Key == x.Status).FirstOrDefault().Value;
                                });
                                await db.SaveChangesAsync();
                            }

                        }
                    }
                }
                //nếu đổi giáo viên thì thông báo cho giáo viên mới
                if (itemModel.TeacherId.HasValue && entity.TeacherId != itemModel.TeacherId)
                {
                    tbl_UserInformation teacher = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == itemModel.TeacherId);
                    tbl_Notification notification = new tbl_Notification()
                    {
                        Title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " thông báo lớp học mới",
                        Content = "Bạn được chỉ định dạy lớp " + itemModel.Name,
                        UserId = teacher.UserInformationId,
                        IsSeen = false,
                        CreatedBy = user.FullName,
                        CreatedOn = DateTime.Now,
                        Enable = true
                    };
                    db.tbl_Notification.Add(notification);
                    await db.SaveChangesAsync();
                    Thread thread1 = new Thread(() =>
                    {
                        AssetCRM.OneSignalPushNotifications(notification.Title, notification.Content, teacher.OneSignal_DeviceId);
                        AssetCRM.SendMail(teacher.Email, notification.Title, notification.Content);
                    });
                    thread1.Start();
                }
                //nếu đổi học vụ thì thông báo cho học vụ mới
                if (itemModel.TeacherId.HasValue && entity.AcademicId != itemModel.AcademicId)
                {
                    tbl_UserInformation academic = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == itemModel.AcademicId);
                    tbl_Notification notification = new tbl_Notification()
                    {
                        Title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " thông báo lớp học mới",
                        Content = "Bạn được chỉ định hỗ trợ lớp " + itemModel.Name,
                        UserId = academic.UserInformationId,
                        IsSeen = false,
                        CreatedBy = user.FullName,
                        CreatedOn = DateTime.Now,
                        Enable = true
                    };
                    db.tbl_Notification.Add(notification);
                    await db.SaveChangesAsync();
                    Thread thread2 = new Thread(() =>
                    {
                        AssetCRM.OneSignalPushNotifications(notification.Title, notification.Content, academic.OneSignal_DeviceId);
                        AssetCRM.SendMail(academic.Email, notification.Title, notification.Content);
                    });
                    thread2.Start();
                    
                }
                entity.Name = itemModel.Name ?? entity.Name;
                entity.Thumbnail = itemModel.Thumbnail ?? entity.Thumbnail;
                entity.Price = itemModel.Price ?? entity.Price;
                entity.Status = itemModel.Status ?? entity.Status;
                entity.StatusName = itemModel.StatusName ?? entity.StatusName;
                entity.AcademicId = itemModel.AcademicId ?? entity.AcademicId;
                entity.TeacherId = itemModel.TeacherId ?? entity.TeacherId;
                entity.MaxQuantity = itemModel.MaxQuantity ?? entity.MaxQuantity;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                return entity;
            }
        }
        /// <summary>
        /// true - lớp đang học (có học viện trong lớp)
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckExistStudentInClass(int id)
        {
            using (var db = new lmsDbContext())
            {
                var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == id);
                if (_class == null)
                    throw new Exception("Không tìm thấy lớp");
                var studentInClass = await db.tbl_StudentInClass.AnyAsync(x => x.ClassId == id && x.Enable == true);
                return studentInClass;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == id);
                if (_class == null)
                    throw new Exception("Không tìm thấy lớp");
                _class.Enable = false;
                await db.SaveChangesAsync();
                var schedules = await db.tbl_Schedule.Where(x => x.ClassId == id && x.Enable == true).ToListAsync();
                if (schedules.Any())
                {
                    foreach (var item in schedules)
                    {
                        item.Enable = false;
                    }
                    await db.SaveChangesAsync();
                }
            }
        }
        public static async Task<AppDomainResult> GetAll(ClassSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new ClassSearch();
                string myBranchIds = "";
                if (user.RoleId != ((int)RoleEnum.admin) && user.RoleId != ((int)RoleEnum.student) && user.RoleId != ((int)RoleEnum.parents))
                    myBranchIds = user.BranchIds;
                int uid = 0;
                if (user.RoleId == ((int)RoleEnum.student))
                    uid = user.UserInformationId;
                if (user.RoleId == ((int)RoleEnum.parents))
                    uid = baseSearch.StudentId;

                string sql = $"Get_Class @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@Name = N'{baseSearch.Name ?? ""}'," +
                    $"@BranchIds = N'{baseSearch.BranchIds ?? ""}'," +
                    $"@Types = N'{baseSearch.Types ?? ""}'," +
                    $"@Status = N'{baseSearch.Status ?? ""}'," +
                    $"@MyBranchIds = N'{myBranchIds ?? ""}'," +
                    $"@Uid = N'{uid}'," +
                    $"@Sort = N'{baseSearch.Sort}'," +
                    $"@SortType = N'{(baseSearch.SortType ? 1 : 0)}'";
                var data = await db.Database.SqlQuery<Get_Class>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_Class(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public static async Task AutoUpdateStatus()
        {
            using (var db = new lmsDbContext())
            {
                var classs = await db.tbl_Class.Where(x => x.Status != 3 && x.Enable == true && x.StartDay.HasValue && x.EndDay.HasValue).ToListAsync();
                if (classs.Any())
                {
                    foreach (var item in classs)
                    {
                        DateTime endDay = item.EndDay.Value.AddDays(1);
                        if (item.Status == 1 && DateTime.Now >= item.StartDay.Value.Date)
                        {
                            item.Status = 2;
                            item.StatusName = "Đang diễn ra";

                        }
                        else if (item.Status == 2 && DateTime.Now > endDay)
                        {
                            item.Status = 3;
                            item.StatusName = "Kết thúc";
                            //cập nhật trạng thái những học viên trong lớp
                            var studentIds = await db.tbl_StudentInClass.Where(x => x.ClassId == item.Id && x.Enable == true).Select(x => x.StudentId).ToListAsync();
                            if (studentIds.Any())
                            {
                                foreach (var s in studentIds)
                                {
                                    var classIds = await db.tbl_StudentInClass.Where(x => x.StudentId == s && x.Enable == true && x.ClassId != item.Id).Select(x => x.ClassId).ToListAsync();
                                    var checkClass = await db.tbl_Class.Where(x => classIds.Contains(x.Id) && x.Status != 3 && x.Enable == true).AnyAsync();
                                    var student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == s);
                                    if (!checkClass)
                                    {
                                        student.LearningStatus = 3;
                                        student.LearningStatusName = "Học xong";
                                        await db.SaveChangesAsync();
                                    }
                                    //cập nhật trạng thái lộ trình học của học viên -> hoàn thành lộ trình có chương trình tương đương với chương trình của lớp học
                                    List<tbl_StudyRoute> studyRoute = await db.tbl_StudyRoute
                                        .Where(x =>
                                            x.StudentId == s
                                            && x.ProgramId == item.ProgramId
                                            && x.Status == (int)StudyRouteStatus.ChuaHoc
                                            && x.Status == (int)StudyRouteStatus.DangHoc
                                            && x.Enable == true).ToListAsync();
                                    if (studyRoute.Any())
                                    {
                                        studyRoute.ForEach(x =>
                                        {
                                            x.Status = (int)StudyRouteStatus.HoanThanh;
                                            x.StatusName = ListStudyRouteStatus().Where(y => y.Key == x.Status).FirstOrDefault().Value;
                                        });
                                        await db.SaveChangesAsync();
                                    }
                                    // thông báo cho tư vấn viên của mỗi học viên
                                    Thread sendNoti = new Thread(async () =>
                                    {
                                        await NotificationService.Send(new tbl_Notification
                                        {
                                            Title = "Học viên đã hoàn thành khóa học",
                                            Content = "Học viên " + student.FullName + " đã hoàn thành khóa học " + item.Name + ". Vui lòng tư vấn thêm cho học viên.",
                                            UserId = student.SaleId
                                        }, new tbl_UserInformation { FullName = "Tự động" });
                                    });
                                    sendNoti.Start();
                                }
                            }
                        }
                    }
                    await db.SaveChangesAsync();
                }
            }
        }
        public static async Task<AppDomainResult> GetRollUpTeacher(RollUpTeacherSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new RollUpTeacherSearch();
                int teacherId = 0;
                if (user.RoleId == ((int)RoleEnum.teacher))
                    teacherId = user.UserInformationId;
                string sql = $"Get_RollUpTeacher @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@ClassId = {baseSearch.ClassId}," +
                    $"@TeacherId = {teacherId}";
                var data = await db.Database.SqlQuery<Get_RollUpTeacher>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new RollUpTeacherModel(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public static async Task RollUpTeacher(int scheduleId)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Schedule.SingleOrDefaultAsync(x => x.Id == scheduleId);
                if (entity == null)
                    throw new Exception("Không tìm thấy lịch");
                if (entity.TeacherAttendanceId != 0)
                    entity.TeacherAttendanceId = 0;
                else
                    entity.TeacherAttendanceId = entity.TeacherId;
                await db.SaveChangesAsync();
            }
        }
        public class Get_ScheduleInDateNow
        {
            public string FullName { get; set; }
            public string UserCode { get; set; }
            public int ClassId { get; set; }
            public string ClassName { get; set; }
            public int BranchId { get; set; }
            public string BranchName { get; set; }
            public int RoomId { get; set; }
            public string RoomName { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public int TeacherId { get; set; }
            public string TeacherName { get; set; }
        }
        public static async Task<List<Get_ScheduleInDateNow>> GetScheduleInDateNow(int branchId)
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_ScheduleInDateNow @BranchId = {branchId}";
                var data = await db.Database.SqlQuery<Get_ScheduleInDateNow>(sql).ToListAsync();
                return data;
            }
        }
        #region Dạy kèm

        public class TeacherTutoringAvailableSearch : SearchOptions
        {
            public int? CurriculumId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }
        public class TeacherTutoringAvailableModel
        {
            public int TeacherId { get; set; }
            public string TeacherName { get; set; }
            public string TeacherCode { get; set; }
            public string Avatar { get; set; }
            public string Extension { get; set; }
            public bool Fit { get; set; }
            public string Note { get; set; }
            public double Rate { get; set; }
        }
        public static async Task<AppDomainResult> GetTeacherTutoringAvailable(TeacherTutoringAvailableSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                var curriculum = await db.tbl_Curriculum.SingleOrDefaultAsync(x => x.Id == baseSearch.CurriculumId);
                if (curriculum == null)
                    throw new Exception("Không tìm thấy giáo trình");
                var result = new List<TeacherTutoringAvailableModel>();
                var teachers = await db.tbl_TeacherInProgram.Where(x => x.ProgramId == curriculum.ProgramId && x.Enable == true).Select(x => new { x.TeacherId }).ToListAsync();
                if (teachers.Any())
                {
                    foreach (var item in teachers)
                    {
                        var teacher = await db.tbl_UserInformation
                            .FirstOrDefaultAsync(x => x.UserInformationId == item.TeacherId && x.Enable == true);
                        if (teacher != null)
                        {
                            var check = await CheckTeacher(0, item.TeacherId.Value, baseSearch.StartTime, baseSearch.EndTime);
                            var checkTimeAvailable = await db.tbl_ScheduleAvailable
                                .AnyAsync(x => x.TeacherId == teacher.UserInformationId
                                && x.Enable == true
                                && x.StartTime <= baseSearch.StartTime
                                && x.EndTime >= baseSearch.EndTime
                                );

                            double rate = 0;
                            var rates = await db.tbl_Schedule
                                .Where(x => x.TeacherAttendanceId == teacher.UserInformationId && x.Enable == true && x.RateTeacher != 0)
                                .Select(x => x.RateTeacher).ToListAsync();
                            if (rates.Any())
                                rate = Math.Round(rates.Average(), 1);

                            result.Add(new TeacherTutoringAvailableModel
                            {
                                Fit = !checkTimeAvailable ? false : check.Value,
                                Note = !checkTimeAvailable ? "Giáo viên không mở lịch trong thời gian này" : check.Note,
                                TeacherCode = teacher.UserCode,
                                TeacherId = teacher.UserInformationId,
                                TeacherName = teacher.FullName,
                                Avatar = teacher.Avatar,
                                Extension = teacher.Extension,
                                Rate = rate
                            });
                        }
                    }
                }
                result = result.OrderByDescending(x => x.Fit).ThenByDescending(x => x.Rate).ToList();
                int totalRow = result.Count();
                return new AppDomainResult
                {
                    Data = result.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList(),
                    TotalRow = totalRow,
                    Success = true
                };
            }
        }
        public class TutoringConfigModel
        {
            /// <summary>
            /// OpenTutoring - Đặt lịch trước bao nhiêu tiếng
            /// CancelTutoring - Hủy lịch trước bao nhiêu tiếng
            /// </summary>
            public string Code { get; set; }
            public string Name { get; set; }
            public int Value { get; set; }
        }
        public static async Task<TutoringConfigModel> TutoringConfig(TutoringConfigModel itemModel)
        {
            using (var db = new lmsDbContext())
            {
                if (itemModel.Code != "OpenTutoring" && itemModel.Code != "CancelTutoring")
                    throw new Exception("Mã không phù hợp");
                var data = await db.tbl_Config.FirstOrDefaultAsync(x => x.Code == itemModel.Code);
                if (data == null)
                {
                    data = new tbl_Config
                    {
                        Code = itemModel.Code,
                        Name = itemModel.Code == "OpenTutoring" ? "Đặt lịch trước bao nhiêu tiếng" : "Hủy lịch trước bao nhiêu tiếng",
                        Value = itemModel.Value.ToString(),
                    };
                    db.tbl_Config.Add(data);
                }
                else
                {
                    data.Value = itemModel.Value.ToString();
                }
                await db.SaveChangesAsync();
                return new TutoringConfigModel
                {
                    Code = data.Code,
                    Name = data.Name,
                    Value = int.Parse(data.Value)
                };
            }
        }
        public static async Task<List<TutoringConfigModel>> GetTutoringConfig()
        {
            using (var db = new lmsDbContext())
            {
                var result = new List<TutoringConfigModel>();
                var openTutoring = await db.tbl_Config.FirstOrDefaultAsync(x => x.Code == "OpenTutoring");
                if (openTutoring != null)
                    result.Add(new TutoringConfigModel
                    {
                        Code = openTutoring.Code,
                        Name = openTutoring.Name,
                        Value = int.Parse(openTutoring.Value)
                    });
                else
                    result.Add(new TutoringConfigModel
                    {
                        Code = "OpenTutoring",
                        Name = "Đặt lịch trước bao nhiêu tiếng",
                        Value = 0
                    });

                var cancelTutoring = await db.tbl_Config.FirstOrDefaultAsync(x => x.Code == "CancelTutoring");
                if (cancelTutoring != null)
                    result.Add(new TutoringConfigModel
                    {
                        Code = cancelTutoring.Code,
                        Name = cancelTutoring.Name,
                        Value = int.Parse(cancelTutoring.Value)
                    });
                else
                    result.Add(new TutoringConfigModel
                    {
                        Code = "CancelTutoring",
                        Name = "Hủy lịch trước bao nhiêu tiếng",
                        Value = 0
                    });
                return result;
            }
        }
        public class TutoringCurriculumModel
        {
            /// <summary>
            /// Số buổi đã đặt
            /// </summary>
            public int Booked { get; set; }
            /// <summary>
            /// Số buổi
            /// </summary>
            public int? Lesson { get; set; }
            /// <summary>
            /// Thời gian
            /// </summary>
            public int? Time { get; set; }
        }
        /// <summary>
        /// Lấy chi tiết đã học bao nhiêu buổi và 
        /// </summary>
        /// <returns></returns>
        public static async Task<TutoringCurriculumModel> TutoringCurriculum(int classId)
        {
            using (var db = new lmsDbContext())
            {
                var result = new TutoringCurriculumModel
                {
                    Booked = 0,
                    Lesson = 0,
                    Time = 0,
                };
                var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == classId);
                if (_class == null)
                    return result;
                var curriculum = await db.tbl_Curriculum.SingleOrDefaultAsync(x => x.Id == _class.CurriculumId);
                if (curriculum == null)
                    return result;

                result.Lesson = curriculum.Lesson;
                result.Time = curriculum.Time;
                result.Booked = await db.tbl_Schedule.Where(x => x.ClassId == classId
                        && x.Enable == true
                        && x.StatusTutoring != 2
                        && x.StatusTutoring != 4
                        && x.StatusTutoring != 5).CountAsync();
                return result;
            }
        }
        #endregion
    }
}