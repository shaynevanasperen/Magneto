using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DemoApplication.DomainModel.MultipleDatabases;
using Quarks.EntityFramework.Conventions.StoreModel;

namespace DemoApplication.EntityFramework.DbContextScope
{
	public class UsersContext : System.Data.Entity.DbContext
	{
		public UsersContext() : base("Users") { }

		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.AddBefore<ForeignKeyIndexConvention>(new RemoveUnderscoreFromDefaultForeignKeyNames());
		}
	}
}
