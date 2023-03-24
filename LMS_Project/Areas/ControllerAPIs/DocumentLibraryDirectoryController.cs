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
    [RoutePrefix("api/DocumentLibraryDirectory")]
    [ClaimsAuthorize]
    public class DocumentLibraryDirectoryController : BaseController
    {
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(DocumentLibraryDirectoryCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await DocumentLibraryDirectoryService.Insert(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> Update(DocumentLibraryDirectoryUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await DocumentLibraryDirectoryService.Update(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> GetAll([FromUri] DocumentLibraryDirectorySearch baseSearch)
        {
            var data = await DocumentLibraryDirectoryService.GetAll(baseSearch);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await DocumentLibraryDirectoryService.GetById(id, GetCurrentUser());
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data = data });
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await DocumentLibraryDirectoryService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
    }
}
