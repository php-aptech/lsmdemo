using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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
    public class NewsFeedGroupService
    {
        public static async Task<tbl_NewsFeedGroup> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_NewsFeedGroup.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                return data;
            }
        }
        public static async Task<tbl_NewsFeedGroup> Insert(NewsFeedGroupCreate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var model = new tbl_NewsFeedGroup(itemModel);
                        var members = new List<Members>();
                        var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == itemModel.ClassId);
                        if (_class == null)
                            throw new Exception("Không tìm thấy lớp học");

                        var checkExist = await db.tbl_NewsFeedGroup.AnyAsync(x => x.ClassId == itemModel.ClassId && x.Enable == true);
                        if (checkExist)
                            throw new Exception("Lớp đã tạo nhóm");

                        members.Add(new Members { UserId = _class.AcademicId, Type = 1 });
                        members.Add(new Members { UserId = _class.TeacherId, Type = 1 });

                        var studentInClass = await db.tbl_StudentInClass
                            .Where(x => x.ClassId == _class.Id && x.Enable == true && x.StudentId != null).Select(x => x.StudentId).Distinct().ToListAsync();
                        if (studentInClass.Any())
                        {
                            studentInClass.ForEach(x => members.Add(new Members { UserId = x.Value, Type = 2 }));
                        }


                        //Check người tạo group có trong danh sách member class chưa
                        var createrGroup = members.Any(x => x.UserId == userLogin.UserInformationId);
                        if (!createrGroup)
                            members.Add(new Members { UserId = userLogin.UserInformationId, Type = 1 });

                        model.ClassId = itemModel.ClassId;
                        model.Members = members.Count();
                        model.CreatedBy = model.ModifiedBy = userLogin.FullName;
                        db.tbl_NewsFeedGroup.Add(model);
                        await db.SaveChangesAsync();

                        if (members.Any())
                        {
                            db.tbl_UserInNFGroup.AddRange(members.Select(x => new tbl_UserInNFGroup
                            {
                                CreatedBy = userLogin.FullName,
                                CreatedOn = DateTime.Now,
                                Enable = true,
                                ModifiedBy = userLogin.FullName,
                                ModifiedOn = DateTime.Now,
                                NewsFeedGroupId = model.Id,
                                Type = x.Type,
                                TypeName = x.TypeName,
                                UserId = x.UserId
                            }));
                        }
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

        public static async Task<tbl_NewsFeedGroup> Update(NewsFeedGroupUpdate itemModel, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                if (userLogin.RoleId != (int)RoleEnum.admin)
                {
                    var userInGroup = await db.tbl_UserInNFGroup.SingleOrDefaultAsync(x => x.Enable == true && x.NewsFeedGroupId == itemModel.Id && x.UserId == userLogin.UserInformationId);
                    if (userInGroup == null || userInGroup.Type != 1)
                        throw new Exception("Không có quyền thao tác !");
                }         
                var entity = await db.tbl_NewsFeedGroup.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu !");
                entity.Name = itemModel.Name ?? entity.Name;
                entity.BackGround = itemModel.BackGround ?? entity.BackGround;
                entity.ModifiedBy = userLogin.FullName;
                await db.SaveChangesAsync();
                return entity;
            }
        }


        public static async Task Delete(int id, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                if (userLogin.RoleId != (int)RoleEnum.admin)
                {
                    var userInGroup = await db.tbl_UserInNFGroup.SingleOrDefaultAsync(x => x.Enable == true && x.NewsFeedGroupId == id && x.UserId == userLogin.UserInformationId);
                    if (userInGroup == null || userInGroup.Type != 1)
                        throw new Exception("Không có quyền thao tác !");
                }
                var entity = await db.tbl_NewsFeedGroup.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu !");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public static async Task<AppDomainResult> GetAll(NewsFeedGroupSearch baseSearch, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null)
                    baseSearch = new NewsFeedGroupSearch();
                int? userId = null;
                if (userLogin.RoleId != (int)RoleEnum.admin)
                {
                    userId = userLogin.UserInformationId;
                }
                var parms = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@PageIndex", Value= baseSearch.PageIndex, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@PageSize", Value= baseSearch.PageSize, DbType = DbType.Int16},
                    new SqlParameter { ParameterName = "@Name", Value= (string.IsNullOrEmpty(baseSearch.Name)) ? string.Empty : baseSearch.Name ,DbType = DbType.String },
                    new SqlParameter { ParameterName = "@ClassId", Value= baseSearch.ClassId == null ? 0 : baseSearch.ClassId,DbType = DbType.Int64 },
                    new SqlParameter { ParameterName = "@UserId", Value= userId == null ? 0 : userId,DbType = DbType.Int64 },
                    new SqlParameter { ParameterName = "@Sort", Value= baseSearch.Sort, DbType = DbType.Int16 },
                    new SqlParameter { ParameterName = "@SortType", Value= baseSearch.SortType, DbType = DbType.Boolean },
                };
                var sqlStr = $"Exec Get_NewsFeedGroup @PageIndex,@PageSize,@Name,@ClassId,@UserId,@Sort,@SortType";
                var data = await db.Database.SqlQuery<Get_NewsFeedGroup>(sqlStr, parms.ToArray()).ToListAsync();
                if (!data.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };
                int totalRow = data[0].TotalRow ?? 0;
                var result = data.Select(i => new tbl_NewsFeedGroup(i)).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }
        public class Get_ClassAvailable_NewsFeedGroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Type { get; set; }
            public string TypeName { get; set; }
        }
        public static async Task<List<Get_ClassAvailable_NewsFeedGroup>> GetClassAvailable()
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_ClassAvailable_NewsFeedGroup";
                var data = await db.Database.SqlQuery<Get_ClassAvailable_NewsFeedGroup>(sql).ToListAsync();
                return data;
            }
        }
    }
}