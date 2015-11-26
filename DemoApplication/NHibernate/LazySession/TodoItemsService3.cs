using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;
using NHibernate.Sessions;

namespace DemoApplication.NHibernate.LazySession
{
	class TodoItemsService3
	{
		readonly IData<ILazySession> _data;

		public TodoItemsService3(IData<ILazySession> data)
		{
			_data = data;
		}

		public TodoItem[] GetAllTodoItems(int userId)
		{
			using (var transaction = _data.Context.Current.BeginTransaction())
			{
				var todoItems = _data.Query(new AllTodoItemsForUser { UserId = userId });
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetTodoItemsDueThisMonth(int userId)
		{
			var today = DateTime.Today;
			var endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			using (var transaction = _data.Context.Current.BeginTransaction())
			{
				var todoItems = _data.Query(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfMonth });
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetUpcomingTodoItems(int userId, Priority priority)
		{
			using (var transaction = _data.Context.Current.BeginTransaction())
			{
				var todoItems = _data.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = priority });
				transaction.Commit();
				return todoItems;
			}
		}

		public bool CompleteTodoItem(int id)
		{
			using (var transaction = _data.Context.Current.BeginTransaction())
			{
				var success = _data.Command(new CompleteTodoItem { Id = id });
				transaction.Commit();
				return success;
			}
		}
	}
}
