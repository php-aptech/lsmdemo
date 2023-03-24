using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class FeedbackService
    {
        public static async Task<tbl_Feedback> Insert(FeedbackCreate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var model = new tbl_Feedback(itemModel);
                model.Status = (int)FeedbackStatus.MoiGui;
                model.CreatedIdBy = userLogin.UserInformationId;
                model.CreatedBy = model.ModifiedBy = userLogin.FullName;
                model.StarRating = 0;
                db.tbl_Feedback.Add(model);
                await db.SaveChangesAsync();
                //thÔng báo cho học vụ có feedback mới
                List<tbl_UserInformation> academics = await db.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.academic).ToListAsync();
                if (academics.Any())
                {
                    Thread sendNoti = new Thread(async () =>
                    {
                        foreach (var item in academics)
                        {
                            await NotificationService.Send(new tbl_Notification
                            {
                                Title = "Bạn nhận được phản hồi mới",
                                Content = model.Content,
                                UserId = item.UserInformationId
                            }, userLogin);
                        }
                    });
                    sendNoti.Start();
                }
                return model;
            }
        }
        public static async Task<tbl_Feedback> Update(FeedbackUpdate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Feedback.SingleOrDefaultAsync(x => x.Id == itemModel.Id && x.Enable == true);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                //Kiểm tra nếu user là người gửi phản hồi thì trạng thái phản hồi phải là mới gửi thì cho phép sửa thông tin
                if (entity.CreatedIdBy == userLogin.UserInformationId && entity.Status != (int)FeedbackStatus.MoiGui)
                    throw new Exception("Phản hồi đang được xử lý, thao tác thất bại");
                entity.Title = string.IsNullOrEmpty(itemModel.Title) ? entity.Title : itemModel.Title == null ? entity.Title : itemModel.Title;
                entity.Content = string.IsNullOrEmpty(itemModel.Content) ? entity.Content : itemModel.Content == null ? entity.Content : itemModel.Content;
                entity.IsIncognito = itemModel.IsIncognito == null ? entity.IsIncognito : itemModel.IsIncognito;
                entity.IsPriority = itemModel.IsPriority == null ? entity.IsPriority : itemModel.IsPriority;
                //Trường hợp đổi trạngg thái
                if (itemModel.Status != null)
                {
                    entity.Status = itemModel.Status;
                }
                await db.SaveChangesAsync();
                return entity;
            }
        }

        public static async Task RatingFeedBack(int id, int startRating, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Feedback.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                ////Phản hồi phải ở trạng thái xử lý đã xong mới cho học viên đánh giá
                //if (entity.CreatedIdBy == userLogin.UserInformationId && entity.Status != (int)FeedbackStatus.DaXong)
                //    throw new Exception("Đánh giá thất bại");
                entity.StarRating = startRating;
                await db.SaveChangesAsync();
            }
        }

        public static async Task<tbl_Feedback> GetById(int id, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Feedback.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                if (entity == null)
                    return null;
                var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == entity.CreatedIdBy && x.Enable == true);
                entity.Avatar = user == null ? null : user.Avatar;
                //Kiểm tra cờ ẩn danh nếu có thì ẩn thông tin người tạo phản hồi
                if (entity.IsIncognito == true && entity.CreatedIdBy != userLogin.UserInformationId && userLogin.RoleId != (int)RoleEnum.admin)
                {
                    entity.Avatar = null;
                    entity.CreatedIdBy = null;
                    entity.CreatedBy = null;
                    entity.ModifiedBy = null;
                }
                return entity;
            }
        }

        public static async Task<AppDomainResult> GetAll(FeedbackSearch baseSearch, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null)
                    baseSearch = new FeedbackSearch();

                string userIds = "";
                if (userLogin.RoleId == ((int)RoleEnum.student))
                    userIds = userLogin.UserInformationId.ToString();
                else
                    userIds = baseSearch.UserIds ?? "";

                var parms = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@PageIndex", Value= baseSearch.PageIndex, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@PageSize", Value= baseSearch.PageSize, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@Status", Value= baseSearch.Status == null ? 0 : baseSearch.Status,DbType = DbType.Int16 },
                    new SqlParameter { ParameterName = "@Sort", Value= baseSearch.Sort, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@SortType", Value= baseSearch.SortType, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@SaleId", Value= userLogin.RoleId == ((int)RoleEnum.sale) ? userLogin.UserInformationId : 0, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@UserIds", Value= userIds, DbType = DbType.String},
                    new SqlParameter { ParameterName = "@BranchIds",
                        Value =
                        (
                            userLogin.RoleId == ((int)RoleEnum.admin)
                            || userLogin.RoleId == ((int)RoleEnum.student)
                            || userLogin.RoleId == ((int)RoleEnum.parents)
                            || userLogin.RoleId == ((int)RoleEnum.sale)
                        ) ? "" : userLogin.BranchIds
                        , DbType = DbType.String},
                };
                var sqlStr = $"Exec Get_Feedback @PageIndex,@PageSize,@Status,@Sort,@SortType,@SaleId,@UserIds,@BranchIds";
                var data = await db.Database.SqlQuery<Get_Feedback>(sqlStr, parms.ToArray()).ToListAsync();
                if (!data.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                int totalRow = data[0].TotalRow ?? 0;
                var result = data.Select(i => new tbl_Feedback(i)).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }
    }
}