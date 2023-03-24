using LMS_Project.Areas.Request;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LMS_Project.Areas.ControllerAPIs
{
    /// <summary>
    /// lộ trình của học viên
    /// </summary>
    [RoutePrefix("api/StudyRoute")]
    [ClaimsAuthorize]
    public class StudyRouteController : BaseController
    {
        /// <summary>
        /// thêm lộ trình cho học viên
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route()]
        public async Task<HttpResponseMessage> Insert(StudyRouteCreate request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await StudyRouteService.Insert(request, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = result });
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
        /// cập nhật lộ trình
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route()]
        public async Task<HttpResponseMessage> Update(StudyRouteUpdate request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await StudyRouteService.Update(request, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = result });
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
        /// thay đổi thứ tự lộ trình
        /// </summary>
        /// <param name="IdUp"></param>
        /// <param name="IdDown"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update-index")]
        public async Task<HttpResponseMessage> UpdateIndex([FromBody]UpdateIndex request)
        {            
            try
            {
                var result = await StudyRouteService.UpdateIndex(request.IdUp, request.IdDown, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = result });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }           
        }
        
        /// <summary>
        /// xem lộ trình của học viên
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("student-study-route")]
        public async Task<HttpResponseMessage> StudentStudyRoute(string studentIds = null, string parentIds = null)
        {
            var result = await StudyRouteService.GetStudyRouteOfStudent(studentIds, parentIds);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = result });
        }
        /// <summary>
        /// xóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                var result = await StudyRouteService.Delete(id, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = result });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
    }
    public class UpdateIndex
    {
        public int IdUp { get; set; }
        public int IdDown { get; set; }
    }
}
