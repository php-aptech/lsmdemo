using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
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
using System.Web.Helpers;
using System.Web.Http;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Areas.ControllerAPIs
{
    [RoutePrefix("api/CurriculumDetail")]
    [ClaimsAuthorize]
    public class CurriculumDetailController : BaseController
    {
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await CurriculumDetailService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(CurriculumDetailCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CurriculumDetailService.Insert(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> Update(CurriculumDetailUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CurriculumDetailService.Update(model, GetCurrentUser());
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
                await CurriculumDetailService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll([FromUri] CurriculumDetailSearch baseSearch)
        {
            var data = await CurriculumDetailService.GetAll(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpPost]
        [Route("file/{curriculumDetailId}")]
        [ClaimsAuthorize]
        public async Task<HttpResponseMessage> AddFile(int curriculumDetailId)
        {
            try
            {
                var curriculumDetail = await CurriculumDetailService.GetById(curriculumDetailId);
                if (curriculumDetail == null)
                    throw new Exception("Không tìm thấy danh mục");
                //Kiểm tra có tồn tại file nào của chương chưa thì gắn index
                string link = "";
                var httpContext = HttpContext.Current;
                var file = httpContext.Request.Files.Get("File");
                if (file != null)
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    string fileName = Guid.NewGuid() + ext; // getting File Name
                    string fileExtension = Path.GetExtension(fileName).ToLower();
                    var result = AssetCRM.isValIdDocument(ext); // ValIdate Header
                    if (result)
                    {
                        fileName = Guid.NewGuid() + ext;
                        var path = Path.Combine(httpContext.Server.MapPath("~/Upload/FileInCurriculum/"), fileName);
                        string strPathAndQuery = httpContext.Request.Url.PathAndQuery;
                        string strUrl = httpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                        link = strUrl + "Upload/FileInCurriculum/" + fileName;

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.InputStream.CopyTo(stream);
                        }

                        //file.SaveAs(path);
                        var model = new FileInCurriculumDetailCreate { CurriculumDetailId = curriculumDetailId, FileName = file.FileName, FileUrl = link};
                        var data = await FileInCurriculumDetailService.Insert(model,GetCurrentUser());
                        return Request.CreateResponse(HttpStatusCode.OK, new { data = data, message = ApiMessage.SAVE_SUCCESS });
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
        [HttpPut]
        [Route("file")]
        public async Task<HttpResponseMessage> UpdateFile(FileInCurriculumDetailUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await FileInCurriculumDetailService.Update(model, GetCurrentUser());
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
        [Route("file/{fileId}")]
        public async Task<HttpResponseMessage> DeleteFile(int fileId)
        {
            try
            {
                await FileInCurriculumDetailService.Delete(fileId);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("file/{curriculumDetailId}")]
        public async Task<HttpResponseMessage> GetFile(int curriculumDetailId)
        {
            var data = await FileInCurriculumDetailService.GetByCurriculumDetail(curriculumDetailId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }

        [HttpPut]
        [Route("CurriculumDetailIndex")]
        public async Task<HttpResponseMessage> UpdateCurriculumDetailIndex(List<CurriculumDetailUpdate> request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CurriculumDetailService.UpdateCurriculumDetailIndex(request, GetCurrentUser());
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
        [Route("FileCurriculumDetailIndex")]
        public async Task<HttpResponseMessage> FileUpdateCurriculumDetailIndex(List<FileInCurriculumDetailUpdate> request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await FileInCurriculumDetailService.FileUpdateCurriculumDetailIndex(request, GetCurrentUser());
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
    }
}
