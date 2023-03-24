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

namespace LMS_Project.Areas.ControllerAPIs
{
    [RoutePrefix("api/GeneralNotification")]
    [ClaimsAuthorize]
    public class GeneralNotificationController : BaseController
    {
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(GeneralNotificationCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await GeneralNotificationService.Insert(model, GetCurrentUser());
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
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await GeneralNotificationService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll([FromUri] SearchOptions baseSearch)
        {
            var data = await GeneralNotificationService.GetAll(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("receiver/{id}")]
        public async Task<HttpResponseMessage> GetReceiver(int id)
        {
            var data = await GeneralNotificationService.GetReceiver(id);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
    }
}
