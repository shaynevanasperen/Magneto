using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.MultipleDatabases;

namespace DemoApplication.NHibernate.LazySessions
{
	class TodoItemsService2
	{
		readonly ITodoItemsContext _todoItemsContext;

		public TodoItemsService2(ITodoItemsContext todoItemsContext)
		{
			_todoItemsContext = todoItemsContext;
		}

		public TodoItem[] GetTodoItemsDueThisWeek(Guid userId)
		{
			var endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
			using (var transaction = _todoItemsContext.Session.BeginTransaction())
			{
				var todoItems = _todoItemsContext.Data().Query(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfWeek });
				transaction.Commit();
				return todoItems;
			}
		}
		public TodoItem[] GetTodoItemsCompletedLastWeek(Guid userId)
		{
			using (var transaction = _todoItemsContext.Session.BeginTransaction())
			{
				var todoItems = _todoItemsContext.Data().Query(new TodoItemsForUserCompletedLastWeek { UserId = userId });
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetUpcomingTodoItems(Guid userId, Priority priority)
		{
			using (var transaction = _todoItemsContext.Session.BeginTransaction())
			{
				var todoItems = _todoItemsContext.Data().Query(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = priority });
				transaction.Commit();
				return todoItems;
			}
		}
	}
}