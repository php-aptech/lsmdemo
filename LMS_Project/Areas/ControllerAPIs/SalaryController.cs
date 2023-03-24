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
using static LMS_Project.Services.SalaryService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [RoutePrefix("api/Salary")]
    [ClaimsAuthorize]
    public class SalaryController : BaseController
    {
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await SalaryService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(SalaryCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await SalaryService.Insert(model, GetCurrentUser());
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
        [HttpPost]
        [Route("salary-closing")]
        public async Task<HttpResponseMessage> SalaryClosing()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await SalaryService.SalaryClosing(GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !"});
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
        public async Task<HttpResponseMessage> Update(SalaryUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await SalaryService.Update(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> GetAll([FromUri] SalarySearch baseSearch)
        {
            var data = await SalaryService.GetAll(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("teaching-detail")]
        public async Task<HttpResponseMessage> GetTeachingDetail([FromUri] TeachingDetailSearch baseSearch)
        {
            var data = await SalaryService.GetTeachingDetail(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        [HttpGet]
        [Route("user-available")]
        public async Task<HttpResponseMessage> GetUserAvailable([FromUri] UserAvailableSearch baseSearch)
        {
            var data = await SalaryService.GetUserAvailable(baseSearch);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
    }
}
