using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.NHibernate.LazySession;
using Machine.Fakes;
using Machine.Specifications;
using NHibernate.Sessions;

#pragma warning disable 0169 // For MSpec behaviour fields
// ReSharper disable once CheckNamespace
namespace DemoApplication.Tests.NHibernate.LazySession.TodoItemsService1Tests
{
	[Subject(typeof(TodoItemsService1))]
	class When_getting_todo_items_due_this_week : WithSubject<TodoItemsService1>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService1>> transaction_created_committed_and_disposed;

		It should_query_for_todo_items_for_the_specified_user_due_by_the_end_of_the_week = () =>
			The<IData>().WasToldTo(x => x.Query(new TodoItemsForUserDueBy { UserId = 1, DueDate = endOfWeek }, The<ILazySession>()));

		Because of = () =>
			Subject.GetTodoItemsDueThisWeek(1);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForALazySession>();
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
			The<IData>().WasToldTo(x => x.Query(new TodoItemsForUserCompletedLastWeek { UserId = 1 }, The<ILazySession>(), CacheOption.Default));

		Because of = () =>
			Subject.GetTodoItemsCompletedLastWeek(1);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForALazySession>();
		};
	}

	[Subject(typeof(TodoItemsService1))]
	class When_getting_upcoming_todo_items : WithSubject<TodoItemsService1>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService1>> transaction_created_committed_and_disposed;

		It should_query_for_upcoming_todo_items_with_the_specified_priority_for_the_specified_user = () =>
			The<IData>().WasToldTo(x => x.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = 1, Priority = Priority.Normal }, The<ILazySession>(), CacheOption.Default));

		Because of = () =>
			Subject.GetUpcomingTodoItems(1, Priority.Normal);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForALazySession>();
		};
	}
}
#pragma warning restore 0169
