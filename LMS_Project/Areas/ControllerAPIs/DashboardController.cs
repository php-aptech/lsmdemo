using LMS_Project.Areas.Models;
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
using static LMS_Project.Services.DashboardService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class DashboardController : BaseController
    {
        [HttpGet]
        [Route("api/Dashboard/Overview")]
        public async Task<HttpResponseMessage> GetAll()
        {
            try
            {
                var data = await DashboardService.OverviewModel();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
            
        }
        [HttpGet]
        [Route("api/Dashboard/StatisticGetInMonth")]
        public async Task<HttpResponseMessage> GetAllInMonth()
        {
            try
            {
                var data = await DashboardService.OverviewModelInMonth();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }
        [HttpGet]
        [Route("api/Dashboard/StatisticAgeStudent")]
        public async Task<HttpResponseMessage> GetAllAgeStudent()
        {
            try
            {
                var data = await DashboardService.StatisticForAge();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }
        [HttpGet]
        [Route("api/Dashboard/StatisticTopCourse")]
        public async Task<HttpResponseMessage> StatisticTopCourse()
        {
            try
            {
                var data = await DashboardService.StatisticTopCourse();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }
        [HttpGet]
        [Route("api/Dashboard/OverviewTeacher")]
        public async Task<HttpResponseMessage> OverviewTeacher()
        {
            try
            {
                var data = await DashboardService.OverviewModelForTeacher(GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }

        [HttpGet]
        [Route("api/Dashboard/OverviewStudent")]
        public async Task<HttpResponseMessage> OverviewStudent()
        {
            try
            {
                var data = await DashboardService.StatisticCourseStudent(GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/Dashboard/Student/LearningDetails")]
        public async Task<HttpResponseMessage> LearningDetails()
        {
            try
            {
                var data = await DashboardService.LearningDetails(GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        /// <summary>
        /// Thống kê học tập của người dùng
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/OverviewLearning")]
        public async Task<HttpResponseMessage> OverviewLearning([FromUri] OverviewSearch search)
        {
            try
            {
                var data = await DashboardService.OverviewLearning(search);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        /// <summary>
        /// Thống kê khoá học của hệ thống
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/OverviewVideoCourse")]
        public async Task<HttpResponseMessage> OverviewVideoCourse([FromUri] OverviewSearch search)
        {
            try
            {
                var data = await DashboardService.OverviewVideoCourse(search);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        /// <summary>
        /// Thống kê bài tập
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/OverviewExam")]
        public async Task<HttpResponseMessage> OverviewExam([FromUri] OverviewSearch search)
        {
            try
            {
                var data = await DashboardService.OverviewExam(search);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        /// <summary>
        /// Thống kê khách hàng và học viên theo tháng dành cho tư vấn viên
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/StatisticialCustomerInMonth")]
        public async Task<HttpResponseMessage> StatisticialCustomerInMonth([FromUri] StatisticialCustomerSearch baseSearch)
        {
            try
            {
                var data = await DashboardService.StatisticialCustomerInMonth(baseSearch);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }

        /// <summary>
        /// Thống kê tất cả khách hàng và học viên dành cho tư vấn viên
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/StatisticialAllCustomer")]
        public async Task<HttpResponseMessage> StatisticialAllCustomer([FromUri] StatisticialCustomerSearch baseSearch)
        {
            try
            {
                var data = await DashboardService.StatisticialAllCustomer(baseSearch);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }

        
    }



}
