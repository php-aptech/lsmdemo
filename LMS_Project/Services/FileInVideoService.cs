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
    public class FileInVideoService
    {
        public static async Task<tbl_FileInVideo> Insert(FileInVideoCreate fileInVideoCreate, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var lessonVideo = await db.tbl_LessonVideo.AnyAsync(x => x.Id == fileInVideoCreate.LessonVideoId);
                    if (!lessonVideo)
                        throw new Exception("Không tìm thấy bài học");
                    var model = new tbl_FileInVideo(fileInVideoCreate);
                    model.CreatedBy = model.ModifiedBy = user.FullName;
                    db.tbl_FileInVideo.Add(model);
                    await db.SaveChangesAsync();
                    return model;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var fileInVideo = await db.tbl_FileInVideo.SingleOrDefaultAsync(x => x.Id == id);
                    if (fileInVideo == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    fileInVideo.Enable = false;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<List<tbl_FileInVideo>> GetByLesson(int lessonVideoId)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_FileInVideo.Where(x => x.LessonVideoId == lessonVideoId && x.Enable == true).OrderBy(x => x.Id).ToListAsync();
                return data;
            }
        }
    }
}