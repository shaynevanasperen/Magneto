using System;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;

namespace DemoApplication.EntityFramework.DbContext
{
	class TodoItemsService3
	{
		readonly IData<DemoContext> _data;

		public TodoItemsService3(IData<DemoContext> data)
		{
			_data = data;
		}

		public Task<TodoItem[]> GetTodoItemsDueThisWeekAsync(int userId)
		{
			var endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
			return _data.QueryAsync(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfWeek });
		}

		public Task<TodoItem[]> GetTodoItemsCompletedLastWeekAsync(int userId)
		{
			return _data.QueryAsync(new TodoItemsForUserCompletedLastWeek { UserId = userId });
		}

		public Task<TodoItem[]> GetUpcomingTodoItemsAsync(int userId, Priority priority)
		{
			return _data.QueryAsync(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = priority });
		}
	}
}
