using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static LMS_Project.Models.lmsEnum;
using static LMS_Project.Services.TranscriptService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [RoutePrefix("api/Transcript")]
    [ClaimsAuthorize]
    public class TranscriptController : BaseController
    {
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(TranscriptCreate model)
        {
            try
            {
                var data = await TranscriptService.Insert(model, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await TranscriptService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpPut]
        [Route("point-edit")]
        public async Task<HttpResponseMessage> PointEdit(TranscriptModel model)
        {
            try
            {
                await TranscriptService.PointEdit(model, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("by-class/{classId}")]
        public async Task<HttpResponseMessage> GetByClass(int classId)
        {
            var data = await TranscriptService.GetByClass(classId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("get-point-by-student-class")]
        public async Task<HttpResponseMessage> GetByStudentClass(int studentId, int classId)
        {
            var data = await TranscriptService.GetByStudentClass(studentId, classId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("point/{transcriptId}")]
        public async Task<HttpResponseMessage> GetPoint(int transcriptId)
        {
            var data = await TranscriptService.GetPoint(transcriptId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("student-point")]
        public async Task<HttpResponseMessage> GetStudentPoint([FromUri] PointSearch baseSearch)
        {
            var data = await TranscriptService.GetPointByStudent(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
    }
}
