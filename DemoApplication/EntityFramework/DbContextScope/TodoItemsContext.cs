using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DemoApplication.DomainModel.MultipleDatabases;
using Quarks.EntityFramework.Conventions.StoreModel;

namespace DemoApplication.EntityFramework.DbContextScope
{
	public class TodoItemsContext : System.Data.Entity.DbContext
	{
		public TodoItemsContext() : base("TodoItems") { }

		public DbSet<TodoItem> TodoItems { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.AddBefore<ForeignKeyIndexConvention>(new RemoveUnderscoreFromDefaultForeignKeyNames());
		}
	}
}
