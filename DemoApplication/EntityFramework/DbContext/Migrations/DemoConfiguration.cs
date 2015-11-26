using System.Data.Entity.Migrations;

namespace DemoApplication.EntityFramework.DbContext.Migrations
{
	internal sealed class DemoConfiguration : DbMigrationsConfiguration<DemoContext>
	{
		public DemoConfiguration()
		{
			MigrationsDirectory = @"EntityFramework\DbContext\Migrations";
		}
	}
}
