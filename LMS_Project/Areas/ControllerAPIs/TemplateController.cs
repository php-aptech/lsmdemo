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
    [RoutePrefix("api/Template")]
    public class TemplateController : BaseController
    {
        [HttpGet]
        [Route("guide/{type}")]
        [ClaimsAuthorize]
        public async Task<HttpResponseMessage> GetGuide(int type)
        {
            var data = await TemplateService.GetGuide(type);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPut]
        [Route("")]
        [ClaimsAuthorize]
        public async Task<HttpResponseMessage> Update(TemplateUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await TemplateService.Update(model, GetCurrentUser());
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
        [Route("")]
        [ClaimsAuthorize]
        public async Task<HttpResponseMessage> GetAll()
        {
            var data = await TemplateService.GetAll();
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }

        [HttpGet]
        [Route("by-type/{type}")]
        public async Task<HttpResponseMessage> GetByType(int type)
        {
            var data = await TemplateService.GetByType(type);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
    }
}
