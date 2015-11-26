using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;
using DemoApplication.NHibernate.Session;
using Machine.Fakes;
using Machine.Specifications;
using NHibernate;

#pragma warning disable 0169 // For MSpec behaviour fields
// ReSharper disable once CheckNamespace
namespace DemoApplication.Tests.NHibernate.Session.TodoItemsService1Tests
{
	[Subject(typeof(TodoItemsService1))]
	class When_getting_all_users : WithSubject<TodoItemsService1>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService1>> transaction_created_committed_and_disposed;

		It should_query_for_all_users = () =>
			The<IData>().WasToldTo(x => x.Query(new AllUsers(), The<ISession>()));

		Because of = () =>
			Subject.GetAllUsers();

		Establish context = () =>
			With<ConfigForASession>();
	}

	[Subject(typeof(TodoItemsService1))]
	class When_getting_todo_items_due_this_week : WithSubject<TodoItemsService1>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService1>> transaction_created_committed_and_disposed;

		It should_query_for_todo_items_for_the_specified_user_due_by_the_end_of_the_week = () =>
			The<IData>().WasToldTo(x => x.Query(new TodoItemsForUserDueBy { UserId = 1, DueDate = endOfWeek }, The<ISession>()));

		Because of = () =>
			Subject.GetTodoItemsDueThisWeek(1);

		Establish context = () =>
		{
			With<ConfigForASession>();
			endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
		};

		static DateTime endOfWeek;
	}

	[Subject(typeof(TodoItemsService1))]
	class When_getting_todo_items_completed_last_week : WithSubject<TodoItemsService1>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService1>> transaction_created_committed_and_disposed;

		It should_query_for_todo_items_for_the_specified_user_completed_last_week = () =>
			The<IData>().WasToldTo(x => x.Query(new TodoItemsForUserCompletedLastWeek { UserId = 1 }, The<ISession>(), CacheOption.Default));

		Because of = () =>
			Subject.GetTodoItemsCompletedLastWeek(1);

		Establish context = () =>
			With<ConfigForASession>();
	}

	[Subject(typeof(TodoItemsService1))]
	class When_sending_alerts_for_todo_items_due_tomorrow : WithSubject<TodoItemsService1>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService1>> transaction_created_committed_and_disposed;

		It should_send_alerts_only_for_todo_items_returned_from_querying_urgent_upcoming_todo_items_for_the_specified_user_which_are_due_tomorrow_or_earlier = () =>
		{
#pragma warning disable 4014
			todoItem1.WasToldTo(x => x.SendAlertAsync(The<IUserAlertService>()));
			todoItem2.WasToldTo(x => x.SendAlertAsync(The<IUserAlertService>()));
			todoItem3.WasNotToldTo(x => x.SendAlertAsync(Param.IsAny<IUserAlertService>()));
#pragma warning restore 4014
		};

		It should_save_all_todo_items_for_which_an_alert_was_sent = () =>
		{
			The<ISession>().WasToldTo(x => x.Update(todoItem1));
			The<ISession>().WasToldTo(x => x.Update(todoItem2));
			The<ISession>().WasNotToldTo(x => x.Update(todoItem3));
		};

		Because of = () =>
			Subject.SendAlertsForTodoItemsDueTomorrowAsync(1).Wait();

		Establish context = () =>
		{
			With<ConfigForASession>();
			todoItem1 = An<TodoItem>();
			todoItem1.WhenToldTo(x => x.DueDate).Return(DateTime.Today.AddDays(1));
			todoItem2 = An<TodoItem>();
			todoItem2.WhenToldTo(x => x.DueDate).Return(DateTime.Today.AddDays(2));
			todoItem3 = An<TodoItem>();
			todoItem3.WhenToldTo(x => x.DueDate).Return(DateTime.Today.AddDays(3));
			The<IData>()
				.WhenToldTo(x => x.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = 1, Priority = Priority.Urgent }, The<ISession>(), CacheOption.Refresh))
				.Return(new[] { todoItem1, todoItem2, todoItem3 });
		};

		static TodoItem todoItem1, todoItem2, todoItem3;
	}
}
#pragma warning restore 0169
