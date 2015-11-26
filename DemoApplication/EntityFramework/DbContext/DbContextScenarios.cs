using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel.SingleDatabase;

namespace DemoApplication.EntityFramework.DbContext
{
	class DbContextScenarios : IDisposable
	{
		readonly IData _data;
		readonly IUserAlertService _userAlertService;

		public DbContextScenarios(IData data, IUserAlertService userAlertService)
		{
			_data = data;
			_userAlertService = userAlertService;
			createSampleData();
		}

		void createSampleData()
		{
			Console.WriteLine("Creating sample data for {0}", GetType().Name);
			var sampleTodoItems = SampleDataFactory.CreateTodoItems();
			using (var demoContext = new DemoContext())
			{
				foreach (var sampleUser in sampleTodoItems.Select(x => x.User).Distinct())
					demoContext.Users.Add(sampleUser);
				foreach (var sampleTodoItem in sampleTodoItems)
					demoContext.TodoItems.Add(sampleTodoItem);
				demoContext.SaveChanges();
			}
		}

		void deleteAllData()
		{
			Console.WriteLine("Deleting all data for {0}", GetType().Name);
			using (var demoContext = new DemoContext())
			{
				demoContext.Database.ExecuteSqlCommand("DELETE FROM dbo.[TodoItem]");
				demoContext.Database.ExecuteSqlCommand("DELETE FROM dbo.[User]");
				demoContext.SaveChanges();
			}
		}

		public void Dispose()
		{
			deleteAllData();
		}

		public T ExecuteWithTodoItemsService1<T>(Expression<Func<TodoItemsService1, Task<T>>> scenarioExpression)
		{
			using (var demoContext = new DemoContext())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService1).FullName);
				var todoItemsService = new TodoItemsService1(demoContext, _data, _userAlertService);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService).Result;
			}
		}

		public T ExecuteWithTodoItemsService2<T>(Expression<Func<TodoItemsService2, Task<T>>> scenarioExpression)
		{
			using (var demoContext = new DemoContext())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService2).FullName);
				var todoItemsService = new TodoItemsService2(demoContext);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService).Result;
			}
		}

		public T ExecuteWithTodoItemsService3<T>(Expression<Func<TodoItemsService3, Task<T>>> scenarioExpression)
		{
			using (var demoContext = new DemoContext())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService3).FullName);
				var todoItemsService = new TodoItemsService3(new Data<DemoContext>(_data, demoContext));
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService).Result;
			}
		}
	}
}
