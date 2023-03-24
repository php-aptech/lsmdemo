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
    [RoutePrefix("api/PaymentSession")]
    [ClaimsAuthorize]
    public class PaymentSessionController : BaseController
    {
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await PaymentSessionService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(PaymentSessionCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await PaymentSessionService.Insert(model, GetCurrentUser());
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
        public async Task<HttpResponseMessage> Update(PaymentSessionUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await PaymentSessionService.Update(model, GetCurrentUser());
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
        //[HttpDelete]
        //[Route("{id}")]
        //public async Task<HttpResponseMessage> Delete(int id)
        //{
        //    try
        //    {
        //        await PaymentSessionService.Delete(id);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
        //    }
        //}
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll([FromUri] PaymentSessionSearch baseSearch)
        {
            var data = await PaymentSessionService.GetAll(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { 
                message = "Thành công !", 
                totalRevenue = data.TotalRevenue,
                totalIncome = data.TotalIncome,
                totalExpense = data.TotalExpense, 
                totalRow = data.TotalRow, 
                data = data.Data });
        }
    }
}
