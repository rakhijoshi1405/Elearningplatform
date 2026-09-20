using System;
using System.Data.Entity.Migrations;

namespace Elearningplatform.Migrations
{
    public partial class UpdatePaymentModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Payment",
                c => new
                {
                    PaymentId = c.Int(nullable: false, identity: true),
                    CourseId = c.Int(nullable: false),
                    StudentId = c.Int(nullable: false),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Status = c.String(),
                    PaymentDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.PaymentId);
        }

        public override void Down()
        {
            DropTable("dbo.Payment");
        }
    }
}