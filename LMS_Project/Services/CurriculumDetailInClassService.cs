﻿using LMS_Project.Areas.Models;
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
    public class CurriculumDetailInClassService
    {
        public static async Task<tbl_CurriculumDetailInClass> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_CurriculumDetailInClass.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }

        public static async Task<tbl_CurriculumDetailInClass> Update(CurriculumDetailInClassUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_CurriculumDetailInClass.SingleOrDefaultAsync(x => x.Id == itemModel.Id && x.Enable == true);
                if (entity == null)
                    throw new Exception("Không tìm thấy");
                entity.Name = string.IsNullOrEmpty(itemModel.Name) ? entity.Name : itemModel.Name;              
                entity.ModifiedBy = user.FullName;
                //Nếu vị trí muốn đổi có chương nào tồn tại thì hoán đổi vị trí cho nhau
                if (itemModel.Index != null)
                {
                    var detailOfCurriculumExsist = await db.tbl_CurriculumDetailInClass.SingleOrDefaultAsync(x => x.Enable == true && x.CurriculumIdInClass == entity.Id && x.Index == itemModel.Index);
                    if (detailOfCurriculumExsist != null)
                    {
                        detailOfCurriculumExsist.Index = entity.Index;
                    }
                    entity.Index = itemModel.Index;
                }
                await db.SaveChangesAsync();
                return entity;
            }    
        }

        public static async Task<tbl_CurriculumDetailInClass> Insert(CurriculumDetailInClassCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var curriculumInClass = await db.tbl_CurriculumInClass.SingleOrDefaultAsync(x => x.Id == itemModel.CurriculumIdInClass);
                if (curriculumInClass == null)
                    throw new Exception("Không tìm thấy giáo trình");

                var lastIndex = await db.tbl_CurriculumDetailInClass
                    .Where(x => x.CurriculumIdInClass == curriculumInClass.Id && x.Enable == true).OrderByDescending(x => x.Index).FirstOrDefaultAsync();

                var model = new tbl_CurriculumDetailInClass(itemModel);
                model.IsHide = true;
                model.CreatedBy = model.ModifiedBy = user.FullName;
                model.Index = lastIndex == null ? 1 : ((lastIndex.Index ?? 0) + 1);
                db.tbl_CurriculumDetailInClass.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }

        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_CurriculumDetailInClass.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public static async Task Hide(int id)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await db.tbl_CurriculumDetailInClass.SingleOrDefaultAsync(x => x.Id == id);
                        if (entity == null)
                            throw new Exception("Không tìm thấy chủ đề");
                        entity.IsHide = !entity.IsHide;
                        var files = await db.tbl_FileCurriculumInClass
                            .Where(x => x.CurriculumDetailId == entity.Id && x.Enable == true).ToListAsync();
                        if (files.Any())
                        {
                            foreach (var item in files)
                            {
                                item.IsHide = entity.IsHide;
                            }
                        }
                        await db.SaveChangesAsync();
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
        public static async Task<List<Get_CurriculumDetailInClass>> GetCurriculumDetailInClass(CurriculumDetailInClassSearch baseSearch, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null)
                    baseSearch = new CurriculumDetailInClassSearch();
                var parms = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@CurriculumId", Value= baseSearch.CurriculumIdInClassId == null ? 0 : baseSearch.CurriculumIdInClassId, DbType = DbType.Int64 },
                    new SqlParameter { ParameterName = "@RoleId", Value= userLog.RoleId, DbType = DbType.Int64}
                };
                var sqlStr = $"Exec Get_CurriculumDetailInClass @CurriculumId,@RoleId";
                var data = await db.Database.SqlQuery<Get_CurriculumDetailInClass>(sqlStr, parms.ToArray()).ToListAsync();
                return data;
            }
        }
        public static async Task Complete(int id, tbl_UserInformation user)
        {
            try
            {
                using (var db = new lmsDbContext())
                {
                    var curriculumDetail = await db.tbl_CurriculumDetailInClass.SingleOrDefaultAsync(x => x.Id == id);
                    if (curriculumDetail == null)
                        throw new Exception("Không tìm thấy chương");
                    curriculumDetail.IsComplete = true;
                    curriculumDetail.CompletePercent = 100;
                    await db.SaveChangesAsync();
                    var fileInCourses = await db.tbl_FileCurriculumInClass.Where(x => x.CurriculumDetailId == id && x.Enable == true).Select(x => x.Id).ToListAsync();
                    if (fileInCourses.Any())
                    {
                        foreach (var item in fileInCourses)
                            await FileCurriculumInClassSerivce.Complete(item, user);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static async Task<List<tbl_CurriculumDetailInClass>> UpdateCurriculumDetailIndex(List<CurriculumDetailInClassUpdate> request, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                List<tbl_CurriculumDetailInClass> curriculumDetails = new List<tbl_CurriculumDetailInClass>();
                foreach (var item in request)
                {
                    tbl_CurriculumDetailInClass curriculumDetail = await db.tbl_CurriculumDetailInClass.SingleOrDefaultAsync(x => x.Id == item.Id && x.Enable == true);
                    if (curriculumDetail != null)
                    {
                        curriculumDetail.Index = item.Index;
                        curriculumDetail.ModifiedBy = user.FullName;
                        curriculumDetail.ModifiedOn = DateTime.Now;
                        curriculumDetails.Add(curriculumDetail);
                    }
                }
                if (curriculumDetails.Any())
                {
                    await db.SaveChangesAsync();
                }

                return curriculumDetails;
            }
        }

    }
}