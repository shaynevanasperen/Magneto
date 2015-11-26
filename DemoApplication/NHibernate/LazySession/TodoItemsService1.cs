using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;
using NHibernate.Sessions;

namespace DemoApplication.NHibernate.LazySession
{
	class TodoItemsService1
	{
		readonly ILazySession _lazySession;
		readonly IData _data;

		public TodoItemsService1(ILazySession lazySession, IData data)
		{
			_lazySession = lazySession;
			_data = data;
		}

		public TodoItem[] GetTodoItemsDueThisWeek(int userId)
		{
			var endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
			using (var transaction = _lazySession.Current.BeginTransaction())
			{
				var todoItems = _data.Query(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfWeek }, _lazySession);
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetTodoItemsCompletedLastWeek(int userId)
		{
			using (var transaction = _lazySession.Current.BeginTransaction())
			{
				var todoItems = _data.Query(new TodoItemsForUserCompletedLastWeek { UserId = userId }, _lazySession);
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetUpcomingTodoItems(int userId, Priority priority)
		{
			using (var transaction = _lazySession.Current.BeginTransaction())
			{
				var todoItems = _data.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = priority }, _lazySession);
				transaction.Commit();
				return todoItems;
			}
		}
	}
}