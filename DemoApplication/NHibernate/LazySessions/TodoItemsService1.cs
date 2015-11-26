using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.MultipleDatabases;

namespace DemoApplication.NHibernate.LazySessions
{
	class TodoItemsService1
	{
		readonly ITodoItemsContext _todoItemsContext;
		readonly IData _data;

		public TodoItemsService1(ITodoItemsContext todoItemsContext, IData data)
		{
			_todoItemsContext = todoItemsContext;
			_data = data;
		}

		public TodoItem[] GetAllTodoItems(Guid userId)
		{
			using (var transaction = _todoItemsContext.Session.BeginTransaction())
			{
				var todoItems = _data.Query(new AllTodoItemsForUser { UserId = userId }, _todoItemsContext);
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetTodoItemsDueThisMonth(Guid userId)
		{
			var today = DateTime.Today;
			var endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			using (var transaction = _todoItemsContext.Session.BeginTransaction())
			{
				var todoItems = _data.Query(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfMonth }, _todoItemsContext);
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetUpcomingTodoItems(Guid userId, Priority priority)
		{
			using (var transaction = _todoItemsContext.Session.BeginTransaction())
			{
				var todoItems = _data.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = priority }, _todoItemsContext);
				transaction.Commit();
				return todoItems;
			}
		}

		public bool CompleteTodoItem(int id)
		{
			using (var transaction = _todoItemsContext.Session.BeginTransaction())
			{
				var success = _data.Command(new CompleteTodoItem { Id = id }, _todoItemsContext);
				transaction.Commit();
				return success;
			}
		}
	}
}