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
    public class ClassRegistrationService
    {
        public static async Task<tbl_ClassRegistration> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_ClassRegistration.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public class AddToClassModel
        { 
            public List<int> ClassRegistrationIds { get; set; }
            public int classId { get; set; }
        }
        public static async Task AddToClass(AddToClassModel model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var _class = await db.tbl_Class.SingleOrDefaultAsync(x => x.Id == model.classId);
                        if (_class == null)
                            throw new Exception("Không tìm thấy lớp");
                        if (_class.Status == 3)
                            throw new Exception("Lớp học đã kết thúc");
                        var countStudentInClass = await db.tbl_StudentInClass.CountAsync(x => x.ClassId == _class.Id && x.Enable == true);
                        if (_class.MaxQuantity < (countStudentInClass + model.ClassRegistrationIds.Count()))
                            throw new Exception("Lớp không đủ chỗ");

                        if (!model.ClassRegistrationIds.Any())
                            throw new Exception("Không tìm thấy dữ liệu");

                        foreach (var item in model.ClassRegistrationIds)
                        {
                            var classRegistration = await db.tbl_ClassRegistration.SingleOrDefaultAsync(x => x.Id == item);
                            if (classRegistration == null)
                                throw new Exception("Không tìm thấy học viên chờ xếp lớp");
                            var student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == classRegistration.StudentId);
                            if (student == null)
                                throw new Exception("Không tìm thấy học viên");

                            if (classRegistration.Status == 2)
                                throw new Exception($"Học viên {student.FullName} đã xếp lớp");
                            if (classRegistration.Status == 3)
                                throw new Exception($"Học viên {student.FullName} đã hoàn tiền");

                            var checkExist = await db.tbl_StudentInClass.AnyAsync(x => x.StudentId == classRegistration.StudentId && x.ClassId == model.classId && x.Enable == true);
                            if (checkExist)
                                throw new Exception($"Học viên {student.FullName} đã có trong lớp {_class.Name}");

                            var newStudentInClass = new tbl_StudentInClass//thêm vào lớp mới
                            {
                                BranchId = classRegistration.BranchId,
                                ClassId = _class.Id,
                                CreatedBy = user.FullName,
                                CreatedOn = DateTime.Now,
                                Note = "Học viên chờ xếp lớp chuyển vào lớp",
                                StudentId = classRegistration.StudentId,
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

                            classRegistration.Status = 2;
                            classRegistration.StatusName = "Đã xếp lớp";

                            await db.SaveChangesAsync();
                        }
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

        public static async Task<tbl_ClassRegistration> Update(ClassRegistrationUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_ClassRegistration.SingleOrDefaultAsync(x => x.Id == itemModel.Id.Value);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Note = itemModel.Note ?? entity.Note;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public static async Task<AppDomainResult> GetAll(ClassRegistrationSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new ClassRegistrationSearch();
                string myBranchIds = "";
                if (user.RoleId != ((int)RoleEnum.admin))
                    myBranchIds = user.BranchIds;
                int mySaleId = 0;
                if (user.RoleId == ((int)RoleEnum.sale))
                    mySaleId = user.UserInformationId;
                string sql = $"Get_ClassRegistration @Search = N'{baseSearch.Search ?? ""}', @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@BranhIds = N'{baseSearch.BranhIds}'," +
                    $"@ProgramIds = N'{baseSearch.ProgramIds}'," +
                    $"@Status = N'{baseSearch.Status}'," +
                    $"@MyBranchIds = N'{myBranchIds ?? ""}'," +
                    $"@SaleId = N'{mySaleId}'";
                var data = await db.Database.SqlQuery<Get_ClassRegistration>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_ClassRegistration(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
    }
}