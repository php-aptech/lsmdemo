using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LMS_Project.Services
{
    public class PushAuto
    {
        public static void PushOneMinute()
        {
            Task.Run(() => {
                NotificationInVideoService.SeenNotification();
                ScheduleService.AutoCloseZoom();
                //BillService.PaymentNotification();
                AutoNotiService.AutoNotiClassComing();
            });
        }
        public static void PushOneDay()
        {
            Task.Run(() => {
                ///Tự động khoá tài khoản
                //UserInformation.AutoInActive();
                ClassReserveService.AutoUpdateStatus();
                ClassService.AutoUpdateStatus();
                ///Cập nhật khuyến mãi
                DiscountService.Expired();
                BillService.PaymentNotification();
            });
        }
    }
}