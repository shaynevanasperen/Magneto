using System;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;

namespace DemoApplication.EntityFramework.DbContext
{
	class TodoItemsService2
	{
		readonly DemoContext _demoContext;

		public TodoItemsService2(DemoContext demoContext)
		{
			_demoContext = demoContext;
		}

		public Task<TodoItem[]> GetAllTodoItemsAsync(int userId)
		{
			return _demoContext.Data().QueryAsync(new AllTodoItemsForUser { UserId = userId });
		}

		public Task<TodoItem[]> GetTodoItemsDueThisMonthAsync(int userId)
		{
			var today = DateTime.Today;
			var endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			return _demoContext.Data().QueryAsync(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfMonth });
		}

		public Task<TodoItem[]> GetUpcomingTodoItemsAsync(int userId, Priority priority)
		{
			return _demoContext.Data().QueryAsync(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = priority });
		}

		public async Task<bool> CompleteTodoItemAsync(int id)
		{
			var success = await _demoContext.Data().CommandAsync(new CompleteTodoItem { Id = id }).ConfigureAwait(false);
			await _demoContext.SaveChangesAsync().ConfigureAwait(false);
			return success;
		}
	}
}