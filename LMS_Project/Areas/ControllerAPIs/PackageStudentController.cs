using ExcelDataReader;
using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class PackageStudentController : BaseController
    {
        [HttpDelete]
        [Route("api/PackageStudent/{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await PackageStudentService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpPost]
        [Route("api/PackageStudent")]
        public async Task<HttpResponseMessage> Insert(PackageStudentCreate itemModel)
        {
            try
            {
                var data = await PackageStudentService.Insert(itemModel,GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/PackageStudent")]
        public async Task<HttpResponseMessage> GetAll([FromUri] PackageStudentSearch search)
        {
            var data = await PackageStudentService.GetAll(search, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("api/PackageStudent/{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await PackageStudentService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
    }
}
