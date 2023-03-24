using LMS_Project.Areas.Models;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static LMS_Project.Models.lmsEnum;
using static LMS_Project.Services.DashboardService;

namespace LMS_Project.Services
{
    public class Time
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int LastMonth { get; set; }
        public int LastYear { get; set; }
        public int Day { get; set; }
    }
    public class StatisticalService
    {
        public static Time GetTime()
        {
            DateTime timeNow = DateTime.Now;
            Time time = new Time();
            time.Month = timeNow.Month;
            time.Year = timeNow.Year;
            time.LastMonth = time.Month - 1 == 0 ? 12 : time.Month - 1;
            time.Year = time.LastMonth == 12 ? time.Year - 1 : time.Year;
            time.Day = timeNow.Day;
            return time;
        }
        public static async Task<List<ListOverviewModel>> GetStatisticalOverview(OverviewFilter search, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null)
                {
                    search = new OverviewFilter();
                }
                int? Role = 0;
                if (user.RoleId == (int)RoleEnum.parents)
                {
                    search.UserId = search.UserId;
                    Role = (int)RoleEnum.student;
                }
                else
                {
                    search.UserId = user.UserInformationId;
                    Role = user.RoleId;
                }
                //Time time = GetTime();

                string sql = $"GetGetStatistical_Overview " +
                        //$"@Month = N'{time.Month}'," +
                        //$"@Year = N'{search.Year}'," +
                        //$"@LastMonth = N'{time.LastMonth}'," +
                        //$"@LastYear = N'{time.LastYear}'," +
                        //$"@Day = N'{time.Day}'," +
                        $"@RoleId = N'{Role}'," +
                        $"@UserId = N'{search.UserId}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                var result = await db.Database.SqlQuery<OverviewModel>(sql).ToListAsync();

                string increase = "Tăng ";
                string decrease = "Giảm ";
                string restString = " so với tháng trước";
                double value = 0;
                int tang = 1;
                int giam = 2;
                foreach (var item in result)
                {
                    if (item.PreValue.HasValue)
                    {
                        if (item.Value - item.PreValue == 0)
                        {
                            item.SubValue = null;
                            item.Type = 0;
                        }
                        //nếu tháng này = 0 và tháng trước < 0 thì tháng này tăng 100
                        else if (item.Value == 0 && item.PreValue.Value < 0)
                        {
                            item.SubValue = increase + "100%" + restString;
                            item.Type = tang;
                        }
                        //nếu tháng này = 0 và tháng trước > 0 thì tháng này giảm 100
                        else if (item.Value == 0 && item.PreValue.Value > 0)
                        {
                            item.SubValue = decrease + "100%" + restString;
                            item.Type = giam;
                        }
                        //nếu tháng này > 0 và tháng trước = 0 thì tháng này tăng 100
                        else if (item.Value > 0 && item.PreValue.Value == 0)
                        {
                            item.SubValue = increase + "100%" + restString;
                            item.Type = tang;
                        }
                        //nếu tháng này < 0 và tháng trước = 0 thì tháng này giảm 100
                        else if (item.Value < 0 && item.PreValue.Value == 0)
                        {
                            item.SubValue = decrease + "100%" + restString;
                            item.Type = giam;
                        }
                        else
                        {
                            value = Math.Round(Math.Abs((item.Value / item.PreValue.Value * 100) - 100), 2);
                            if (value > 0)
                            {
                                item.SubValue = increase + value + "%" + restString;
                                item.Type = tang;
                            }
                            else
                            {
                                item.SubValue = decrease + value + "%" + restString;
                                item.Type = giam;
                            }
                        }
                    }

                }

                var temp = result.GroupBy(x => x.Groups).Select(x => new ListOverviewModel
                {
                    Id = x.First().Groups,
                    Title = ListStatisticalOverviewGroups().SingleOrDefault(y => y.Key == x.First().Groups).Value,
                    OverviewModel = x.ToList()
                }).ToList();
                return temp.Where(x => x.OverviewModel.Any()).ToList();
            }

        }

        public static async Task<List<StatisticalYear>> GetStatisticalAge(ChartSearch search)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_Age " +
                        $"@Year = N'{search.Year}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalFeedbackRating(ChartSearch search)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_FeedbackRating " +
                        $"@Year = N'{search.Year}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalNewClass(ChartSearch search)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_NewClass " +
                        $"@Year = N'{search.Year}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalNewCustomer(ChartSearch search)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_NewCustomer " +
                        $"@Year = N'{search.Year}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalPayment(ChartSearch search)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_Payment " +
                        $"@Year = N'{search.Year}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalTopLearningNeed(ChartSearch search)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_TopLearningNeed " +
                        $"@Year = N'{search.Year}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalTopPurpose(ChartSearch search)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_TopPurpose " +
                        $"@Year = N'{search.Year}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalTopSource(ChartSearch search)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_TopSource " +
                        $"@Year = N'{search.Year}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalTopJob(ChartSearch search)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_TopJob " +
                        $"@Year = N'{search.Year}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalTotalScheduleTeacher(ChartSearch search, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_TotalScheduleTeacher " +
                        $"@Year = N'{search.Year}'," +
                        $"@UserId = N'{user.UserInformationId}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalTotalScheduleStudent(ChartSearch search, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_TotalScheduleStudent " +
                        $"@Year = N'{search.Year}'," +
                        $"@UserId = N'{user.UserInformationId}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalRateTeacher(ChartSearch search, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_RateTeacher " +
                        $"@Year = N'{search.Year}'," +
                        $"@UserId = N'{user.UserInformationId}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        public static async Task<List<StatisticalYear>> GetStatisticalNewCustomerOfSalesId(ChartSearch search, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                int? userId = null;
                if (user.RoleId == (int)RoleEnum.sale)
                {
                    userId = user.UserInformationId;
                }
                Time time = GetTime();
                if (search == null)
                {
                    search = new ChartSearch
                    {
                        Year = time.Year,
                    };
                }
                else
                {
                    search.Year = search.Year ?? time.Year;
                }
                string sql = $"GetStatistical_Chart_NewCustomerOfSales " +
                        $"@Year = N'{search.Year}'," +
                        $"@UserId = N'{user.UserInformationId}'," +
                        $"@BranchIds = N'{(search.BranchIds) ?? ""}'";
                return await db.Database.SqlQuery<StatisticalYear>(sql).ToListAsync();
            }

        }
        /// <summary>
        /// Thống kê khách hàng hẹn test theo năm theo sales
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public static async Task<List<StatisticialTestAppointmentModel>> StatisticialTestAppointment(StatisticialCustomerInYearSearch baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                int? userId = null;
                if (user.RoleId == (int)RoleEnum.sale)
                    userId = user.UserInformationId;
                if (baseSearch == null) baseSearch = new StatisticialCustomerInYearSearch();
                string sql = "GetStatistical_Chart_TestAppointment @BranchIds = {0},@Year={1},@UserId={2}";
                var data = await db.Database.SqlQuery<StatisticialTestAppointmentModel>(sql, baseSearch.BranchIds, baseSearch.Year, userId).ToListAsync();
                return data;
            }

        }
    }
}