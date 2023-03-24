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
    [RoutePrefix("api/Feedback")]
    [ClaimsAuthorize]
    public class FeedbackController : BaseController
    {
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(FeedbackCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await FeedbackService.Insert(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> Update(FeedbackUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await FeedbackService.Update(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> GetAll([FromUri] FeedbackSearch baseSearch)
        {
            var data = await FeedbackService.GetAll(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await FeedbackService.GetById(id, GetCurrentUser());
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data = data });
        }

        [HttpPut]
        [Route("rating-feedback/{id}")]
        public async Task<HttpResponseMessage> RatingFeedback(int id, int rating)
        {
            try
            {
                await FeedbackService.RatingFeedBack(id, rating,GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }

    }
}
