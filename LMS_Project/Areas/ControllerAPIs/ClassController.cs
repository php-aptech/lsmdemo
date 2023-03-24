using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static LMS_Project.Models.lmsEnum;
using static LMS_Project.Services.ClassService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [RoutePrefix("api/Class")]
    [ClaimsAuthorize]
    public class ClassController : BaseController
    {
        /// <summary>
        /// Tải lịch học khi tạo lớp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("lesson-when-create")]
        public async Task<HttpResponseMessage> GetLessonWhenCreate([FromBody] LessonSearch itemModel)
        {
            try
            {
                var data = await ClassService.GetLessonWhenCreate(itemModel);
                if (!data.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await ClassService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("teacher-when-create")]
        public async Task<HttpResponseMessage> GetTeacherWhenCreate([FromUri] TeacherSearch itemModel)
        {
            var data = await ClassService.GetTeacherWhenCreate(itemModel);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        /// <summary>
        /// true - lớp đang học (có học viện trong lớp)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("check-exist-student-in-class/{id}")]
        public async Task<HttpResponseMessage> CheckExistStudentInClass(int id)
        {
            var data = await ClassService.CheckExistStudentInClass(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        /// <summary>
        /// lấy danh sách giáo viên khi tạo lịch
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("teacher-available")]
        public async Task<HttpResponseMessage> GetTeacherAvailable([FromUri] TeacherAvailableSearch baseSearch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ClassService.GetTeacherAvailable(baseSearch);
                    if (!data.Any())
                        return Request.CreateResponse(HttpStatusCode.NoContent);
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
        /// <summary>
        /// lấy danh sách phòng khi tạo lịch
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("room-available")]
        public async Task<HttpResponseMessage> GetRoomAvailable([FromUri] RoomAvailableSearch baseSearch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ClassService.GetRoomAvailable(baseSearch);
                    if (!data.Any())
                        return Request.CreateResponse(HttpStatusCode.NoContent);
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
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(ClassCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ClassService.Insert(model, GetCurrentUser());
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
        [Route("")]
        public async Task<HttpResponseMessage> Update(ClassUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ClassService.Update(model, GetCurrentUser());
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
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await ClassService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll([FromUri] ClassSearch baseSearch)
        {
            var data = await ClassService.GetAll(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("roll-up-teacher")]
        public async Task<HttpResponseMessage> GetRollUpTeacher([FromUri] RollUpTeacherSearch baseSearch)
        {
            var data = await ClassService.GetRollUpTeacher(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpPost]
        [Route("roll-up-teacher/{scheduleId}")]
        public async Task<HttpResponseMessage> RollUpTeacher(int scheduleId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ClassService.RollUpTeacher(scheduleId);
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }

        [HttpGet]
        [Route("schedule-in-date-now/{branchId}")]
        public async Task<HttpResponseMessage> GetScheduleInDateNow(int branchId)
        {
            var data = await ClassService.GetScheduleInDateNow(branchId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data });
        }


        /// <summary>
        /// lấy danh sách giáo viên khi đăng ký lịch lớp dạy kèm
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("teacher-tutoring-available")]
        public async Task<HttpResponseMessage> GetTeacherTutoringAvailable([FromUri] TeacherTutoringAvailableSearch baseSearch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ClassService.GetTeacherTutoringAvailable(baseSearch);
                    if (data.TotalRow == 0)
                        return Request.CreateResponse(HttpStatusCode.NoContent);
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data.Data, totalRow = data.TotalRow });
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
        [Route("tutoring-config")]
        public async Task<HttpResponseMessage> TutoringConfig([FromBody] TutoringConfigModel itemModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ClassService.TutoringConfig(itemModel);
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

        [HttpGet]
        [Route("tutoring-config")]
        public async Task<HttpResponseMessage> GetTutoringConfig()
        {
            var data = await ClassService.GetTutoringConfig();
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }

        [HttpGet]
        [Route("tutoring-curriculum")]
        public async Task<HttpResponseMessage> TutoringCurriculum(int classId)
        {
            var data = await ClassService.TutoringCurriculum(classId);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }

        #region Phần giáo trình trong lớp

        [HttpPost]
        [Route("curriculum-detail-in-class/complete/{curriculumDetailInClassId}")]
        public async Task<HttpResponseMessage> CurriculumDetailInClassComplete(int curriculumDetailInClassId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await CurriculumDetailInClassService.Complete(curriculumDetailInClassId, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !"});
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }
        [HttpPost]
        [Route("file-curriculum-in-class/complete/{fileCurriculumInClassId}")]
        public async Task<HttpResponseMessage> FileCurriculumInClassComplete(int fileCurriculumInClassId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await FileCurriculumInClassSerivce.Complete(fileCurriculumInClassId, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }
        [HttpGet]
        [Route("curriculum-in-class/{classId}")]
        public async Task<HttpResponseMessage> GetCurriculumInClass(int classId)
        {
            var data = await CurriculumInClassService.GetCurriculumInClass(classId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("curriculum-details-in-class")]
        public async Task<HttpResponseMessage> GetCurriculumDetailInClass([FromUri]CurriculumDetailInClassSearch baseSearch)
        {
            var data = await CurriculumDetailInClassService.GetCurriculumDetailInClass(baseSearch,GetCurrentUser());
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("file-curriculum-in-class")]
        public async Task<HttpResponseMessage> GetFileCurriculumInClass([FromUri]FilesCurriculumInClassSearch baseSearch)
        {
            var data = await FileCurriculumInClassSerivce.GetFileCurriculumInClass(baseSearch,GetCurrentUser());
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPut]
        [Route("curriculum-in-class")]
        public async Task<HttpResponseMessage> UpdateCurriculumInClass(CurriculumInClassUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CurriculumInClassService.Update(model, GetCurrentUser());
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
        [Route("curriculum-detail-in-class")]
        public async Task<HttpResponseMessage> UpdateCurriculumDetailInClass(CurriculumDetailInClassUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CurriculumDetailInClassService.Update(model, GetCurrentUser());
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
        [HttpPost]
        [Route("curriculum-detail-in-class")]
        public async Task<HttpResponseMessage> AddCurriculumDetailInClass(CurriculumDetailInClassCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CurriculumDetailInClassService.Insert(model, GetCurrentUser());
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
        [Route("curriculum-detail-in-class/{id}")]
        public async Task<HttpResponseMessage> DeleteCurriculumDetailInClass(int id)
        {
            try
            {
                await CurriculumDetailInClassService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpPut]
        [Route("curriculum-detail-in-class-index")]
        public async Task<HttpResponseMessage> UpdateCurriculumDetailIndex(List<CurriculumDetailInClassUpdate> request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CurriculumDetailInClassService.UpdateCurriculumDetailIndex(request, GetCurrentUser());
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
        [HttpPost]
        [Route("file-curriculum-in-class/{curriculumInClassDetailId}")]
        public async Task<HttpResponseMessage> AddFileCurriculumInClass(int curriculumInClassDetailId)
        {
            try
            {
                var curriculumDetail = await CurriculumDetailInClassService.GetById(curriculumInClassDetailId);
                if (curriculumDetail == null)
                    throw new Exception("Không tìm thấy danh mục");
                //Kiểm tra có tồn tại file nào của chương chưa thì gắn index
                string link = "";
                var httpContext = HttpContext.Current;
                var file = httpContext.Request.Files.Get("File");
                if (file != null)
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    string fileName = Guid.NewGuid() + ext; // getting File Name
                    string fileExtension = Path.GetExtension(fileName).ToLower();
                    var result = AssetCRM.isValIdDocument(ext); // ValIdate Header
                    if (result)
                    {
                        fileName = Guid.NewGuid() + ext;
                        var path = Path.Combine(httpContext.Server.MapPath("~/Upload/FileInCurriculum/"), fileName);
                        string strPathAndQuery = httpContext.Request.Url.PathAndQuery;
                        string strUrl = httpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                        link = strUrl + "Upload/FileInCurriculum/" + fileName;

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.InputStream.CopyTo(stream);
                        }

                        //file.SaveAs(path);
                        var model = new FileInCurriculumDetailCreate { CurriculumDetailId = curriculumInClassDetailId, FileName = file.FileName, FileUrl = link };
                        var data = await FileCurriculumInClassSerivce.Insert(model, GetCurrentUser());
                        return Request.CreateResponse(HttpStatusCode.OK, new { data = data, message = ApiMessage.SAVE_SUCCESS });
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
        [HttpPut]
        [Route("file-curriculum-in-class")]
        public async Task<HttpResponseMessage> UpdateFileCurriculumInClass(FileCurriculumInClassUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await FileCurriculumInClassSerivce.Update(model, GetCurrentUser());
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
        [Route("file-curriculum-in-class/{fileId}")]
        public async Task<HttpResponseMessage> DeleteFileCurriculumInClass(int fileId)
        {
            try
            {
                await FileCurriculumInClassSerivce.Delete(fileId);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpPut]
        [Route("hide-file-curriculum-in-class/{id}")]
        public async Task<HttpResponseMessage> HideFileCurriculumInClass(int id)
        {
            try
            {
                await FileCurriculumInClassSerivce.Hide(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }

        [HttpPut]
        [Route("hide-curriculum-detail-in-class/{id}")]
        public async Task<HttpResponseMessage> HideCurriculumDetailInClass(int id)
        {
            try
            {
                await CurriculumDetailInClassService.Hide(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }

        [HttpPut]
        [Route("file-curriculum-in-class-index")]
        public async Task<HttpResponseMessage> FileCurriculumInClassIndex(List<FileCurriculumInClassUpdate> request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await FileCurriculumInClassSerivce.FileCurriculumInClassIndex(request, GetCurrentUser());
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
        #endregion
    }
}
