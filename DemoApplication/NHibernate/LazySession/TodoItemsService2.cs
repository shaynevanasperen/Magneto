using System;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;
using NHibernate.Sessions;

namespace DemoApplication.NHibernate.LazySession
{
	class TodoItemsService2
	{
		readonly ILazySession _lazySession;
		readonly IUserAlertService _userAlertService;

		public TodoItemsService2(ILazySession lazySession, IUserAlertService userAlertService)
		{
			_lazySession = lazySession;
			_userAlertService = userAlertService;
		}

		public User[] GetAllUsers()
		{
			using (var transaction = _lazySession.Current.BeginTransaction())
			{
				var users = _lazySession.Data().Query(new AllUsers());
				transaction.Commit();
				return users;
			}
		}

		public TodoItem[] GetTodoItemsDueThisWeek(int userId)
		{
			var endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
			using (var transaction = _lazySession.Current.BeginTransaction())
			{
				var todoItems = _lazySession.Data().Query(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfWeek });
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetTodoItemsCompletedLastWeek(int userId)
		{
			using (var transaction = _lazySession.Current.BeginTransaction())
			{
				var todoItems = _lazySession.Data().Query(new TodoItemsForUserCompletedLastWeek { UserId = userId });
				transaction.Commit();
				return todoItems;
			}
		}

		public async Task<int> SendAlertsForTodoItemsDueTomorrowAsync(int userId)
		{
			var session = _lazySession.Current; // This is done to ensure that the async continuation uses the same session
            using (var transaction = _lazySession.Current.BeginTransaction())
			{
				var alertsSent = 0;
				foreach (var todoItem in _lazySession.Data()
					.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = Priority.Urgent }, CacheOption.Refresh))
				{
					if (!todoItem.AlertSent && todoItem.DueDate <= DateTime.Today.AddDays(2))
					{
						await todoItem.SendAlertAsync(_userAlertService).ConfigureAwait(true);
						session.Update(todoItem);
						alertsSent++;
					}
				}
				transaction.Commit();
				return alertsSent;
			}
		}
	}
}