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
using static LMS_Project.Services.ExamResultService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class ExamResultController : BaseController
    {
        [HttpGet]
        [Route("api/ExamResult/{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await ExamResultService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("api/ExamResult/Submit")]
        public async Task<HttpResponseMessage> Submit(ExamSubmit model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ExamResultService.Submit(model, GetCurrentUser());
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
        [Route("api/ExamResult")]
        public async Task<HttpResponseMessage> GetAll([FromUri] ExamResultSearch search)
        {
            var data = await ExamResultService.GetAll(search);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("api/ExamResult/Detail/{examResultId}")]
        public async Task<HttpResponseMessage> GetDetail(int examResultId)
        {
            var data = await ExamResultService.GetDetail(examResultId);
            if (data.Data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data.Data });
        }
        /// <summary>
        /// Chọn giáo viên chấm bài
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ExamResult/add-teacher")]
        public async Task<HttpResponseMessage> AddTeacher([FromBody] AddTeacherModel itemModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ExamResultService.AddTeacher(itemModel, GetCurrentUser());
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
        /// <summary>
        /// Giáo viên chấm bài
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ExamResult/mark")]
        public async Task<HttpResponseMessage> Mark([FromBody] MarkModel itemModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ExamResultService.Mark(itemModel, GetCurrentUser());
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
    }
}
