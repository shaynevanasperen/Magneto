using System;
using System.Linq.Expressions;
using Data.Operations;
using DemoApplication.DomainModel.MultipleDatabases;
using NHibernate.Sessions;

namespace DemoApplication.NHibernate.LazySessions
{
	class LazySessionsScenarios : IDisposable
	{
		readonly ILazySessionsScoper _lazySessionsScoper;
		readonly IData _data;
		readonly IUserAlertService _userAlertService;

		public LazySessionsScenarios(ILazySessionsScoper lazySessionsScoper, IData data, IUserAlertService userAlertService)
		{
			_lazySessionsScoper = lazySessionsScoper;
			_data = data;
			_userAlertService = userAlertService;
			createSampleData();
		}

		void createSampleData()
		{
			Console.WriteLine("Creating sample data for {0}", GetType().Name);
			using (var scope = _lazySessionsScoper.Scope())
			{
				var sampleUsers = SampleDataFactory.CreateUsers();
				using (var transaction = scope.CurrentFor<Users>().BeginTransaction())
				{
					scope.CurrentFor<Users>().Save(sampleUsers.Item1);
					scope.CurrentFor<Users>().Save(sampleUsers.Item2);
					transaction.Commit();
				}
				var sampleTodoItems = SampleDataFactory.CreateTodoItems(sampleUsers);
				using (var transaction = scope.CurrentFor<TodoItems>().BeginTransaction())
				{
					foreach (var sampleTodoItem in sampleTodoItems)
						scope.CurrentFor<TodoItems>().Save(sampleTodoItem);
					transaction.Commit();
				}
			}
		}

		void deleteAllData()
		{
			Console.WriteLine("Deleting all data for {0}", GetType().Name);
			using (var scope = _lazySessionsScoper.Scope())
			{
				using (var transaction = scope.CurrentFor<TodoItems>().BeginTransaction())
				{
					scope.CurrentFor<TodoItems>().CreateQuery("delete TodoItem t").ExecuteUpdate();
					transaction.Commit();
				}
				using (var transaction = scope.CurrentFor<Users>().BeginTransaction())
				{
					scope.CurrentFor<Users>().CreateQuery("delete User u").ExecuteUpdate();
					transaction.Commit();
				}
			}
		}

		public void Dispose()
		{
			deleteAllData();
			_lazySessionsScoper.Dispose();
		}

		public T ExecuteWithTodoItemsService1<T>(Expression<Func<TodoItemsService1, T>> scenarioExpression)
		{
			using (var todoItemsContext = new TodoItemsContext(_lazySessionsScoper.ScopeFor<TodoItems>()))
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService1).FullName);
				var todoItemsService = new TodoItemsService1(todoItemsContext, _data);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService);
			}
		}

		public T ExecuteWithTodoItemsService2<T>(Expression<Func<TodoItemsService2, T>> scenarioExpression)
		{
			using (var todoItemsContext = new TodoItemsContext(_lazySessionsScoper.ScopeFor<TodoItems>()))
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService2).FullName);
				var todoItemsService = new TodoItemsService2(todoItemsContext);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService);
			}
		}

		public T ExecuteWithTodoItemsService3<T>(Expression<Func<TodoItemsService3, T>> scenarioExpression)
		{
			using (var todoItemsData = new TodoItemsData(_data, new TodoItemsContext(_lazySessionsScoper.ScopeFor<TodoItems>())))
			using (var usersData = new UsersData(_data, new UsersContext(_lazySessionsScoper.ScopeFor<Users>())))
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService3).FullName);
				var todoItemsService = new TodoItemsService3(todoItemsData, usersData, _userAlertService);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService);
			}
		}
	}
}
