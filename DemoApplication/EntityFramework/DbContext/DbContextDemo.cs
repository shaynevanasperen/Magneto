using System;
using System.Data.Entity;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;

namespace DemoApplication.EntityFramework.DbContext
{
	static class DbContextDemo
	{
		public static void Execute()
		{
			Database.SetInitializer(new DropCreateDatabaseAlways<DemoContext>());
			using (var demoContext = new DemoContext())
				demoContext.Database.Initialize(true);

			Console.WriteLine("Creating an {0}", typeof(IData).Name);
			var data = new Data.Operations.Data(new DataQueryCache(new MemoryCacheDefaultCacheStore()));
			Console.WriteLine("Creating an {0}", typeof(IUserAlertService).Name);
			var userAlertService = new UserAlertService();
			Console.WriteLine("Creating a {0}", typeof(DbContextScenarios).FullName);
			using (var scenarios = new DbContextScenarios(data, userAlertService))
			{
				Console.WriteLine("Executing various scenarios by using different service types to demonstrate the different usage patterns");

				var users = scenarios.ExecuteWithTodoItemsService1(x => x.GetAllUsersAsync());
				Console.WriteLine("Found {0} users", users.Length);

				var todoItems = scenarios.ExecuteWithTodoItemsService2(x => x.GetAllTodoItemsAsync(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				todoItems = scenarios.ExecuteWithTodoItemsService3(x => x.GetTodoItemsDueThisWeekAsync(users[1].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				todoItems = scenarios.ExecuteWithTodoItemsService1(x => x.GetTodoItemsDueThisWeekAsync(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				todoItems = scenarios.ExecuteWithTodoItemsService2(x => x.GetTodoItemsDueThisMonthAsync(users[1].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				// Two calls to demonstrate caching
				todoItems = scenarios.ExecuteWithTodoItemsService3(x => x.GetTodoItemsCompletedLastWeekAsync(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);
				todoItems = scenarios.ExecuteWithTodoItemsService1(x => x.GetTodoItemsCompletedLastWeekAsync(users[0].Id));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				// Two calls to demonstrate caching with transformation
				todoItems = scenarios.ExecuteWithTodoItemsService2(x => x.GetUpcomingTodoItemsAsync(users[1].Id, Priority.Normal));
				Console.WriteLine("Found {0} todo items", todoItems.Length);
				todoItems = scenarios.ExecuteWithTodoItemsService3(x => x.GetUpcomingTodoItemsAsync(users[1].Id, Priority.Urgent));
				Console.WriteLine("Found {0} todo items", todoItems.Length);

				var alertsSent = scenarios.ExecuteWithTodoItemsService1(x => x.SendAlertsForTodoItemsDueTomorrowAsync(users[1].Id));
				Console.WriteLine("Sent {0} alerts", alertsSent);

				var success = scenarios.ExecuteWithTodoItemsService2(x => x.CompleteTodoItemAsync(todoItems[0].Id));
				Console.WriteLine("TodoItem {0} {1} completed", todoItems[0].Id, success ? "was" : "wasn't");
			}
		}
	}
}
