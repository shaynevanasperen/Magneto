using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;
using NHibernate;

namespace DemoApplication.NHibernate.Session
{
	class TodoItemsService3
	{
		readonly IData<ISession> _data;

		public TodoItemsService3(IData<ISession> data)
		{
			_data = data;
		}

		public TodoItem[] GetTodoItemsDueThisWeek(int userId)
		{
			var endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
			using (var transaction = _data.Context.BeginTransaction())
			{
				var todoItems = _data.Query(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfWeek });
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetTodoItemsCompletedLastWeek(int userId)
		{
			using (var transaction = _data.Context.BeginTransaction())
			{
				var todoItems = _data.Query(new TodoItemsForUserCompletedLastWeek { UserId = userId });
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetUpcomingTodoItems(int userId, Priority priority)
		{
			using (var transaction = _data.Context.BeginTransaction())
			{
				var todoItems = _data.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = priority });
				transaction.Commit();
				return todoItems;
			}
		}
	}
}
