using System;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.MultipleDatabases;
using Mehdime.Entity;

namespace DemoApplication.EntityFramework.DbContextScope
{
	class TodoItemsService1
	{
		readonly IAmbientDbContextLocator _ambientDbContextLocator;
		readonly IData _data;

		public TodoItemsService1(IAmbientDbContextLocator ambientDbContextLocator, IData data)
		{
			_ambientDbContextLocator = ambientDbContextLocator;
			_data = data;
		}

		public Task<TodoItem[]> GetTodoItemsDueThisWeekAsync(Guid userId)
		{
			var endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
			return _data.QueryAsync(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfWeek }, _ambientDbContextLocator);
		}

		public Task<TodoItem[]> GetTodoItemsCompletedLastWeekAsync(Guid userId)
		{
			return _data.QueryAsync(new TodoItemsForUserCompletedLastWeek { UserId = userId }, _ambientDbContextLocator);
		}

		public Task<TodoItem[]> GetUpcomingTodoItemsAsync(Guid userId, Priority priority)
		{
			return _data.QueryAsync(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = priority }, _ambientDbContextLocator);
		}
	}
}
