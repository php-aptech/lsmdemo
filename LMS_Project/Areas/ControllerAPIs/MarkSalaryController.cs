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
    [RoutePrefix("api/MarkSalary")]
    [ClaimsAuthorize]
    public class MarkSalaryController : BaseController
    {
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await MarkSalaryService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("insert-or-update")]
        public async Task<HttpResponseMessage> InsertOrUpdate(MarkSalaryCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await MarkSalaryService.InsertOrUpdate(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> GetAll([FromUri] MarkSalarySearch baseSearch)
        {
            var data = await MarkSalaryService.GetAll(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
    }
}
