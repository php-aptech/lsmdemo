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
    [RoutePrefix("api/CertificateTemplate")]
    [ClaimsAuthorize]
    public class CertificateTemplateController : BaseController
    {
        [HttpGet]
        [Route("guide")]
        public HttpResponseMessage GetGuide()
        {
            var data = CertificateTemplateService.GetGuide();
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(CertificateTemplateCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CertificateTemplateService.Insert(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> Update(CertificateTemplateUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CertificateTemplateService.Update(model, GetCurrentUser());
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
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CertificateTemplateService.Delete(id, GetCurrentUser());
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
        public async Task<HttpResponseMessage> Get([FromUri] CertificateTemplateSearch search)
        {
            var data = await CertificateTemplateService.Get(search);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data.Data, totalRow = data.TotalRow });
        }
        [HttpGet]
        [Route("get-template")]
        public async Task<HttpResponseMessage> GetTemplate(int studentId, int certificateTemplateId)
        {
            try
            {
                var data = await CertificateTemplateService.GetTemplate(studentId, certificateTemplateId);
                if (data == null)
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
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
            try
            {
                var data = await CertificateTemplateService.GetById(id);
                if (data == null)
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
    }
}