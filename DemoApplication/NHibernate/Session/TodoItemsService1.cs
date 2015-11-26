using System;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;
using NHibernate;

namespace DemoApplication.NHibernate.Session
{
	class TodoItemsService1
	{
		readonly ISession _session;
		readonly IData _data;
		readonly IUserAlertService _userAlertService;

		public TodoItemsService1(ISession session, IData data, IUserAlertService userAlertService)
		{
			_session = session;
			_data = data;
			_userAlertService = userAlertService;
		}

		public User[] GetAllUsers()
		{
			using (var transaction = _session.BeginTransaction())
			{
				var users = _data.Query(new AllUsers(), _session);
				transaction.Commit();
				return users;
			}
		}

		public TodoItem[] GetTodoItemsDueThisWeek(int userId)
		{
			var endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
			using (var transaction = _session.BeginTransaction())
			{
				var todoItems = _data.Query(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfWeek }, _session);
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetTodoItemsCompletedLastWeek(int userId)
		{
			using (var transaction = _session.BeginTransaction())
			{
				var todoItems = _data.Query(new TodoItemsForUserCompletedLastWeek { UserId = userId }, _session);
				transaction.Commit();
				return todoItems;
			}
		}

		public async Task<int> SendAlertsForTodoItemsDueTomorrowAsync(int userId)
		{
			using (var transaction = _session.BeginTransaction())
			{
				var alertsSent = 0;
				foreach (var todoItem in _data
					.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = Priority.Urgent }, _session, CacheOption.Refresh))
				{
					if (!todoItem.AlertSent && todoItem.DueDate <= DateTime.Today.AddDays(2))
					{
						await todoItem.SendAlertAsync(_userAlertService).ConfigureAwait(false);
						_session.Update(todoItem);
						alertsSent++;
					}
				}
				transaction.Commit();
				return alertsSent;
			}
		}
	}
}