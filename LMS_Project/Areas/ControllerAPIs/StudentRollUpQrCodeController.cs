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
    [RoutePrefix("api/StudentRollUpQrCode")]
    [ClaimsAuthorize]
    public class StudentRollUpQrCodeController : BaseController
    {
        ///// <summary>
        ///// tạo mã Qr
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route()]
        //public async Task<HttpResponseMessage> Insert(StudentRollUpQrCodeCreate request)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var result = await StudentRollUpQrCodeService.Insert(request);
        //            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", result });
        //        }
        //        catch (Exception e)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
        //        }
        //    }
        //    var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
        //    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        //}
        /// <summary>
        /// xem mã Qr
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route()]
        public async Task<HttpResponseMessage> GetQrCodeByStudent(int studentId, int scheduleId)
        {
            try
            {
                var data = await StudentRollUpQrCodeService.GetQrCodeByStudent(studentId, scheduleId);
                if (data == null)
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
           
        }
        /// <summary>
        /// điểm danh bằng mã Qr
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("attendance-by-qr-code")]
        public async Task<HttpResponseMessage> AttendanceByQrCode(StudentRollUpQrCodeCreate request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await StudentRollUpQrCodeService.AttendanceByQrCode(request);
                    if (data == null)
                        return Request.CreateResponse(HttpStatusCode.NoContent);
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
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
