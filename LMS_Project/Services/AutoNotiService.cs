using LMS_Project.LMS;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace LMS_Project.Services
{
    public class AutoNotiService
    {
        public static async Task AutoNotiClassComing()
        {
            using (var db = new lmsDbContext())
            {
                //var timeNow = DateTime.Now;
                var timeAfter1Hour = DateTime.Now.AddHours(1);
                //danh sách schedule 1 tiếng nữa vô học
                List<tbl_Schedule> schedule = await db.tbl_Schedule.Where(x => x.Enable == true && x.Announced == false && x.StartTime <= timeAfter1Hour).ToListAsync();
                if (schedule.Any())
                {
                    string title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"];

                    tbl_UserInformation user = new tbl_UserInformation
                    {
                        FullName = "Tự động"
                    };
                    int scheduleCount = schedule.Count;
                    for (int i = 0; i < scheduleCount; i++)
                    {
                        string startTime = (schedule[i].StartTime ?? timeAfter1Hour).ToString("dd/MM/yyyy HH:mm");
                        int classId = schedule[i].ClassId ?? 0;
                        tbl_Class _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == classId);
                        if (_class == null)
                        {
                            continue;
                        }
                        string className = _class.Name;

                        //thông báo cho giáo viên của lớp
                        int teacherId = _class.TeacherId ?? 0;
                        if (teacherId > 0)
                        {
                            Thread sendTeacher = new Thread(async () =>
                            {
                                tbl_Notification notiTeacher = new tbl_Notification();
                                notiTeacher.Title = title + " thông báo lịch dạy";
                                notiTeacher.Content = "Bạn có lịch dạy lớp " + className + " vào lúc " + startTime + ". Vui lòng kiểm tra.";
                                notiTeacher.UserId = teacherId;
                                await NotificationService.Send(notiTeacher, user, "", true);
                            });
                            sendTeacher.Start();
                        }

                        //thông báo cho học viên của mỗi schedule
                        List<tbl_UserInformation> studentInClasses = await (from sic in db.tbl_StudentInClass
                                                                            join u in db.tbl_UserInformation on sic.StudentId equals u.UserInformationId into list
                                                                            from l in list
                                                                            where sic.Enable == true && l.Enable == true && classId == sic.ClassId
                                                                            select l).ToListAsync();

                        int studentCount = studentInClasses.Count;
                        if (studentInClasses.Any())
                        {
                            Thread sendStudent = new Thread(async () =>
                            {
                                for (int j = 0; j < studentCount; j++)
                                {
                                    tbl_Notification notiStudent = new tbl_Notification();
                                    notiStudent.Title = title + " thông báo lịch học";
                                    notiStudent.Content = "Bạn có lịch học lớp " + className + " vào lúc " + startTime + ". Vui lòng kiểm tra.";
                                    notiStudent.UserId = studentInClasses[j].UserInformationId;
                                    await NotificationService.Send(notiStudent, user, "", true);
                                    //nếu học viên này có phụ huynh thì thông báo cho phụ huynh
                                    if (studentInClasses[j].ParentId != null)
                                    {
                                        tbl_Notification notiParent = new tbl_Notification();
                                        notiParent.Title = title + " thông báo lịch học";
                                        notiParent.Content = "Học viên " + studentInClasses[j].FullName + " có lịch học lớp " + className + " vào lúc " + startTime + ". Vui lòng kiểm tra.";
                                        notiParent.UserId = studentInClasses[j].ParentId;
                                        await NotificationService.Send(notiParent, user, "", true);
                                    }
                                }
                            });
                            sendStudent.Start();
                        }
                        schedule[i].Announced = true;
                    }
                    await db.SaveChangesAsync();
                }
            }
        }

    }
}