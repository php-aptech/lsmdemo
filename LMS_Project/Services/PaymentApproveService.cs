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
    public class PaymentApproveService
    {
        public static async Task<tbl_PaymentApprove> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_PaymentApprove.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        /// <summary>
        /// Tránh trường hợp học viên thanh toán quá số tiền nên sẽ có tính năng lưu lại số tiền duyệt và hoàn tiền
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task Approve(int id, int status, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (user.RoleId != ((int)RoleEnum.admin) && user.RoleId != ((int)RoleEnum.accountant))
                    throw new Exception("Không được phép duyệt");
                var entity = await db.tbl_PaymentApprove.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Status = status;
                entity.StatusName = status == 1 ? "Chờ duyệt"
                                    : status == 2 ? "Đã duyệt"
                                    : status == 3 ? "Không duyệt" : "";

                var bill = await db.tbl_Bill.SingleOrDefaultAsync(x => x.Id == entity.BillId);
                if (bill == null)
                    throw new Exception("Không tìm thấy thông tin thanh toán");

                var branch = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == bill.BranchId);

                if (entity.Status == 2)
                {
                    await BillService.Payment(new BillService.PaymentCreate
                    {
                        Id = entity.BillId ?? 0,
                        Note = entity.Note,
                        Paid = entity.Money,
                    }, user);

                    string content = $"Yêu cầu duyệt thanh toán tại trung tâm {branch?.Name} của bạn đã duyệt";
                    Thread send = new Thread(async () =>
                    {
                        await NotificationService.Send(new tbl_Notification
                        {
                            Content = content,
                            Title = "Duyệt thanh toán",
                            UserId = entity.UserId
                        }, user);
                        await NotificationService.Send(new tbl_Notification
                        {
                            Content = content,
                            Title = "Duyệt thanh toán",
                            UserId = bill.StudentId
                        }, user);
                    });
                }
                else if (entity.Status == 3)
                {

                    string content = $"Yêu cầu duyệt thanh toán tại trung tâm {branch?.Name} của bạn đã bị hủy, vui lòng liên hệ trung tâm để được hỗ trợ.";
                    Thread send = new Thread(async () =>
                    {
                        await NotificationService.Send(new tbl_Notification
                        {
                            Content = content,
                            Title = "Hủy thanh toán",
                            UserId = entity.UserId
                        }, user);
                        await NotificationService.Send(new tbl_Notification
                        {
                            Content = content,
                            Title = "Hủy thanh toán",
                            UserId = bill.StudentId
                        }, user);
                    });
                }
                await db.SaveChangesAsync();
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_PaymentApprove.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public class PaymentApproveResult : AppDomainResult
        {
            public double TotalMoney { get; set; }
        }
        public static async Task<PaymentApproveResult> GetAll(PaymentApproveSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new PaymentApproveSearch();
                if (user.RoleId != ((int)RoleEnum.admin) && user.RoleId != ((int)RoleEnum.accountant))
                    baseSearch.UserId = user.UserInformationId;

                string sql = $"Get_PaymentApprove @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@Status = '{baseSearch.Status}'," +
                    $"@UserId = '{baseSearch.UserId}',"+
                    $"@FromDate = N'{baseSearch.FromDate ?? ""}'," +
                    $"@ToDate = N'{baseSearch.ToDate ?? ""}'"; 

                var data = await db.Database.SqlQuery<Get_PaymentApprove>(sql).ToListAsync();
                if (!data.Any()) return new PaymentApproveResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_PaymentApprove(i)).ToList();
                var totalMoney = data[0].TotalMoney;

                return new PaymentApproveResult { 
                    TotalRow = totalRow, 
                    Data = result,
                    TotalMoney = totalMoney
                };
            }
        }
    }
}