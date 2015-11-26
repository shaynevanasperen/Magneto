using System;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.MultipleDatabases;

namespace DemoApplication.NHibernate.LazySessions
{
	class TodoItemsService3
	{
		readonly ITodoItemsData _todoItemsData;
		readonly IUsersData _usersData;
		readonly IUserAlertService _userAlertService;

		public TodoItemsService3(ITodoItemsData todoItemsData, IUsersData usersData, IUserAlertService userAlertService)
		{
			_todoItemsData = todoItemsData;
			_usersData = usersData;
			_userAlertService = userAlertService;
		}

		public User[] GetAllUsers()
		{
			using (var transaction = _usersData.Context.Session.BeginTransaction())
			{
				var users = _usersData.Query(new AllUsers());
				transaction.Commit();
				return users;
			}
		}

		public TodoItem[] GetTodoItemsDueThisWeek(Guid userId)
		{
			var endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
			using (var transaction = _todoItemsData.Context.Session.BeginTransaction())
			{
				var todoItems = _todoItemsData.Query(new TodoItemsForUserDueBy { UserId = userId, DueDate = endOfWeek });
				transaction.Commit();
				return todoItems;
			}
		}

		public TodoItem[] GetTodoItemsCompletedLastWeek(Guid userId)
		{
			using (var transaction = _todoItemsData.Context.Session.BeginTransaction())
			{
				var todoItems = _todoItemsData.Query(new TodoItemsForUserCompletedLastWeek { UserId = userId });
				transaction.Commit();
				return todoItems;
			}
		}

		public async Task<int> SendAlertsForTodoItemsDueTomorrowAsync(Guid userId)
		{
			var session = _todoItemsData.Context.Session; // This is done to ensure that the async continuation uses the same session
			using (var transaction = session.BeginTransaction())
			{
				var alertsSent = 0;
				foreach (var todoItem in _todoItemsData
					.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = userId, Priority = Priority.Urgent }, CacheOption.Refresh))
				{
					if (!todoItem.AlertSent && todoItem.DueDate <= DateTime.Today.AddDays(2))
					{
						await todoItem.SendAlertAsync(_userAlertService).ConfigureAwait(false);
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
