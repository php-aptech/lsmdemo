using ExcelDataReader;
using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using static LMS_Project.Models.lmsEnum;
using static LMS_Project.Services.LessonVideoService;
using WMPLib;
namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class LessonVideoController : BaseController
    {
        [HttpPost]
        [Route("api/LessonVideo")]
        public async Task<HttpResponseMessage> Insert(LessonVideoCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var httpContext = HttpContext.Current;
                    var data = await LessonVideoService.Insert(
                        model, 
                        GetCurrentUser(),
                        httpContext.Server.MapPath("~/Upload/FileInVideo/"));
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }
        [HttpPut]
        [Route("api/LessonVideo")]
        public async Task<HttpResponseMessage> Update(LessonVideoUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var httpContext = HttpContext.Current;
                    var data = await LessonVideoService.Update(
                        model, 
                        GetCurrentUser(),
                        httpContext.Server.MapPath("~/Upload/FileInVideo/"));
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }
        [HttpDelete]
        [Route("api/LessonVideo/{id}")] 
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await LessonVideoService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }

        [HttpPut]
        [Route("api/LessonVideo/ChangeIndex")]
        public async Task<HttpResponseMessage> LessonVideoChangeIndex([FromBody] ChangeLessonIndexModel model)
        {
            try
            {
                await LessonVideoService.ChangeIndex(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }

        [HttpGet]
        [Route("api/LessonVideo/GetBySection/{sectionId}")]
        public async Task<HttpResponseMessage> GetBySection(int sectionId)
        {
            var data = await LessonVideoService.GetBySection(sectionId,GetCurrentUser());
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("api/FileInVideo")]
        public async Task<HttpResponseMessage> InsertFileInVideo(FileInVideoCreate fileInVideoCreate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await FileInVideoService.Insert(fileInVideoCreate, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }
        [HttpDelete]
        [Route("api/FileInVideo/{id}")]
        public async Task<HttpResponseMessage> DeleteFileInVideo(int id)
        {
            try
            {
                await FileInVideoService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/FileInVideo/GetByLesson/{lessonVideoId}")]
        public async Task<HttpResponseMessage> GetByLesson(int lessonVideoId)
        {
            var data = await FileInVideoService.GetByLesson(lessonVideoId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("api/FileInVideo/UploadFile")]
        public HttpResponseMessage UploadFile()
        {
            try
            {
                string link = "";
                var httpContext = HttpContext.Current;
                var file = httpContext.Request.Files.Get("File");
                if (file != null)
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    string fileName = Guid.NewGuid() + ext; // getting File Name
                    string fileExtension = Path.GetExtension(fileName).ToLower();
                    var result = AssetCRM.IsValidDocument(ext); // Validate Header
                    if (result)
                    {
                        fileName = Guid.NewGuid() + ext;
                        var path = Path.Combine(httpContext.Server.MapPath("~/Upload/FileInVideo/"), fileName);
                        string strPathAndQuery = httpContext.Request.Url.PathAndQuery;
                        string strUrl = httpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                        link = strUrl + "Upload/FileInVideo/" + fileName;
                        file.SaveAs(path);
                        if (!link.Contains("https"))
                            link = link.Replace("http", "https");
                        return Request.CreateResponse(HttpStatusCode.OK, new { data = link, message = ApiMessage.SAVE_SUCCESS });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ApiMessage.INVALID_FILE });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ApiMessage.NOT_FOUND });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
            }
        }
        [HttpGet]
        [Route("api/FileInVideo/UploadFile/File")]
        public HttpResponseMessage GetFileInVideoUpload()
        {
            string result = "jpg,jpeg,png" +
                ",bmp" +
                ",mp4" +
                ",flv" +
                ",mpeg" +
                ",mov" +
                ",mp3" +
                ",doc" +
                ",docx" +
                ",pdf" +
                ",csv" +
                ",xlsx" +
                ",xls" +
                ",ppt" +
                ",pptx" +
                ",zip" +
                ",rar";
            return Request.CreateResponse(HttpStatusCode.OK, new { data = result, message = "Thành công" });
        }
        [HttpPost]
        [Route("api/LessonVideo/Completed/{lessonVideoId}")]
        public async Task<HttpResponseMessage> Completed(int lessonVideoId)
        {
            try
            {
                await LessonVideoService.Completed(lessonVideoId, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        //[HttpPost]
        //[Route("api/LessonVideo/test")]
        //public async Task<HttpResponseMessage> test()
        //{
        //    try
        //    {
        //        var httpContext = HttpContext.Current;
        //        double time = 0;
        //        var player = new WindowsMediaPlayer();
        //        var clip = player.newMedia($"{httpContext.Server.MapPath("~/Upload/Mau/")}/test.wav");
        //        time = clip.duration;
        //        return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", time });
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
        //    }
        //}
    }
}
