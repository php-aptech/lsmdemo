﻿using LMS_Project.Areas.Models;
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
    public class BranchService
    {
        public static async Task<tbl_Branch> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_Branch> Insert(BranchCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var checkCode = await db.tbl_Branch.AnyAsync(x => x.Code.ToUpper() == itemModel.Code.ToUpper() && x.Enable == true);
                if (checkCode)
                    throw new Exception("Mã đã tồn tại");
                var model = new tbl_Branch(itemModel);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_Branch.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task<tbl_Branch> Update(BranchUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                if (itemModel.Code != null)
                {
                    var checkCode = await db.tbl_Branch.AnyAsync(x => x.Code.ToUpper() == itemModel.Code.ToUpper() && x.Enable == true && x.Id != entity.Id);
                    if (checkCode)
                        throw new Exception("Mã đã tồn tại");
                }
                entity.Code = itemModel.Code ?? entity.Code;
                entity.Name = itemModel.Name ?? entity.Name;
                entity.AreaId = itemModel.AreaId ?? entity.AreaId;
                entity.DistrictId = itemModel.DistrictId ?? entity.DistrictId;
                entity.WardId = itemModel.WardId ?? entity.WardId;
                entity.Address = itemModel.Address ?? entity.Address;
                entity.Mobile = itemModel.Mobile ?? entity.Mobile;
                entity.Email = itemModel.Email ?? entity.Email;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Branch.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;

                //Kiểm tra dữ liệu
                var _class = await db.tbl_Class.FirstOrDefaultAsync(x => x.BranchId == id && x.Enable == true);
                if (_class != null)
                    throw new Exception($"Có dữ liệu lớp {_class.Name} thuộc trung tâm {entity.Name}, không thể xóa");

                string sqlTeacher = $"Get_User @PageIndex = {1}," +
                    $"@PageSize = {1}," +
                    $"@BranchIds = N'{id}'";
                var teachers = await db.Database.SqlQuery<Get_UserInformation>(sqlTeacher).ToListAsync();
                if (teachers.Any())
                    throw new Exception($"Có dữ liệu nhân viên {teachers[0].FullName} thuộc trung tâm {entity.Name}, không thể xóa");

                await db.SaveChangesAsync();
            }
        }
        public static async Task<AppDomainResult> GetAll(BranchSearch baseSearch,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new BranchSearch();

                var l = await db.tbl_Branch.Where(x => x.Enable == true
                && (x.Name.Contains(baseSearch.Name) || string.IsNullOrEmpty(baseSearch.Name))
                && (x.Code.Contains(baseSearch.Code) || string.IsNullOrEmpty(baseSearch.Code))
                ).OrderBy(x => x.Name).ToListAsync();

                ///Nhân viên chỉ thấy trung tâm của mình
                if (user.RoleId != ((int)RoleEnum.admin))
                {
                    l = l.Where(x => user.BranchIds.Contains(x.Id.ToString())).ToList();
                }
                if(!l.Any())
                    return new AppDomainResult { Data = null, TotalRow = 0, Success = true };

                int totalRow = l.Count();
                var result = l.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow, Success = true };
            }
        }
    }
}