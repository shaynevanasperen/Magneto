using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel.MultipleDatabases;
using Mehdime.Entity;

namespace DemoApplication.EntityFramework.DbContextScope
{
	class DbContextScopeScenarios : IDisposable
	{
		readonly IDbContextScopeFactory _dbContextScopeFactory;
		readonly IAmbientDbContextLocator _ambientDbContextLocator;
		readonly IData _data;
		readonly IUserAlertService _userAlertService;

		public DbContextScopeScenarios(
			IDbContextScopeFactory dbContextScopeFactory, IAmbientDbContextLocator ambientDbContextLocator, IData data, IUserAlertService userAlertService)
		{
			_dbContextScopeFactory = dbContextScopeFactory;
			_ambientDbContextLocator = ambientDbContextLocator;
			_data = data;
			_userAlertService = userAlertService;
			createSampleData();
		}

		void createSampleData()
		{
			Console.WriteLine("Creating sample data for {0}", GetType().Name);
			using (var dbContextScope = _dbContextScopeFactory.Create())
			{
				var sampleUsers = SampleDataFactory.CreateUsers();
				dbContextScope.DbContexts.Get<UsersContext>().Users.Add(sampleUsers.Item1);
				dbContextScope.DbContexts.Get<UsersContext>().Users.Add(sampleUsers.Item2);

				var sampleTodoItems = SampleDataFactory.CreateTodoItems(sampleUsers);
				foreach (var sampleTodoItem in sampleTodoItems)
					dbContextScope.DbContexts.Get<TodoItemsContext>().TodoItems.Add(sampleTodoItem);

				dbContextScope.SaveChanges();
			}
		}

		void deleteAllData()
		{
			Console.WriteLine("Deleting all data for {0}", GetType().Name);
			using (var dbContextScope = _dbContextScopeFactory.Create())
			{
				dbContextScope.DbContexts.Get<TodoItemsContext>().Database.ExecuteSqlCommand("DELETE FROM dbo.[TodoItem]");
				dbContextScope.DbContexts.Get<UsersContext>().Database.ExecuteSqlCommand("DELETE FROM dbo.[User]");
				dbContextScope.SaveChanges();
			}
		}

		public void Dispose()
		{
			deleteAllData();
		}

		public T ExecuteWithTodoItemsService1<T>(Expression<Func<TodoItemsService1, Task<T>>> scenarioExpression)
		{
			using (_dbContextScopeFactory.Create())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService1).FullName);
				var todoItemsService = new TodoItemsService1(_ambientDbContextLocator, _data);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService).Result;
			}
		}

		public T ExecuteWithTodoItemsService2<T>(Expression<Func<TodoItemsService2, Task<T>>> scenarioExpression)
		{
			using (var dbContextScope = _dbContextScopeFactory.Create())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService2).FullName);
				var todoItemsService = new TodoItemsService2(dbContextScope, _ambientDbContextLocator, _userAlertService);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService).Result;
			}
		}

		public T ExecuteWithTodoItemsService3<T>(Expression<Func<TodoItemsService3, Task<T>>> scenarioExpression)
		{
			using (var dbContextScope = _dbContextScopeFactory.Create())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService3).FullName);
				var todoItemsService = new TodoItemsService3(new Data<IAmbientDbContextLocator>(_data, _ambientDbContextLocator), dbContextScope);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService).Result;
			}
		}
	}
}
