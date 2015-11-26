using System;
using System.Data.SqlClient;
using DemoApplication.EntityFramework.DbContext;
using DemoApplication.EntityFramework.DbContextScope;
using DemoApplication.NHibernate.LazySession;
using DemoApplication.NHibernate.LazySessions;
using DemoApplication.NHibernate.Session;

namespace DemoApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			SessionDemo.Execute();
			LazySessionDemo.Execute();
			LazySessionsDemo.Execute();
			DbContextDemo.Execute();
			DbContextScopeDemo.Execute();
		}

		internal static void EnsureDatabaseExists(string databaseName)
		{
			using (var connection = new SqlConnection("server=(localdb)\\MSSQLLocalDB"))
			{
				connection.Open();

				var sql = string.Format(@"IF (SELECT DB_ID('{0}')) IS NULL
	CREATE DATABASE [{0}] ON PRIMARY (NAME='{0}', FILENAME = '{1}\{0}.mdf') LOG ON (NAME='{0}_log', FILENAME = '{1}\{0}_log.ldf')",
					databaseName,
					Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
				);

				var command = new SqlCommand(sql, connection);
				command.ExecuteNonQuery();
			}
		}
	}
}
