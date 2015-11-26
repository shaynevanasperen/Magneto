using System;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.MultipleDatabases;
using Mehdime.Entity;

namespace DemoApplication.EntityFramework.DbContextScope
{
	class TodoItemsService2
	{
		readonly IDbContextScope _dbContextScope;
		readonly IAmbientDbContextLocator _ambientDbContextLocator;
		readonly IUserAlertService _userAlertService;

		public TodoItemsService2(IDbContextScope dbContextScope, IAmbientDbContextLocator ambientDbContextLocator, IUserAlertService userAlertService)
		{
			_dbContextScope = dbContextScope;
			_ambientDbContextLocator = ambientDbContextLocator;
			_userAlertService = userAlertService;
		}

		public Task<User[]> GetAllUsersAsync()
		{
			return _ambientDbContextLocator.Data().QueryAsync(new AllUsers());
		}

		public Task<TodoItem[]> GetTodoItemsDueThisWeekAsync(Guid userId)
		{
			var endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
			return _ambientDbContextLocator.Data().QueryAsync(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfWeek });
		}

		public Task<TodoItem[]> GetTodoItemsCompletedLastWeekAsync(Guid userId)
		{
			return _ambientDbContextLocator.Data().QueryAsync(new TodoItemsForUserCompletedLastWeek { UserId = userId });
		}

		public async Task<int> SendAlertsForTodoItemsDueTomorrowAsync(Guid userId)
		{
			foreach (var todoItem in await _ambientDbContextLocator.Data()
				.QueryAsync(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = Priority.Urgent }, CacheOption.Refresh)
				.ConfigureAwait(false))
			{
				if (!todoItem.AlertSent && todoItem.DueDate <= DateTime.Today.AddDays(2))
					await todoItem.SendAlertAsync(_userAlertService).ConfigureAwait(false);
			}
			var updateCount = await _dbContextScope.SaveChangesAsync().ConfigureAwait(false);
			return updateCount;
		}
	}
}
