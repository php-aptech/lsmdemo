using LMS_Project.Areas.Models;
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
    [RoutePrefix("api/NewsFeedReply")]
    [ClaimsAuthorize]
    public class NewsFeedReplyController : BaseController
    {

        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(NewsFeedReplyCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await NewsFeedReplyService.Insert(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> Update(NewsFeedReplyUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await NewsFeedReplyService.Update(model, GetCurrentUser());
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
                await NewsFeedReplyService.Delete(id, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll([FromUri] NewsFeedReplySearch baseSearch)
        {
            var data = await NewsFeedReplyService.GetAll(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
    }
}
