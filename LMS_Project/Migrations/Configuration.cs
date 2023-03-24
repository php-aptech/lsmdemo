namespace LMS_Project.Migrations
{
    using LMS_Project.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using static LMS_Project.Models.lmsEnum;

    internal sealed class Configuration : DbMigrationsConfiguration<LMS_Project.Models.lmsDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LMS_Project.Models.lmsDbContext context)
        {
            string email = "monamedia@gmail.com";
            string fullName = "Mona Media";
            string mobile = "1900636648";
            string pass = "nhYOiIa2GG0=";
            string byUser = "Hệ thống";
            //context.tbl_UserInformation.Add(new tbl_UserInformation
            //{
            //    FullName = fullName,
            //    UserCode = "AD00001",
            //    UserName = "Admin",
            //    Password = pass,
            //    DOB = DateTime.Now,
            //    Gender = 2,
            //    Mobile = mobile,
            //    Extension = "",
            //    Email = email,
            //    LinkFaceBook = "",
            //    AreaId = 0,
            //    DistrictId = 0,
            //    WardId = 0,
            //    Address = "",
            //    StatusId = ((int)AccountStatus.active),
            //    RoleId = ((int)RoleEnum.admin),
            //    RoleName = "Admin",
            //    Avatar = "",
            //    OneSignal_DeviceId = "",
            //    Enable = true,
            //    CreatedOn = DateTime.Now,
            //    CreatedBy = byUser,
            //    ModifiedOn = DateTime.Now,
            //    ModifiedBy = byUser
            //});
            //context.tbl_Config.Add(new tbl_Config
            //{
            //    Code = "Register",
            //    Name = "Đăng ký tài khoản",
            //    Value = "Allow"
            //});
            //context.tbl_CustomerStatus.Add(new tbl_CustomerStatus
            //{
            //    CreatedBy = byUser,
            //    CreatedOn = DateTime.Now,
            //    Enable = true,
            //    ModifiedBy = byUser,
            //    ModifiedOn = DateTime.Now,
            //    Name = "Cần tư vấn",
            //    Type = 1
            //});
            //context.tbl_CustomerStatus.Add(new tbl_CustomerStatus
            //{
            //    CreatedBy = byUser,
            //    CreatedOn = DateTime.Now,
            //    Enable = true,
            //    ModifiedBy = byUser,
            //    ModifiedOn = DateTime.Now,
            //    Name = "Đã chuyển học viên",
            //    Type = 3
            //});
            //context.tbl_PaymentMethod.AddRange(new List<tbl_PaymentMethod>()
            //{
            //    new tbl_PaymentMethod{
            //        Active = true,
            //        Code = "cash",
            //        CreatedBy = byUser,
            //        CreatedOn = DateTime.Now,
            //        Description = "",
            //        Enable = true,
            //        ModifiedBy = byUser,
            //        ModifiedOn = DateTime.Now,
            //        Name = "Tiền mặt",
            //        Thumbnail = "",
            //    },
            //    new tbl_PaymentMethod{
            //        Active = true,
            //        Code = "transfer",
            //        CreatedBy = byUser,
            //        CreatedOn = DateTime.Now,
            //        Description = "Thông tin thanh toán chuyển khoản",
            //        Enable = true,
            //        ModifiedBy = byUser,
            //        ModifiedOn = DateTime.Now,
            //        Name = "Chuyển khoản",
            //        Thumbnail = "",
            //    },
            //});
        }
    }
}
