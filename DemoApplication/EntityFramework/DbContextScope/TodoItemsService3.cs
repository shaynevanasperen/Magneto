using System;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.MultipleDatabases;
using Mehdime.Entity;

namespace DemoApplication.EntityFramework.DbContextScope
{
	class TodoItemsService3
	{
		readonly IData<IAmbientDbContextLocator> _data;
		readonly IDbContextScope _dbContextScope;

		public TodoItemsService3(IData<IAmbientDbContextLocator> data, IDbContextScope dbContextScope)
		{
			_data = data;
			_dbContextScope = dbContextScope;
		}

		public Task<TodoItem[]> GetAllTodoItemsAsync(Guid userId)
		{
			return _data.QueryAsync(new AllTodoItemsForUser { UserId = userId });
		}

		public Task<TodoItem[]> GetTodoItemsDueThisMonthAsync(Guid userId)
		{
			var today = DateTime.Today;
			var endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			return _data.QueryAsync(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfMonth });
		}

		public Task<TodoItem[]> GetUpcomingTodoItemsAsync(Guid userId, Priority priority)
		{
			return _data.QueryAsync(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = priority });
		}

		public async Task<bool> CompleteTodoItemAsync(int id)
		{
			var success = await _data.CommandAsync(new CompleteTodoItem { Id = id }).ConfigureAwait(false);
			await _dbContextScope.SaveChangesAsync().ConfigureAwait(false);
			return success;
		}
	}
}
