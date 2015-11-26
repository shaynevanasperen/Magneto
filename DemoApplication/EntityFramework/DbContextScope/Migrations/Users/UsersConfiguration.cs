using System.Data.Entity.Migrations;

namespace DemoApplication.EntityFramework.DbContextScope.Migrations.Users
{
	internal sealed class UsersConfiguration : DbMigrationsConfiguration<UsersContext>
	{
		public UsersConfiguration()
		{
			MigrationsDirectory = @"EntityFramework\DbContextScope\Migrations\Users";
		}
	}
}
