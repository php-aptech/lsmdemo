namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_user_rf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbl_UserInformation", "RefreshToken", c => c.String());
            AddColumn("dbo.tbl_UserInformation", "RefreshTokenExpires", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbl_UserInformation", "RefreshTokenExpires");
            DropColumn("dbo.tbl_UserInformation", "RefreshToken");
        }
    }
}
