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
    [RoutePrefix("api/PaymentApprove")]
    [ClaimsAuthorize]
    public class PaymentApproveController : BaseController
    {
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await PaymentApproveService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        /// <summary>
        /// 1 - Chờ duyệt
        /// 2 - Đã duyệt
        /// 3 - Không duyệt
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/status/{status}")]
        public async Task<HttpResponseMessage> Approve(int id, int status)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await PaymentApproveService.Approve(id, status, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
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
                await PaymentApproveService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll([FromUri] PaymentApproveSearch baseSearch)
        {
            var data = await PaymentApproveService.GetAll(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", 
                totalRow = data.TotalRow, 
                data = data.Data,
                totalMoney = data.TotalMoney
            });
        }
    }
}
