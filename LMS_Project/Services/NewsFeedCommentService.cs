using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class NewsFeedCommentService
    {
        public static async Task<tbl_NewsFeedComment> Insert(NewsFeedCommentCreate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var model = new tbl_NewsFeedComment(itemModel);
                        var newsFeed = await db.tbl_NewsFeed.SingleOrDefaultAsync(x => x.Enable == true && x.Id == model.NewsFeedId);
                        if (newsFeed == null)
                            throw new Exception("Bản tin không tồn tại");
                        model.CreatedBy = model.ModifiedBy = userLogin.FullName;
                        model.CreatedIdBy = userLogin.UserInformationId;
                        model.TotalReply = 0;
                        db.tbl_NewsFeedComment.Add(model);
                        await db.SaveChangesAsync();
                        //Cập nhật số lượng bình luận bản tin
                        var newsFeedComments = await db.tbl_NewsFeedComment.Where(x => x.Enable == true && x.NewsFeedId == newsFeed.Id).ToListAsync();
                        newsFeed.TotalComment = newsFeedComments.Count();
                        await db.SaveChangesAsync();
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

        public static async Task<tbl_NewsFeedComment> Update(NewsFeedCommentUpdate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_NewsFeedComment.SingleOrDefaultAsync(x => x.Enable == true && x.Id == itemModel.Id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                if (entity.CreatedIdBy != userLogin.UserInformationId)
                    throw new Exception("Không có quyền thao tác");
                entity.Content = itemModel.Content ?? entity.Content;
                entity.ModifiedBy = userLogin.FullName;
                await db.SaveChangesAsync();
                return entity;
            }    
        }

        public static async Task Delete(int id, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await db.tbl_NewsFeedComment.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                        if (entity == null)
                            throw new Exception("Không tìm thấy dữ liệu !");
                        var newsFeed = await db.tbl_NewsFeed.SingleOrDefaultAsync(x => x.Enable == true && x.Id == entity.NewsFeedId);
                        if (newsFeed == null)
                        {
                            throw new Exception("Bản tin không tồn tại");
                        }
                        //Admin, người tạo bình luận này và người tạo ra bản tin sẽ được quyền xóa những comment này
                        if (userLogin.RoleId != (int)RoleEnum.admin && entity.CreatedIdBy != userLogin.UserInformationId && newsFeed.CreatedIdBy != userLogin.UserInformationId)
                        {
                            throw new Exception("Không có quyền thao tác");
                        }
                        entity.Enable = false;
                        await db.SaveChangesAsync();
                        //Cập nhật lại số lượng cmt
                        var comments = await db.tbl_NewsFeedComment.Where(x => x.Enable == true && x.NewsFeedId == newsFeed.Id).ToListAsync();
                        newsFeed.TotalComment = comments.Count();
                        await db.SaveChangesAsync();
                        tran.Commit();
                    }
                    catch(Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }    
                    
            }
        }

        public static async Task<AppDomainResult> GetAll(NewsFeedCommentSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null)
                    baseSearch = new NewsFeedCommentSearch();
                var parms = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@PageIndex", Value= baseSearch.PageIndex, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@PageSize", Value= baseSearch.PageSize, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@NewsFeedId", Value= baseSearch.NewsFeedId == null ? 0 : baseSearch.NewsFeedId,DbType = DbType.Int64 },
                    new SqlParameter { ParameterName = "@Sort", Value= baseSearch.Sort, DbType = DbType.Int16 },
                    new SqlParameter { ParameterName = "@SortType", Value= baseSearch.SortType, DbType = DbType.Boolean },
                };
                var sqlStr = $"Exec Get_NewsFeedComment @PageIndex,@PageSize,@NewsFeedId,@Sort,@SortType";
                var data = await db.Database.SqlQuery<Get_NewsFeedComment>(sqlStr, parms.ToArray()).ToListAsync();
                if (!data.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                int totalRow = data[0].TotalRow ?? 0;
                var result = data.Select(i => new tbl_NewsFeedComment(i)).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }

    }
}