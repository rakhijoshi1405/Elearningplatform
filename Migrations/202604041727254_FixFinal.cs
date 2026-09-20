namespace Elearningplatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixFinal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Certificate",
                c => new
                    {
                        CertificateId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        IssueDate = c.DateTime(nullable: false),
                        CertificateNumber = c.String(),
                    })
                .PrimaryKey(t => t.CertificateId);
            
            AlterColumn("dbo.VideoCourseContents", "Duration", c => c.Int());
            AlterColumn("dbo.VideoCourseContents", "FileSize", c => c.Long());
            AlterColumn("dbo.VideoCourseContents", "DisplayOrder", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.VideoCourseContents", "DisplayOrder", c => c.Int(nullable: false));
            AlterColumn("dbo.VideoCourseContents", "FileSize", c => c.Long(nullable: false));
            AlterColumn("dbo.VideoCourseContents", "Duration", c => c.Int(nullable: false));
            DropTable("dbo.Certificate");
        }
    }
}
