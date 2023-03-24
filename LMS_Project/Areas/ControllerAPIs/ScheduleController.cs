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
using static LMS_Project.Services.ScheduleService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [RoutePrefix("api/Schedule")]
    [ClaimsAuthorize]
    public class ScheduleController : BaseController
    {
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await ScheduleService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(ScheduleCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ScheduleService.Insert(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> Update(ScheduleUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ScheduleService.Update(model, GetCurrentUser());
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
        /// Hủy lịch dạy kèm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("tutoring-cancel/{id}")]
        public async Task<HttpResponseMessage> TutoringCancel(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ScheduleService.TutoringCancel(id, GetCurrentUser());
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
                await ScheduleService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        /// <summary>
        /// Xem lịch
        /// </summary>
        /// <remarks>
        /// Xem lịch theo lớp truyền classId <br></br>
        /// Xem lịch học của học viên và lịch dạy của giáo viên api tự bắt theo token  <br></br>
        /// Kiểm tra lịch trung tâm và giáo viên BranchIds, TeacherIds  <br></br>
        /// </remarks>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll([FromUri] ScheduleSearch baseSearch)
        {
            var data = await ScheduleService.GetAll(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        [HttpPut]
        [Route("rate-teacher")]
        public async Task<HttpResponseMessage> RateTeacher(RateTeacherModel itemModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ScheduleService.RateTeacher(itemModel, GetCurrentUser());
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

        #region phần zoom

        [HttpPost]
        [Route("create-zoom/{scheduleId}")]
        public async Task<HttpResponseMessage> CreateZoom(int scheduleId)
        {
            try
            {
                var data = await ScheduleService.CreateZoom(scheduleId, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }

        [HttpPut]
        [Route("close-zoom/{scheduleId}")]
        public async Task<HttpResponseMessage> CloseZoom(int scheduleId)
        {
            try
            {
                await ScheduleService.CloseZoom(scheduleId, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("recording/{scheduleId}")]
        public async Task<HttpResponseMessage> GetRecording(int scheduleId)
        {
            try
            {
                var data = await ScheduleService.GetRecording(scheduleId);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            }
            catch (Exception e)
            {
                return Request
                        .CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("zoom-room")]
        public async Task<HttpResponseMessage> GetZoomRoom([FromUri] SearchOptions baseSearch)
        {
            var data = await ScheduleService.GetZoomRoom(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        #endregion
    }
}
