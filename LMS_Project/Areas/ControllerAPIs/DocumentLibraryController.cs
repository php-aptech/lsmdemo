using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LMS_Project.Areas.ControllerAPIs
{
    [RoutePrefix("api/DocumentLibrary")]
    [ClaimsAuthorize]
    public class DocumentLibraryController : BaseController
    {
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(DocumentLibraryCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await DocumentLibraryService.Insert(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> Update(DocumentLibraryUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await DocumentLibraryService.Update(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> GetAll([FromUri] DocumentLibrarySearch baseSearch)
        {
            var data = await DocumentLibraryService.GetAll(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await DocumentLibraryService.GetById(id, GetCurrentUser());
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
                await DocumentLibraryService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }

        /// <summary>
        /// Định dạng file của document library
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/FileInDocumentLibrary/UploadFile")]
        public HttpResponseMessage UploadFile()
        {
            try
            {
                string link = "";
                var httpContext = HttpContext.Current;
                var file = httpContext.Request.Files.Get("File");
                if (file != null)
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    string fileName = Guid.NewGuid() + ext; // getting File Name
                    string fileExtension = Path.GetExtension(fileName).ToLower();
                    var result = AssetCRM.IsValidDocument(ext); // Validate Header                 
                    if (result)
                    {
                        fileName = Guid.NewGuid() + ext;
                        var path = Path.Combine(httpContext.Server.MapPath("~/Upload/DocumentLibrary/"), fileName);
                        string strPathAndQuery = httpContext.Request.Url.PathAndQuery;
                        string strUrl = httpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                        link = strUrl + "Upload/DocumentLibrary/" + fileName;

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.InputStream.CopyTo(stream);
                        }

                        //file.SaveAs(path);
                        if (!link.Contains("https"))
                            link = link.Replace("http", "https");
                        return Request.CreateResponse(HttpStatusCode.OK, new { data = link, message = ApiMessage.SAVE_SUCCESS });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ApiMessage.INVALID_FILE });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ApiMessage.NOT_FOUND });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
            }
        }

    }
}
