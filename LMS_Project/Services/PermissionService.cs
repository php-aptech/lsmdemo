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
    public class PermissionService
    {
        public class RoleModel
        { 
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public static async Task<List<RoleModel>> GetRole()
        {
            var data = new List<RoleModel>()
            {
                new RoleModel { Id = ((int)RoleEnum.admin), Name = "Admin" },
                new RoleModel { Id = ((int)RoleEnum.teacher), Name = "Giáo viên" },
                new RoleModel { Id = ((int)RoleEnum.student), Name = "Học viên" },
                new RoleModel { Id = ((int)RoleEnum.manager), Name = "Quản lý" },
                new RoleModel { Id = ((int)RoleEnum.sale), Name = "Tư vấn viên" },
                new RoleModel { Id = ((int)RoleEnum.accountant), Name = "Kế toán" },
                new RoleModel { Id = ((int)RoleEnum.academic), Name = "Học vụ" },
                new RoleModel { Id = ((int)RoleEnum.parents), Name = "Phụ huynh" },
            };
            return data;
        }
        public static async Task<List<RoleModel>> GetRoleStaff()
        {
            var data = new List<RoleModel>()
            {
                new RoleModel { Id = ((int)RoleEnum.admin), Name = "Admin" },
                new RoleModel { Id = ((int)RoleEnum.teacher), Name = "Giáo viên" },
                new RoleModel { Id = ((int)RoleEnum.manager), Name = "Quản lý" },
                new RoleModel { Id = ((int)RoleEnum.sale), Name = "Tư vấn viên" },
                new RoleModel { Id = ((int)RoleEnum.accountant), Name = "Kế toán" },
                new RoleModel { Id = ((int)RoleEnum.academic), Name = "Học vụ" },
            };
            return data;
        }
        public class PermissionCreate
        {
            public string Controller { get; set; }
            public string Action { get; set; }
            public string Description { get; set; }
        }
        public static async Task<tbl_Permission> Insert(PermissionCreate itemModel)
        {
            using (var db = new lmsDbContext())
            {
                var check = await db.tbl_Permission.AnyAsync(x => x.Controller.ToUpper() == itemModel.Controller.ToUpper()
                && x.Action.ToUpper() == itemModel.Action.ToUpper());
                if (check)
                    throw new Exception("Đã có");
                var model = new tbl_Permission
                {
                    Action = itemModel.Action,
                    Allowed = "",
                    Controller = itemModel.Controller,
                    Description = itemModel.Description
                };
                db.tbl_Permission.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public class PermissionUpdate
        {
            public int Id { get; set; }
            /// <summary>
            /// danh sách Id quyền, mẫu 1,2,3
            /// </summary>
            public string Allowed { get; set; }
        }
        public static async Task<tbl_Permission> Update(PermissionUpdate model)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Permission.SingleOrDefaultAsync(x => x.Id == model.Id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Allowed = model.Allowed;
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public class PermissionSearch
        { 
            public string search { get; set; }
        }
        public static async Task<List<PermissionModel>> GetAll(PermissionSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new PermissionSearch();
                 var data = await db.tbl_Permission.Where(x=>
                x.Controller.Contains(baseSearch.search)
                || x.Action.Contains(baseSearch.search)
                || string.IsNullOrEmpty(baseSearch.search))
                    .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToListAsync();
                var result = (from i in data
                              select new PermissionModel(i)).ToList();
                return result;
            }
        }
    }
}