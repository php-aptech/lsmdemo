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
    public class UserInNFGroupService
    {
        public static async Task<object> Insert(UserInNFGroupCreate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (userLogin.RoleId != (int)RoleEnum.admin)
                        {
                            var userLoginInGroup = await db.tbl_UserInNFGroup.SingleOrDefaultAsync(x => x.NewsFeedGroupId == itemModel.NewsFeedGroupId && x.UserId == userLogin.UserInformationId && x.Enable == true);
                            if (userLoginInGroup.Type != 1 || userLoginInGroup == null)
                            {
                                throw new Exception("Không có quyền thao tác");
                            }
                        }
                        var group = await db.tbl_NewsFeedGroup.SingleOrDefaultAsync(x => x.Enable == true && x.Id == itemModel.NewsFeedGroupId);
                        if (group == null)
                            throw new Exception("Nhóm không tồn tại !");
                        if (!itemModel.Members.Any())
                            throw new Exception("Vui lòng chọn người dùng !");
                        var usersIntoGroup = new List<Members>();
                        foreach (var member in itemModel.Members)
                        {
                            var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == member.UserId && x.Enable == true);
                            if (user == null)
                                throw new Exception("Người dùng không tồn tại !");
                            //Kiểm tra user thêm vào đã có trong nhóm chưa
                            var memberExistsGroup = await db.tbl_UserInNFGroup.AnyAsync(x => x.NewsFeedGroupId == itemModel.NewsFeedGroupId && x.UserId == member.UserId && x.Enable == true);
                            if (memberExistsGroup)
                            {
                                continue;
                            }
                            //Lấy những user không bị trùng trong nhóm
                            usersIntoGroup.Add(member);
                        }
                        var model = new tbl_UserInNFGroup(itemModel);
                        if (usersIntoGroup.Any())
                        {
                            db.tbl_UserInNFGroup.AddRange(usersIntoGroup.Select(x => new tbl_UserInNFGroup
                            {
                                CreatedBy = userLogin.FullName,
                                CreatedOn = DateTime.Now,
                                Enable = true,
                                ModifiedBy = userLogin.FullName,
                                ModifiedOn = DateTime.Now,
                                NewsFeedGroupId = model.NewsFeedGroupId,
                                Type = x.Type,
                                TypeName = x.TypeName,
                                UserId = x.UserId
                            }));
                            group.Members = group.Members + usersIntoGroup.Count();
                            await db.SaveChangesAsync();
                        }
                        await db.SaveChangesAsync();
                        tran.Commit();
                        return usersIntoGroup;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }

        public static async Task<tbl_UserInNFGroup> Update(UserInNFGroupUpdate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_UserInNFGroup.SingleOrDefaultAsync(x => x.Enable == true && x.Id == itemModel.Id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu !");
                if (userLogin.RoleId != (int)RoleEnum.admin)
                {
                    var userLoginInGroup = await db.tbl_UserInNFGroup.SingleOrDefaultAsync(x => x.Enable == true && x.NewsFeedGroupId == entity.NewsFeedGroupId && x.UserId == userLogin.UserInformationId);
                    if (userLoginInGroup.Type != 1 || userLoginInGroup == null)
                    {
                        throw new Exception("Không có quyền thao tác !");
                    }
                }
                entity.Type = itemModel.Type ?? entity.Type;
                entity.TypeName = itemModel.TypeName ?? entity.TypeName;
                entity.ModifiedBy = userLogin.FullName ?? entity.ModifiedBy;
                await db.SaveChangesAsync();
                return entity;
            }
        }

        public static async Task Delete(int id, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_UserInNFGroup.SingleOrDefaultAsync(x => x.Enable == true && x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu !");
                if (userLogin.RoleId != (int)RoleEnum.admin)
                {
                    var userLoginInGroup = await db.tbl_UserInNFGroup.SingleOrDefaultAsync(x => x.Enable == true && x.NewsFeedGroupId == entity.NewsFeedGroupId && x.UserId == userLogin.UserInformationId);
                    if (userLoginInGroup.Type != 1 || userLoginInGroup == null)
                    {
                        throw new Exception("Không có quyền thao tác !");
                    }
                }
                entity.Enable = false;
                await db.SaveChangesAsync();
                var newsFeedGroup = await db.tbl_NewsFeedGroup.SingleOrDefaultAsync(x => x.Id == entity.NewsFeedGroupId);
                if (newsFeedGroup == null)
                    throw new Exception("Không tìm thấy nhóm");
                newsFeedGroup.Members = await db.tbl_UserInNFGroup.CountAsync(x => x.NewsFeedGroupId == newsFeedGroup.Id && x.Enable == true);
                await db.SaveChangesAsync();
            }
        }

        public static async Task<tbl_UserInNFGroup> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_UserInNFGroup.SingleOrDefaultAsync(x => x.Enable == true && x.Id == id);
                if (entity == null)
                    return null;
                var userInGroup = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.Enable == true && x.UserInformationId == entity.UserId);
                var data = new tbl_UserInNFGroup(entity)
                {
                    FullName = userInGroup.FullName ?? "",
                    UserCode = userInGroup.UserCode ?? "",
                    RoleName = userInGroup.RoleName ?? "",
                    Avatar = userInGroup.Avatar ?? ""
                };
                return data;
            }
        }

        public static async Task<AppDomainResult> GetAll(UserInNFGroupSearch baseSearch, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null)
                    baseSearch = new UserInNFGroupSearch();
                var parms = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@PageIndex", Value= baseSearch.PageIndex, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@PageSize", Value= baseSearch.PageSize, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@FullName", Value= (string.IsNullOrEmpty(baseSearch.FullName)) ? string.Empty : baseSearch.FullName ,DbType = DbType.String },
                    new SqlParameter { ParameterName = "@RoleId", Value= (string.IsNullOrEmpty(baseSearch.RoleId)) ? string.Empty : baseSearch.RoleId ,DbType = DbType.String },
                    new SqlParameter { ParameterName = "@Type", Value= baseSearch.Type == null ? 0 : baseSearch.Type ,DbType = DbType.Int16 },
                    new SqlParameter { ParameterName = "@NewsFeedGroupId", Value= baseSearch.NewsFeedGroupId == null ? 0 : baseSearch.NewsFeedGroupId ,DbType = DbType.Int64 },
                    new SqlParameter { ParameterName = "@Sort", Value= baseSearch.Sort, DbType = DbType.Int16 },
                    new SqlParameter { ParameterName = "@SortType", Value= baseSearch.SortType, DbType = DbType.Boolean }
                };
                var sqlStr = $"Exec Get_UserInNewsFeedGroup @PageIndex ,@PageSize ,@FullName ,@RoleId ,@Type ,@NewsFeedGroupId ,@Sort ,@SortType";
                var data = await db.Database.SqlQuery<Get_UserInNFGroup>(sqlStr, parms.ToArray()).ToListAsync();
                if (!data.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                int totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_UserInNFGroup(i)).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }

        public static async Task<AppDomainResult> GetUserNotInGroup(int? groupId)
        {
            using (var db = new lmsDbContext())
            {
                var parms = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@NewsFeedGroupId", Value= groupId ?? null, DbType = DbType.Int64},
                };
                var sqlStr = $"Exec Get_UserNotInNewsFeedGroup @NewsFeedGroupId";
                var data = await db.Database.SqlQuery<Get_UserNotInNFGroup>(sqlStr, parms.ToArray()).ToListAsync();
                if (!data.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                return new AppDomainResult { Data = data, Success = true };
            }
        }

    }
}