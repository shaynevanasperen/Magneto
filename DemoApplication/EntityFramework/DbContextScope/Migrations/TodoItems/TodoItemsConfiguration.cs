using System.Data.Entity.Migrations;

namespace DemoApplication.EntityFramework.DbContextScope.Migrations.TodoItems
{
	internal sealed class TodoItemsConfiguration : DbMigrationsConfiguration<TodoItemsContext>
	{
		public TodoItemsConfiguration()
		{
			MigrationsDirectory = @"EntityFramework\DbContextScope\Migrations\TodoItems";
		}
	}
}
