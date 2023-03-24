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
    public class DocumentLibraryDirectoryService
    {
        public static async Task<tbl_DocumentLibraryDirectory> Insert(DocumentLibraryDirectoryCreate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var model = new tbl_DocumentLibraryDirectory(itemModel);
                model.CreatedBy = userLogin.FullName;
                db.tbl_DocumentLibraryDirectory.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }

        public static async Task<tbl_DocumentLibraryDirectory> Update(DocumentLibraryDirectoryUpdate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_DocumentLibraryDirectory.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value && x.Enable == true);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Name = itemModel.Name == null ? entity.Name : string.IsNullOrEmpty(itemModel.Name) ? entity.Name : itemModel.Name;
                entity.Description = itemModel.Description == null ? entity.Description : itemModel.Description;
                entity.ModifiedBy = userLogin.FullName;
                await db.SaveChangesAsync();
                return entity;
            }
        }

        public static async Task<tbl_DocumentLibraryDirectory> GetById(int id, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_DocumentLibraryDirectory.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                if (entity == null)
                    return null;
                return entity;
            }
        }
        public static async Task<AppDomainResult> GetAll(DocumentLibraryDirectorySearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null)
                    baseSearch = new DocumentLibraryDirectorySearch();
                var parms = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@Sort", Value= baseSearch.Sort, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@SortType", Value= baseSearch.SortType, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@Name", Value= string.IsNullOrEmpty(baseSearch.Name) ? string.Empty : baseSearch.Name,DbType = DbType.String }
                };
                var sqlStr = $"Exec Get_DocumentLibraryDirectory @Name,@Sort,@SortType";
                var data = await db.Database.SqlQuery<Get_DocumentLibraryDirectory>(sqlStr, parms.ToArray()).ToListAsync();
                if (!data.Any())
                    return new AppDomainResult { Data = null, Success = true };
                var result = data.Select(i => new tbl_DocumentLibraryDirectory(i)).ToList();
                return new AppDomainResult { Data = result, Success = true };
            }
        }

        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_DocumentLibraryDirectory.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
    }
}