using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;
namespace LMS_Project.Services
{
    public class NotificationInClassService
    {
        public static async Task<tbl_NotificationInClass> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_NotificationInClass.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_NotificationInClass> Insert(NotificationInClassCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var _class = await db.tbl_Class
                    .AnyAsync(x => x.Id == itemModel.ClassId);
                var model = new tbl_NotificationInClass(itemModel);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_NotificationInClass.Add(model);
                await db.SaveChangesAsync();
                var studentIds = await db.tbl_StudentInClass
                    .Where(x => x.ClassId == itemModel.ClassId && x.Enable == true).Select(x => x.StudentId).ToListAsync();
                if (studentIds.Any())
                {
                    Thread send = new Thread(async () =>
                    {
                        foreach (var item in studentIds)
                        {
                            await NotificationService.Send(new tbl_Notification
                            {
                                UserId = item,
                                Content = model.Content,
                                Title = model.Title
                            }, user, model.Content, itemModel.IsSendMail);
                        }
                    });
                    send.Start();
                }
                return model;
            }
        }
        public static async Task<AppDomainResult> GetAll(NotificationInClassSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new NotificationInClassSearch();

                var l = await db.tbl_NotificationInClass.Where(x => x.Enable == true && x.ClassId == baseSearch.ClassId).OrderByDescending(x => x.Id).ToListAsync();
                if (!l.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };

                int totalRow = l.Count();
                var result = l.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }
    }
}