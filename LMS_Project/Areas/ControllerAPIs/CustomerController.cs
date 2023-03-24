using ExcelDataReader;
using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static LMS_Project.Models.lmsEnum;
using static LMS_Project.Services.CustomerService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [RoutePrefix("api/Customer")]
    [ClaimsAuthorize]
    public class CustomerController : BaseController
    {
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await CustomerService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("check-exist")]
        public async Task<HttpResponseMessage> CheckExist([FromUri] CheckExistModel itemModel)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
            }
            var data = await CustomerService.CheckExist(itemModel);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("send-mail")]
        public async Task<HttpResponseMessage> SendMail(SendMailModel itemModel)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
            }
            await CustomerService.SendMail(itemModel);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !"});
        }
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(CustomerCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CustomerService.Insert(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> Update(CustomerUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CustomerService.Update(model, GetCurrentUser());
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
                await CustomerService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        /// <summary>
        /// Nhân viên thuộc trung nào chỉ thấy khách hàng ở trung tâm đó, Tư vấn viên chỉ thấy khách hàng của chính mình
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll([FromUri] CustomerSearch baseSearch)
        {
            var data = await CustomerService.GetAll(baseSearch,GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpPost]
        [Route("ImportCustomer")]
        public async Task<HttpResponseMessage> ImportCustomer()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                HttpPostedFile Inputfile = null;
                Stream FileStream = null;
                using (var db = new lmsDbContext())
                {
                    var model = new List<CustomerCreate>();
                    if (httpRequest.Files.Count > 0)
                    {
                        Inputfile = httpRequest.Files.Get("File");
                        FileStream = Inputfile.InputStream;
                        if (Inputfile != null && FileStream != null)
                        {
                            if (Inputfile.FileName.EndsWith(".xls"))
                                reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                            else if (Inputfile.FileName.EndsWith(".xlsx"))
                                reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                            else
                                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Không đúng định dạng." });
                            dsexcelRecords = reader.AsDataSet();
                            reader.Close();
                            if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                            {
                                DataTable dtStudentRecords = dsexcelRecords.Tables[0];
                                for (int i = 3; i < dtStudentRecords.Rows.Count; i++)
                                {
                                    var item = new CustomerCreate
                                    {
                                        FullName = dtStudentRecords.Rows[i][0].ToString(),
                                        Email = dtStudentRecords.Rows[i][1].ToString(),
                                        Mobile = dtStudentRecords.Rows[i][2].ToString(),
                                    };
                                    if (GetCurrentUser().RoleId == ((int)RoleEnum.sale))
                                        item.SaleId = GetCurrentUser().UserInformationId;
                                    else item.SaleId = await GetSaleRadom();
                                    if (string.IsNullOrEmpty(item.FullName) || string.IsNullOrEmpty(item.Email) || string.IsNullOrEmpty(item.Mobile))
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Vui lòng điền đầy đủ tin khách hàng" });
                                    }
                                    model.Add(item);
                                }
                            }
                            else
                                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Không có dữ liệu." });
                        }
                        else
                            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "File lỗi." });
                    }
                    await CustomerService.ImportData(model, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thêm thành công" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
            }
        }
    }
}
