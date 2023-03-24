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
    public class ClassReserveService
    {
        public static async Task<tbl_ClassReserve> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_ClassReserve.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_ClassReserve> Insert(ClassReserveCreate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var model = new tbl_ClassReserve(itemModel);
                var studentInClass = await db.tbl_StudentInClass.FirstOrDefaultAsync(x => x.Id == itemModel.StudentInClassId && x.Enable == true);
                if (studentInClass == null)
                    throw new Exception("Không tìm thấy học viên trong lớp");
                var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == studentInClass.ClassId);
                if (_class == null)
                    throw new Exception("Không tìm thấy lớp");
                if (_class.Status == 3)
                    throw new Exception("Lớp đã kết thúc không thể bảo lưu");
                model.StudentId = studentInClass.StudentId;
                model.ClassId = _class.Id;
                model.BranchId = _class.BranchId;
                model.Price = _class.Price;
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_ClassReserve.Add(model);
                studentInClass.Enable = false;
                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task<tbl_ClassReserve> Update(ClassReserveUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_ClassReserve.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Note = itemModel.Note ?? entity.Note;
                entity.Expires = itemModel.Expires ?? entity.Expires;
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
                var entity = await db.tbl_ClassReserve.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public class AddToClassFromReserveModel
        { 
            public int ClassReserveId { get; set; }
            public int ClassId { get; set; }
        }
        public static async Task AddToClassFromReserve(AddToClassFromReserveModel model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var classReserve = await db.tbl_ClassReserve.FirstOrDefaultAsync(x => x.Id == model.ClassReserveId && x.Enable == true);
                if (classReserve == null)
                    throw new Exception("Không tìm thấy thông tin bảo lưu");
                if (classReserve.Status != 1)
                {
                    string mess = "";
                    switch (classReserve.Status)
                    {
                        case 2: mess = "Học viên đã học lại"; break;
                        case 3: mess = "Đã hoàn tiền cho học viên"; break;
                        case 4: mess = "Dã hết hạn bảo lưu"; break;
                    }
                    throw new Exception(mess);
                }
                var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == model.ClassId);
                if (_class == null)
                    throw new Exception("Không tìm thấy lớp học");
                if (_class.Status == 3)
                    throw new Exception("Lớp học đã kết thúc");

                var countStudentInClass = await db.tbl_StudentInClass.CountAsync(x => x.ClassId == _class.Id && x.Enable == true);
                if (_class.MaxQuantity < countStudentInClass)
                    throw new Exception("Lớp không đủ chỗ");

                var student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == classReserve.StudentId);
                if (student == null)
                    throw new Exception("Không tìm thấy học viên");

                var checkExist = await db.tbl_StudentInClass.AnyAsync(x => x.StudentId == classReserve.StudentId && x.ClassId == _class.Id && x.Enable == true);
                if (checkExist)
                    throw new Exception($"Học viên {student.FullName} đã có trong lớp {_class.Name}");

                var newStudentInClass = new tbl_StudentInClass//thêm vào lớp mới
                {
                    BranchId = _class.BranchId,
                    ClassId = _class.Id,
                    CreatedBy = user.FullName,
                    CreatedOn = DateTime.Now,
                    Note = "Học viên bảo lưu chuyển vào lớp",
                    StudentId = classReserve.StudentId,
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
                classReserve.Status = 2;
                classReserve.StatusName = "Đã học lại";

                await db.SaveChangesAsync();
            }
        }
        public static async Task AutoUpdateStatus()
        {
            using (var db = new lmsDbContext())
            {
                var timenow = DateTime.Now;
                var classReserves = await db.tbl_ClassReserve.Where(x => x.Status == 1 && x.Enable == true && timenow > x.Expires).ToListAsync();
                if (classReserves.Any())
                {
                    foreach (var item in classReserves)
                    {
                        item.Status = 4;
                        item.StatusName = "Hết hạn bảo lưu";
                    }
                }
                await db.SaveChangesAsync();
            }
        }

        public static async Task<AppDomainResult> GetAll(ClassReserveSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new ClassReserveSearch();
                string myBranchIds = "";
                if (user.RoleId != ((int)RoleEnum.admin))
                    myBranchIds = user.BranchIds;
                int mySaleId = 0;
                if (user.RoleId == ((int)RoleEnum.sale))
                    mySaleId = user.UserInformationId;
                string sql = $"Get_ClassReserve @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@BranhIds = N'{baseSearch.BranhIds}'," +
                    $"@Status = N'{baseSearch.Status}'," +
                    $"@MyBranchIds = N'{myBranchIds ?? ""}'," +
                    $"@SaleId = N'{mySaleId}'";
                var data = await db.Database.SqlQuery<Get_ClassReserve>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_ClassReserve(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
    }
}