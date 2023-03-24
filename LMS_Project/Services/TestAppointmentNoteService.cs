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
    public class TestAppointmentNoteService
    {
        public static async Task<tbl_TestAppointmentNote> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_TestAppointmentNote.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_TestAppointmentNote> Insert(TestAppointmentNoteCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var model = new tbl_TestAppointmentNote(itemModel);
                var testAppointment = await db.tbl_TestAppointment.AnyAsync(x => x.Id == itemModel.TestAppointmentId.Value);
                if (!testAppointment)
                    throw new Exception("Không tìm thấy lịch hẹn");
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_TestAppointmentNote.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_TestAppointmentNote.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public static async Task<AppDomainResult> GetAll(TestAppointmentNoteSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new TestAppointmentNoteSearch();

                var l = await db.tbl_TestAppointmentNote.Where(x => x.Enable == true && x.TestAppointmentId == baseSearch.TestAppointmentId
                ).OrderByDescending(x => x.Id).ToListAsync();
                if (!l.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                int totalRow = l.Count();
                var result = l.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }
    }
}