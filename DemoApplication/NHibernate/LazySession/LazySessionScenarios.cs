using System;
using System.Linq;
using System.Linq.Expressions;
using Data.Operations;
using DemoApplication.DomainModel.SingleDatabase;
using NHibernate.Sessions;

namespace DemoApplication.NHibernate.LazySession
{
	class LazySessionScenarios : IDisposable
	{
		readonly ILazySessionScoper _lazySessionScoper;
		readonly IData _data;
		readonly IUserAlertService _userAlertService;

		public LazySessionScenarios(ILazySessionScoper lazySessionScoper, IData data, IUserAlertService userAlertService)
		{
			_lazySessionScoper = lazySessionScoper;
			_data = data;
			_userAlertService = userAlertService;
			createSampleData();
		}

		void createSampleData()
		{
			Console.WriteLine("Creating sample data for {0}", GetType().Name);
			var sampleTodoItems = SampleDataFactory.CreateTodoItems();
			using (var scope = _lazySessionScoper.Scope())
			using (var transaction = scope.Current.BeginTransaction())
			{
				foreach (var sampleUser in sampleTodoItems.Select(x => x.User).Distinct())
					scope.Current.Save(sampleUser);
				foreach (var sampleTodoItem in sampleTodoItems)
					scope.Current.Save(sampleTodoItem);
				transaction.Commit();
			}
		}

		void deleteAllData()
		{
			Console.WriteLine("Deleting all data for {0}", GetType().Name);
			using (var scope = _lazySessionScoper.Scope())
			using (var transaction = scope.Current.BeginTransaction())
			{
				scope.Current.CreateQuery("delete TodoItem t").ExecuteUpdate();
				scope.Current.CreateQuery("delete User u").ExecuteUpdate();
				transaction.Commit();
			}
		}

		public void Dispose()
		{
			deleteAllData();
			_lazySessionScoper.Dispose();
		}

		public T ExecuteWithTodoItemsService1<T>(Expression<Func<TodoItemsService1, T>> scenarioExpression)
		{
			using (var scope = _lazySessionScoper.Scope())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService1).FullName);
				var todoItemsService = new TodoItemsService1(scope, _data);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService);
			}
		}

		public T ExecuteWithTodoItemsService2<T>(Expression<Func<TodoItemsService2, T>> scenarioExpression)
		{
			using (var scope = _lazySessionScoper.Scope())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService2).FullName);
				var todoItemsService = new TodoItemsService2(scope, _userAlertService);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService);
			}
		}

		public T ExecuteWithTodoItemsService3<T>(Expression<Func<TodoItemsService3, T>> scenarioExpression)
		{
			using (var scope = _lazySessionScoper.Scope())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService3).FullName);
				var todoItemsService = new TodoItemsService3(new Data<ILazySession>(_data, scope));
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService);
			}
		}
	}
}
