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
    public class CurriculumInClassService
    {       
        public static async Task<tbl_CurriculumInClass> Update (CurriculumInClassUpdate itemModel, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_CurriculumInClass.SingleOrDefaultAsync(x => x.Id == itemModel.Id && x.Enable == true);
                if (entity == null)
                    throw new Exception("Không tìm thấy giáo trình");
                entity.Name = string.IsNullOrEmpty(itemModel.Name) ? entity.Name : itemModel.Name;
                await db.SaveChangesAsync();
                return entity;
            }
        }



        public static async Task<List<Get_CurriculumInClass>> GetCurriculumInClass(int classId)
        {
            using (var db = new lmsDbContext())
            {
                var parms = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@ClassId", Value= classId == null ? 0 : classId, DbType = DbType.Int64}
                };
                var sqlStr = $"Exec Get_CurriculumInClass @ClassId";
                var data = await db.Database.SqlQuery<Get_CurriculumInClass>(sqlStr, parms.ToArray()).ToListAsync();
                return data;
            }    
        }
    }


}