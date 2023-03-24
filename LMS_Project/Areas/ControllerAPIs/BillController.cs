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
using static LMS_Project.Services.BillService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [RoutePrefix("api/Bill")]
    [ClaimsAuthorize]
    public class BillController : BaseController
    {
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await BillService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("class-available")]
        public async Task<HttpResponseMessage> GetClassAvailable([FromUri] GetClassAvailableSearch baseSearch)
        {
            var data = await BillService.GetClassAvailable(baseSearch);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(BillCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await BillService.Insert(model, GetCurrentUser());
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
        [Route("payment")]
        public async Task<HttpResponseMessage> Payment(PaymentCreate itemModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await BillService.Payment(itemModel, GetCurrentUser());
                    string successMessage = "Thành công";
                    if (GetCurrentUser().RoleId == ((int)RoleEnum.admin) || GetCurrentUser().RoleId == ((int)RoleEnum.accountant))
                        successMessage = "Đã gửi yêu cầu duyệt thanh toán, vui lòng đợi duyệt";
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = successMessage, data });
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
        public async Task<HttpResponseMessage> GetAll([FromUri] BillSearch baseSearch)
        {
            var data = await BillService.GetAll(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !",
                totalRow = data.TotalRow,
                data = data.Data,
                sumDebt = data.SumDebt,
                sumPaid = data.SumPaid,
                sumtotalPrice = data.SumtotalPrice
            });
        }
        [HttpGet]
        [Route("GetDiscountHistory")]
        public async Task<HttpResponseMessage> GetDiscountHistory([FromUri] BillSearch baseSearch)
        {
            var data = await BillService.GetDiscountHistory(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        [HttpGet]
        [Route("detail/{billId}")]
        public async Task<HttpResponseMessage> GetDetail(int billId)
        {
            var data = await BillService.GetDetail(billId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }

        //[HttpGet]
        //[Route("NotificationPayment")]
        //public async Task<HttpResponseMessage> NotificationPayment( )
        //{
        //    await BillService.PaymentNotification();
        //    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
        //}
    }
}
