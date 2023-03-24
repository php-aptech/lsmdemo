using LMS_Project.Areas.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static LMS_Project.Services.DashboardService;

namespace LMS_Project.Areas.ControllerAPIs
{
    /// <summary>
    /// thống kê
    /// </summary>
    [ClaimsAuthorize]
    [RoutePrefix("api/Statistical")]
    public class StatisticalController : BaseController
    {
        /// <summary>
        /// mấy cái ô thống kê
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("overview")]
        public async Task<HttpResponseMessage> GetStatisticalOverview([FromUri] OverviewFilter search)
        {
            var data = await StatisticalService.GetStatisticalOverview(search, GetCurrentUser());
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// tuổi của học viên
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("student-age")]
        public async Task<HttpResponseMessage> GetStatisticalAge([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalAge(search);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// tỉ lệ đánh giá phản hồi
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("feedback-rating")]
        public async Task<HttpResponseMessage> GetStatisticalFeedbackRating([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalFeedbackRating(search);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// lớp mới mỗi tháng
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("new-class")]
        public async Task<HttpResponseMessage> GetStatisticalNewClass([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalNewClass(search);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// khách mới mỗi tháng
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("new-customer")]
        public async Task<HttpResponseMessage> GetStatisticalNewCustomer([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalNewCustomer(search);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// doanh thu
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("revenue")]
        public async Task<HttpResponseMessage> GetStatisticalPayment([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalPayment(search);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// top 5 nhu cầu học
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("top-learning-need")]
        public async Task<HttpResponseMessage> GetStatisticalTopLearningNeed([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalTopLearningNeed(search);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// top 5 mục đích học
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("top-purpose")]
        public async Task<HttpResponseMessage> GetStatisticalTopPurpose([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalTopPurpose(search);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// top 5 nguồn khách hàng
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("top-source")]
        public async Task<HttpResponseMessage> GetStatisticalTopSource([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalTopSource(search);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// top 5 công việc của học viên
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("top-job")]
        public async Task<HttpResponseMessage> GetStatisticalTopJob([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalTopJob(search);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// mấy cái block thống kê của giáo viên
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
       
        /// <summary>
        /// số buổi dạy của giáo viên mỗi tháng
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("total-schedule-teacher")]
        public async Task<HttpResponseMessage> GetStatisticalTotalScheduleTeacher([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalTotalScheduleTeacher(search, GetCurrentUser());
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// số buổi dạy của học viên mỗi tháng
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("total-schedule-student")]
        public async Task<HttpResponseMessage> GetStatisticalTotalScheduleStudent([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalTotalScheduleStudent(search, GetCurrentUser());
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// đánh giá của học viên
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("rate-teacher")]
        public async Task<HttpResponseMessage> GetStatisticalRateTeacher([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalRateTeacher(search, GetCurrentUser());
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// khách mới mỗi tháng theo từng sales
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("new-customerofsales")]
        public async Task<HttpResponseMessage> GetStatisticalNewCustomerOfSales([FromUri] ChartSearch search)
        {
            var data = await StatisticalService.GetStatisticalNewCustomerOfSalesId(search,GetCurrentUser());
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        /// <summary>
        /// Thống kê tất cả khách hàng hẹn test theo năm theo sales
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/StatisticialTestAppointment")]
        public async Task<HttpResponseMessage> StatisticialTestAppointment([FromUri] StatisticialCustomerInYearSearch baseSearch)
        {
            try
            {
                var data = await StatisticalService.StatisticialTestAppointment(baseSearch,GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
    }
}