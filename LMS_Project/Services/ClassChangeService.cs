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
    public class ClassChangeService
    {
        public static async Task<tbl_ClassChange> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_ClassChange.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_ClassChange> Insert(ClassChangeCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var model = new tbl_ClassChange(itemModel);
                var studentInClass = await db.tbl_StudentInClass.FirstOrDefaultAsync(x => x.Id == itemModel.StudentInClassId && x.Enable == true);
                if (studentInClass == null)
                    throw new Exception("Không tìm thấy học viên trong lớp");

                var student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == studentInClass.StudentId);
                if (student == null)
                    throw new Exception("Không tìm thấy học viên");

                var oldClass = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == studentInClass.ClassId);
                if (oldClass == null)
                    throw new Exception("Không tìm thấy lớp cũ");
                model.StudentId = studentInClass.StudentId;
                model.OldClassId = oldClass.Id;
                model.OldPrice = oldClass.Price;
                var newClass = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == itemModel.NewClassId);
                if (newClass == null)
                    throw new Exception("Không tìm thấy lớp mới");

                if (newClass.Status == 3)
                    throw new Exception("Lớp học đã kết thúc");
                var checkExist = await db.tbl_StudentInClass.AnyAsync(x => x.StudentId == model.StudentId && x.ClassId == newClass.Id && x.Enable == true);
                if (checkExist)
                    throw new Exception($"Học viên đã có trong lớp {newClass.Name}");

                model.NewPrice = newClass.Price;
                model.CreatedBy = model.ModifiedBy = user.FullName;
                model.BranchId = newClass.BranchId;
                db.tbl_ClassChange.Add(model);
                studentInClass.Enable = false;//xóa ra khỏi lớp cũ
                var newStudentInClass = new tbl_StudentInClass//thêm vào lớp mới
                {
                    BranchId = model.BranchId,
                    ClassId = model.NewClassId,
                    CreatedBy = user.FullName,
                    CreatedOn = DateTime.Now,
                    Note = "Học viên chuyển lớp",
                    StudentId = model.StudentId,
                    Enable = true,
                    ModifiedBy = user.FullName,
                    ModifiedOn = DateTime.Now,
                    Type = 1,
                    TypeName = "Chính thức",
                    Warning = false

                };
                db.tbl_StudentInClass.Add(newStudentInClass);
                student.LearningStatus = 2;
                student.LearningStatusName = "Đang học";
                await db.SaveChangesAsync();
                return model;

            }
        }

        public static async Task<AppDomainResult> GetAll(ClassChangeSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new ClassChangeSearch();
                string myBranchIds = "";
                if (user.RoleId != ((int)RoleEnum.admin))
                    myBranchIds = user.BranchIds;
                int mySaleId = 0;
                if (user.RoleId == ((int)RoleEnum.sale))
                    mySaleId = user.UserInformationId;
                string sql = $"Get_ClassChange @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@BranhIds = N'{baseSearch.BranhIds}'," +
                    $"@MyBranchIds = N'{myBranchIds ?? ""}'," +
                    $"@SaleId = N'{mySaleId}'";
                var data = await db.Database.SqlQuery<Get_ClassChange>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_ClassChange(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }

    }
}