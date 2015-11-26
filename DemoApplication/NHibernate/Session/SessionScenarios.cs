using System;
using System.Linq;
using System.Linq.Expressions;
using Data.Operations;
using DemoApplication.DomainModel.SingleDatabase;
using NHibernate;

namespace DemoApplication.NHibernate.Session
{
	class SessionScenarios : IDisposable
	{
		readonly ISessionFactory _sessionFactory;
		readonly IData _data;
		readonly IUserAlertService _userAlertService;

		public SessionScenarios(ISessionFactory sessionFactory, IData data, IUserAlertService userAlertService)
		{
			_sessionFactory = sessionFactory;
			_data = data;
			_userAlertService = userAlertService;
			createSampleData();
		}

		void createSampleData()
		{
			Console.WriteLine("Creating sample data for {0}", GetType().Name);
			var sampleTodoItems = SampleDataFactory.CreateTodoItems();
			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				foreach (var sampleUser in sampleTodoItems.Select(x => x.User).Distinct())
					session.Save(sampleUser);
				foreach (var sampleTodoItem in sampleTodoItems)
					session.Save(sampleTodoItem);
				transaction.Commit();
			}
		}

		void deleteAllData()
		{
			Console.WriteLine("Deleting all data for {0}", GetType().Name);
			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete TodoItem t").ExecuteUpdate();
				session.CreateQuery("delete User u").ExecuteUpdate();
				transaction.Commit();
			}
		}

		public void Dispose()
		{
			deleteAllData();
			_sessionFactory.Dispose();
		}

		public T ExecuteWithTodoItemsService1<T>(Expression<Func<TodoItemsService1, T>> scenarioExpression)
		{
			using (var session = _sessionFactory.OpenSession())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService1).FullName);
				var todoItemsService = new TodoItemsService1(session, _data, _userAlertService);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService);
			}
		}

		public T ExecuteWithTodoItemsService2<T>(Expression<Func<TodoItemsService2, T>> scenarioExpression)
		{
			using (var session = _sessionFactory.OpenSession())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService2).FullName);
				var todoItemsService = new TodoItemsService2(session);
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService);
			}
		}

		public T ExecuteWithTodoItemsService3<T>(Expression<Func<TodoItemsService3, T>> scenarioExpression)
		{
			using (var session = _sessionFactory.OpenSession())
			{
				Console.WriteLine("Creating a {0}", typeof(TodoItemsService3).FullName);
				var todoItemsService = new TodoItemsService3(new Data<ISession>(_data, session));
				todoItemsService.Log(scenarioExpression);
				return scenarioExpression.Compile()(todoItemsService);
			}
		}
	}
}
