using System;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;

namespace DemoApplication.EntityFramework.DbContext
{
	class TodoItemsService1
	{
		readonly DemoContext _demoContext;
		readonly IData _data;
		readonly IUserAlertService _userAlertService;

		public TodoItemsService1(DemoContext demoContext, IData data, IUserAlertService userAlertService)
		{
			_demoContext = demoContext;
			_data = data;
			_userAlertService = userAlertService;
		}

		public Task<User[]> GetAllUsersAsync()
		{
			return _data.QueryAsync(new AllUsers(), _demoContext);
		}

		public Task<TodoItem[]> GetTodoItemsDueThisWeekAsync(int userId)
		{
			var endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
			return _data.QueryAsync(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfWeek }, _demoContext);
		}

		public Task<TodoItem[]> GetTodoItemsCompletedLastWeekAsync(int userId)
		{
			return _data.QueryAsync(new TodoItemsForUserCompletedLastWeek { UserId = userId }, _demoContext);
		}

		public async Task<int> SendAlertsForTodoItemsDueTomorrowAsync(int userId)
		{
			foreach (var todoItem in await _data
				.QueryAsync(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = Priority.Urgent }, _demoContext, CacheOption.Refresh)
				.ConfigureAwait(false))
			{
				if (!todoItem.AlertSent && todoItem.DueDate <= DateTime.Today.AddDays(2))
					await todoItem.SendAlertAsync(_userAlertService).ConfigureAwait(false);
			}
			var updateCount = await _demoContext.SaveChangesAsync().ConfigureAwait(false);
			return updateCount;
		}
	}
}