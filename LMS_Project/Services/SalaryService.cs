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
    public class SalaryService
    {
        public static async Task<tbl_Salary> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Salary.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_Salary> Insert(SalaryCreate itemModel, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == itemModel.UserId);
                if (user == null)
                    throw new Exception("Không tìm thấy nhân viên");
                var checkExist = await db.tbl_Salary
                    .AnyAsync(x => x.Year == itemModel.Year && x.Month == itemModel.Month && x.UserId == itemModel.UserId && x.Enable == true);
                if (checkExist)
                    throw new Exception($"Đã tính lương cho nhân viên {user.FullName}");

                var model = new tbl_Salary(itemModel);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_Salary.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        /// <summary>
        /// Tính lương
        /// </summary>
        /// <returns></returns>
        public static async Task SalaryClosing(tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        DateTime time = DateTime.Now.AddMonths(-1);
                        int year = time.Year;
                        int month = time.Month;
                        //var check = await db.tbl_Salary.AnyAsync(x => x.Year == year && x.Month == month && x.Enable == true);
                        //if (check)
                        //    throw new Exception($"Đã tính lương tháng {month} năm {year}");

                        var staffs = await db.tbl_UserInformation
                            .Where(x => x.Enable == true && x.StatusId == ((int)AccountStatus.active) && x.RoleId != ((int)RoleEnum.student) && x.RoleId != ((int)RoleEnum.parents) && x.RoleId != ((int)RoleEnum.admin))
                            .Select(x => x.UserInformationId)
                            .ToListAsync();
                        if (staffs.Any())
                        {
                            foreach (var item in staffs)
                            {
                                var check = await db.tbl_Salary.AnyAsync(x => x.Year == year && x.Month == month && x.Enable == true && x.UserId == item);
                                if (check)
                                    continue; // Đã tính lương rồi thì không tính nữa
                                var salaryConfig = await db.tbl_SalaryConfig.FirstOrDefaultAsync(x => x.UserId == item && x.Enable == true);
                                var salary = new tbl_Salary
                                {
                                    BasicSalary = salaryConfig?.Value ?? 0,
                                    Bonus = 0,
                                    CreatedBy = user.FullName,
                                    CreatedOn = DateTime.Now,
                                    Deduction = 0,
                                    Enable = true,
                                    ModifiedBy = user.FullName,
                                    ModifiedOn = DateTime.Now,
                                    Month = month,
                                    Note = "",
                                    Status = 1,
                                    StatusName = "Chưa chốt",
                                    TotalSalary = salaryConfig?.Value ?? 0,
                                    TeachingSalary = 0,
                                    UserId = item,
                                    Year = year,
                                };
                                db.tbl_Salary.Add(salary);
                                await db.SaveChangesAsync();
                                double teachingSalary = 0;
                                var schedules = await db.tbl_Schedule
                                    .Where(x => x.TeacherAttendanceId == item && x.Enable == true && x.StartTime.Value.Month == month && x.StartTime.Value.Year == year)
                                    .ToListAsync();
                                if (schedules.Any())
                                {
                                    foreach (var schedule in schedules)
                                    {
                                        schedule.SalaryId = salary.Id;
                                        teachingSalary += schedule.TeachingFee ?? 0;
                                        await db.SaveChangesAsync();
                                    }
                                }
                                salary.TeachingSalary = teachingSalary;
                                salary.TotalSalary += teachingSalary;
                                await db.SaveChangesAsync();
                            }
                        }
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }
        public static async Task<tbl_Salary> Update(SalaryUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Salary.SingleOrDefaultAsync(x => x.Id == itemModel.Id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                if (entity.Status == 3)
                    throw new Exception("Đã thanh toán lương, không thể chỉnh sửa");
                entity.Deduction = itemModel.Deduction ?? entity.Deduction;
                entity.Bonus = itemModel.Bonus ?? entity.Bonus;
                entity.Note = itemModel.Note ?? entity.Note;
                entity.TotalSalary = (entity.BasicSalary + entity.TeachingSalary + entity.Bonus) - entity.Deduction;
                entity.Status = itemModel.Status ?? entity.Status;
                entity.StatusName = itemModel.StatusName ?? entity.StatusName;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                if (entity.TotalSalary < 0)
                    throw new Exception("Lương không phù hợp");
                if (entity.Status == 3 && entity.TotalSalary > 0)
                {
                    var staff = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == entity.UserId);
                    var payment = await db.tbl_PaymentMethod.FirstOrDefaultAsync(x => x.Code == "transfer");
                    db.tbl_PaymentSession.Add(new tbl_PaymentSession
                    {
                        BranchId = 0,
                        CreatedBy = user.FullName,
                        CreatedOn = DateTime.Now,
                        Enable = true,
                        ModifiedBy = user.FullName,
                        ModifiedOn = DateTime.Now,
                        PaymentMethodId = payment?.Id,
                        Reason = $"Thanh toán lương nhân viên {staff?.FullName}",
                        UserId = entity.UserId,
                        Note = itemModel.Note,
                        Type = 2,
                        TypeName = "Chi",
                        Value = entity.TotalSalary,
                        PrintContent = Task.Run(() => PaymentSessionService.GetPrintContent(
                            2,
                            entity.UserId,
                            $"Thanh toán lương nhân viên",
                            entity.TotalSalary,
                            user.FullName
                            )).Result,
                    });
                    //thông báo nhận lương
                   
                    Thread sendNoti = new Thread(async () =>
                    {
                        await NotificationService.Send(new tbl_Notification
                        {
                            Title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " thông báo lương tháng " + entity.Month + " năm " + entity.Year,
                            Content = System.Configuration.ConfigurationManager.AppSettings["ProjectName"] + " đã gửi lương tháng " + entity.Month + " năm " + entity.Year + " là " + entity.TotalSalary + " đến bạn. Vui lòng kiểm tra.",
                            UserId = entity.UserId
                        }, user);
                    });
                    sendNoti.Start();
                }
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public static async Task<AppDomainResult> GetAll(SalarySearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new SalarySearch();
                int userId = 0;
                if (user.RoleId != ((int)RoleEnum.admin) && user.RoleId != ((int)RoleEnum.accountant))
                    userId = user.UserInformationId;
                string sql = $"Get_Salary @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@Year = N'{baseSearch.Year}'," +
                    $"@Month = N'{baseSearch.Month}'," +
                    $"@UserId = N'{userId}'," +
                    $"@FullName = N'{baseSearch.FullName ?? ""}'," +
                    $"@UserCode = N'{baseSearch.UserCode ?? ""}'";
                var data = await db.Database.SqlQuery<Get_Salary>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_Salary(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public static async Task<AppDomainResult> GetTeachingDetail(TeachingDetailSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new TeachingDetailSearch();
                string sql = $"Get_TeachingDetail @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@SalaryId = N'{baseSearch.SalaryId}'";
                var data = await db.Database.SqlQuery<Get_TeachingDetail>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new TeachingDetailModel(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }

        public class Get_UserAvailable_Salary
        {
            public int UserInformationId { get; set; }
            public string FullName { get; set; }
            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public string UserCode { get; set; }
        }
        public class UserAvailableSearch
        {
            public int Year { get; set; }
            public int Month { get; set; }
        }
        public static async Task<List<Get_UserAvailable_Salary>> GetUserAvailable(UserAvailableSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_UserAvailable_Salary @year = {baseSearch.Year}, @month = {baseSearch.Month}";
                var data = await db.Database.SqlQuery<Get_UserAvailable_Salary>(sql).ToListAsync();
                return data;
            }
        }
    }
}