namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecertification1233123 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbl_Certificate", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbl_Certificate", "Name");
        }
    }
}
