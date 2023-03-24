namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_file : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbl_CurriculumDetailInClass", "IsHide", c => c.Boolean(nullable: false));
            AlterColumn("dbo.tbl_FileCurriculumInClass", "IsHide", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tbl_FileCurriculumInClass", "IsHide", c => c.Boolean());
            DropColumn("dbo.tbl_CurriculumDetailInClass", "IsHide");
        }
    }
}
