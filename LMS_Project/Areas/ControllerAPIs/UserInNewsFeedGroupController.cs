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
    [RoutePrefix("api/UserInNewsFeedGroup")]
    [ClaimsAuthorize]
    public class UserInNewsFeedGroupController : BaseController
    {
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(UserInNFGroupCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await UserInNFGroupService.Insert(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> Update(UserInNFGroupUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await UserInNFGroupService.Update(model, GetCurrentUser());
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
                await UserInNFGroupService.Delete(id, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }


        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll([FromUri] UserInNFGroupSearch baseSearch)
        {
            var data = await UserInNFGroupService.GetAll(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }




        [HttpGet]
        [Route("user-not-in-group/{groupId}")]
        public async Task<HttpResponseMessage> GetUserNotInGroup(int? groupId)
        {
            var data = await UserInNFGroupService.GetUserNotInGroup(groupId);
            if (data.Data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data.Data });
        }
    }
}
