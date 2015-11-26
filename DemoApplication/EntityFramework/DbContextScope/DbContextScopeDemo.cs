using System;
using System.Data.Entity;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.MultipleDatabases;
using Mehdime.Entity;

namespace DemoApplication.EntityFramework.DbContextScope
{
	static class DbContextScopeDemo
	{
		public static void Execute()
		{
			Database.SetInitializer(new DropCreateDatabaseAlways<TodoItemsContext>());
			using (var demoContext = new TodoItemsContext())
				demoContext.Database.Initialize(true);

			Database.SetInitializer(new DropCreateDatabaseAlways<UsersContext>());
			using (var demoContext = new UsersContext())
				demoContext.Database.Initialize(true);

			Console.WriteLine("Creating an {0}", typeof(IData).Name);
			var data = new Data.Operations.Data(new DataQueryCache(new MemoryCacheDefaultCacheStore()));
			Console.WriteLine("Creating an {0}", typeof(IUserAlertService).Name);
			var userAlertService = new UserAlertService();
			Console.WriteLine("Creating a {0}", typeof(DbContextScopeScenarios).FullName);
			using (var scenarios = new DbContextScopeScenarios(new DbContextScopeFactory(), new AmbientDbContextLocator(), data, userAlertService))
			{
				Console.WriteLine("Executing various scenarios by using different service types to demonstrate the different usage patterns");

				var users = scenarios.ExecuteWithTodoItemsService2(x => x.GetAllUsersAsync());
				Console.WriteLine("Found {0} users", users.Length);

				var todoItems = scenarios.ExecuteWithTodoItemsService3(x => x.GetAllTodoItemsAsync(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				todoItems = scenarios.ExecuteWithTodoItemsService1(x => x.GetTodoItemsDueThisWeekAsync(users[1].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				todoItems = scenarios.ExecuteWithTodoItemsService2(x => x.GetTodoItemsDueThisWeekAsync(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				todoItems = scenarios.ExecuteWithTodoItemsService3(x => x.GetTodoItemsDueThisMonthAsync(users[1].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				// Two calls to demonstrate caching
				todoItems = scenarios.ExecuteWithTodoItemsService1(x => x.GetTodoItemsCompletedLastWeekAsync(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);
				todoItems = scenarios.ExecuteWithTodoItemsService2(x => x.GetTodoItemsCompletedLastWeekAsync(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				// Two calls to demonstrate caching with transformation
				todoItems = scenarios.ExecuteWithTodoItemsService3(x => x.GetUpcomingTodoItemsAsync(users[1].Id, Priority.Normal));
				Console.WriteLine("Found {0} todo items", todoItems.Length);
				todoItems = scenarios.ExecuteWithTodoItemsService1(x => x.GetUpcomingTodoItemsAsync(users[1].Id, Priority.Urgent));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				var alertsSent = scenarios.ExecuteWithTodoItemsService2(x => x.SendAlertsForTodoItemsDueTomorrowAsync(users[1].Id));
				Console.WriteLine("Sent {0} alerts", alertsSent);

				var success = scenarios.ExecuteWithTodoItemsService3(x => x.CompleteTodoItemAsync(todoItems[0].Id));
				Console.WriteLine("TodoItem {0} {1} completed", todoItems[0].Id, success ? "was" : "wasn't");
			}
		}
	}
}
