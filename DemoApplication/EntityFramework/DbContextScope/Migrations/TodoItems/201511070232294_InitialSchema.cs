namespace DemoApplication.EntityFramework.DbContextScope.Migrations.TodoItems
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TodoItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Details = c.String(),
                        Priority = c.Int(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        DateCompleted = c.DateTime(),
                        AlertSent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TodoItem");
        }
    }
}
