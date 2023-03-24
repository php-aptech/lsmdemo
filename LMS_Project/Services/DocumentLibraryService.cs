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

namespace LMS_Project.Services
{
    public class DocumentLibraryService
    {
        public static async Task<tbl_DocumentLibrary> Insert(DocumentLibraryCreate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var model = new tbl_DocumentLibrary(itemModel);
                        var directory = await db.tbl_DocumentLibraryDirectory.SingleOrDefaultAsync(x => x.Enable == true && x.Id == model.DirectoryId);
                        if (directory == null)
                            throw new Exception("Chủ đề không tồn tại");
                        model.CreatedBy = userLogin.FullName;
                        db.tbl_DocumentLibrary.Add(model);
                        await db.SaveChangesAsync();
                        //Cập nhật số lượng tài liệu của chủ đề
                        var totalDocument = await db.tbl_DocumentLibrary.Where(x => x.Enable == true && x.DirectoryId == directory.Id).CountAsync();
                        directory.TotalDocument = totalDocument;
                        await db.SaveChangesAsync();
                        tran.Commit();
                        return model;
                    }
                    catch(Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }

        public static async Task<tbl_DocumentLibrary> Update(DocumentLibraryUpdate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_DocumentLibrary.SingleOrDefaultAsync(x => x.Id == itemModel.Id && x.Enable == true);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.ModifiedBy = userLogin.FullName;
                entity.FileUrl = itemModel.FileUrl == null ? entity.FileUrl : string.IsNullOrEmpty(itemModel.FileUrl) ? entity.FileUrl : itemModel.FileUrl;
                entity.Background = itemModel.Background == null ? entity.Background : string.IsNullOrEmpty(itemModel.Background) ? entity.Background : itemModel.Background;
                entity.Description = itemModel.Description == null ? entity.Description : string.IsNullOrEmpty(itemModel.Description) ? entity.Description : itemModel.Description;
                await db.SaveChangesAsync();
                return entity;
            }    
        }

        public static async Task<AppDomainResult> GetAll(DocumentLibrarySearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null)
                    baseSearch = new DocumentLibrarySearch();
                var parms = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@PageIndex", Value= baseSearch.PageIndex, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@PageSize", Value= baseSearch.PageSize, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@Sort", Value= baseSearch.Sort, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@SortType", Value= baseSearch.SortType, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@DirectoryId", Value= baseSearch.DirectoryId == null ? 0 : baseSearch.DirectoryId,DbType = DbType.Int64 }
                };
                var sqlStr = $"Exec Get_DocumentLibrary @PageIndex,@PageSize,@DirectoryId,@Sort,@SortType";
                var data = await db.Database.SqlQuery<Get_DocumentLibrary>(sqlStr, parms.ToArray()).ToListAsync();
                if (!data.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                int totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_DocumentLibrary(i)).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }

        public static async Task<tbl_DocumentLibrary> GetById(int id, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_DocumentLibrary.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                if (entity == null)
                    return null;
                return entity;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_DocumentLibrary.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }      

    }
}