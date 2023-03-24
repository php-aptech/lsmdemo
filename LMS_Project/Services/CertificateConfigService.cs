using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class CertificateConfigService
    {
        public static async Task<List<string>> GetGuide()
        {
            return new List<string> {
                "{TenHocVien} : Tên học viên",
                "{MaHocVien} : Mã học viên",
                "{NgayHoanThanh} : Ngày hoàn thành",
                "{Ngay} : Ngày hiện tại",
                "{Thang} : Tháng hiện tại",
                "{Nam} : Năm hiện tại",
            };
        }
        public static string ReplaceContent(string content,tbl_UserInformation user)
        {
            content = content.Replace("{TenHocVien}", user.FullName);
            content = content.Replace("{MaHocVien}", user.UserCode);
            content = content.Replace("{NgayHoanThanh}", DateTime.Now.ToString("dd/MM/yyyy"));
            content = content.Replace("{Ngay}", DateTime.Now.Day.ToString());
            content = content.Replace("{Thang}", DateTime.Now.Month.ToString());
            content = content.Replace("{Nam}", DateTime.Now.Year.ToString());
            return content;
        }
        public static async Task Config(CertificateConfigModel model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    if (string.IsNullOrEmpty(model.Content))
                        throw new Exception("Vui lòng nhập nội dung chỉnh sửa");
                    var certificateConfig = await db.tbl_CertificateConfig.FirstOrDefaultAsync();
                    if (certificateConfig == null)
                    {
                        db.tbl_CertificateConfig.Add(new tbl_CertificateConfig
                        {
                            Content = model.Content,
                            CreatedBy = user.FullName,
                            ModifiedBy = user.FullName,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            Enable = true
                        });
                    }
                    else
                    {
                        certificateConfig.ModifiedOn = DateTime.Now;
                        certificateConfig.ModifiedBy = user.FullName;
                        certificateConfig.Content = model.Content;
                    }
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<CertificateConfigModel> GetData()
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var data = await db.tbl_CertificateConfig.FirstOrDefaultAsync();
                    if (data == null)
                        return new CertificateConfigModel { Content = ""};
                    return new CertificateConfigModel { Content = data.Content };
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }
}