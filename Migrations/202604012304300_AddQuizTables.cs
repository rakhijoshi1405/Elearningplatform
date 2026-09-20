namespace Elearningplatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuizTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Question",
                c => new
                    {
                        QuestionId = c.Int(nullable: false, identity: true),
                        QuizId = c.Int(nullable: false),
                        QuestionText = c.String(),
                        OptionA = c.String(),
                        OptionB = c.String(),
                        OptionC = c.String(),
                        OptionD = c.String(),
                        CorrectAnswer = c.String(),
                    })
                .PrimaryKey(t => t.QuestionId);
            
            CreateTable(
                "dbo.Quiz",
                c => new
                    {
                        QuizId = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.QuizId);
            
            CreateTable(
                "dbo.Result",
                c => new
                    {
                        ResultId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        QuizId = c.Int(nullable: false),
                        Score = c.Int(nullable: false),
                        AttemptDate = c.DateTime(nullable: false),
                        IsPassed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ResultId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Result");
            DropTable("dbo.Quiz");
            DropTable("dbo.Question");
        }
    }
}
