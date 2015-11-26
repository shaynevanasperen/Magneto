using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DemoApplication.DomainModel.SingleDatabase;
using Quarks.EntityFramework.Conventions.StoreModel;

namespace DemoApplication.EntityFramework.DbContext
{
	public class DemoContext : System.Data.Entity.DbContext
	{
		public DemoContext() : base("SingleDatabase") { }

		public DbSet<TodoItem> TodoItems { get; set; }
		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.AddBefore<ForeignKeyIndexConvention>(new RemoveUnderscoreFromDefaultForeignKeyNames());
		}
	}
}
