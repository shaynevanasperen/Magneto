using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;
using NHibernate;

namespace DemoApplication.NHibernate.Session
{
	class TodoItemsService2
	{
		readonly ISession _session;

		public TodoItemsService2(ISession session)
		{
			_session = session;
		}

		public TodoItem[] GetAllTodoItems(int userId)
		{
			using (var transaction = _session.BeginTransaction())
			{
				var todoItems = _session.Data().Query(new AllTodoItemsForUser { UserId = userId });
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetTodoItemsDueThisMonth(int userId)
		{
			var today = DateTime.Today;
			var endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			using (var transaction = _session.BeginTransaction())
			{
				var todoItems = _session.Data().Query(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfMonth });
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetUpcomingTodoItems(int userId, Priority priority)
		{
			using (var transaction = _session.BeginTransaction())
			{
				var todoItems = _session.Data().Query(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = priority });
				transaction.Commit();
				return todoItems;
			}
		}

		public bool CompleteTodoItem(int id)
		{
			using (var transaction = _session.BeginTransaction())
			{
				var success = _session.Data().Command(new CompleteTodoItem { Id = id });
				transaction.Commit();
				return success;
			}
		}
	}
}