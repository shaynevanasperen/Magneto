using System;
using System.Reflection;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.MultipleDatabases;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Sessions;
using NHibernate.Sessions.Configuration;
using NHibernate.Tool.hbm2ddl;
using Quarks;

namespace DemoApplication.NHibernate.LazySessions
{
	static class LazySessionsDemo
	{
		public static void Execute()
		{
			Program.EnsureDatabaseExists("Data.Operations.DemoApplication.MultipleDatabases.TodoItems");
			new SchemaExport(getConfiguration(typeof(TodoItems).Name)).Execute(true, true, false);

			Program.EnsureDatabaseExists("Data.Operations.DemoApplication.MultipleDatabases.Users");
			new SchemaExport(getConfiguration(typeof(Users).Name)).Execute(true, true, false);

			Console.WriteLine("Creating an {0}", typeof(ILazySessions).Name);
			var lazySessions = ConfigureNHibernate
				.ForMultipleSessionFactories(getConfiguration)
				.WithLazyInitialization()
				.WithInitializedCallback(x => Console.WriteLine("{0} initialized", x.GetType().Name))
				.WithSessionOpenedCallback(x => Console.WriteLine("{0} opened", x.GetType().Name))
				.RegisterSessionFactory(typeof(TodoItems).Name)
				.RegisterSessionFactory(typeof(Users).Name)
				.CreateLazySessions();
			Console.WriteLine("Creating an {0}", typeof(IData).Name);
			var data = new Data.Operations.Data(new DataQueryCache(new MemoryCacheDefaultCacheStore()));
			Console.WriteLine("Creating an {0}", typeof(IUserAlertService).Name);
			var userAlertService = new UserAlertService();
			Console.WriteLine("Creating a {0}", typeof(LazySessionsScenarios).FullName);
			using (var scenarios = new LazySessionsScenarios(lazySessions, data, userAlertService))
			{
				Console.WriteLine("Executing various scenarios by using different service types to demonstrate the different usage patterns");

				var users = scenarios.ExecuteWithTodoItemsService3(x => x.GetAllUsers());
				Console.WriteLine("Found {0} users", users.Length);

				var todoItems = scenarios.ExecuteWithTodoItemsService1(x => x.GetAllTodoItems(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				todoItems = scenarios.ExecuteWithTodoItemsService2(x => x.GetTodoItemsDueThisWeek(users[1].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				todoItems = scenarios.ExecuteWithTodoItemsService3(x => x.GetTodoItemsDueThisWeek(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				todoItems = scenarios.ExecuteWithTodoItemsService1(x => x.GetTodoItemsDueThisMonth(users[1].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				// Two calls to demonstrate caching
				todoItems = scenarios.ExecuteWithTodoItemsService2(x => x.GetTodoItemsCompletedLastWeek(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);
				todoItems = scenarios.ExecuteWithTodoItemsService3(x => x.GetTodoItemsCompletedLastWeek(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				// Two calls to demonstrate caching with transformation
				todoItems = scenarios.ExecuteWithTodoItemsService1(x => x.GetUpcomingTodoItems(users[1].Id, Priority.Normal));
				Console.WriteLine("Found {0} todo items", todoItems.Length);
				todoItems = scenarios.ExecuteWithTodoItemsService2(x => x.GetUpcomingTodoItems(users[1].Id, Priority.Urgent));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				var alertsSent = scenarios.ExecuteWithTodoItemsService3(x => x.SendAlertsForTodoItemsDueTomorrowAsync(users[1].Id)).Result;
				Console.WriteLine("Sent {0} alerts", alertsSent);

				var success = scenarios.ExecuteWithTodoItemsService1(x => x.CompleteTodoItem(todoItems[0].Id));
				Console.WriteLine("TodoItem {0} {1} completed", todoItems[0].Id, success ? "was" : "wasn't");
			}
		}

		static Configuration getConfiguration(string connectionStringName)
		{
			return Fluently.Configure()
				.Database(MsSqlConfiguration.MsSql2012.ConnectionString(c => c.FromConnectionStringWithKey(connectionStringName)))
				.Mappings(m => m.AutoMappings.Add(AutoMap
					.Source(new AssemblyTypeSource(Assembly.GetExecutingAssembly()))
					.Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsForSessionFactory(connectionStringName))
					.IgnoreBase(typeof(IdentityFieldProvider<,>))
					.Conventions.AddAssembly(Assembly.GetExecutingAssembly())))
				.CurrentSessionContext<ThreadLocalSessionContext>()
				.BuildConfiguration();
		}
	}
}
