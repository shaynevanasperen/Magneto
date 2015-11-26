using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.NHibernate.LazySession;
using Machine.Fakes;
using Machine.Specifications;
using NHibernate.Sessions;

#pragma warning disable 0169 // For MSpec behaviour fields
// ReSharper disable once CheckNamespace
namespace DemoApplication.Tests.NHibernate.LazySession.TodoItemsService3Tests
{
	[Subject(typeof(TodoItemsService3))]
	class When_getting_all_todo_items : WithSubject<TodoItemsService3>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService3>> transaction_created_committed_and_disposed;

		It should_query_for_all_todo_items_for_the_specified_user = () =>
			The<IData<ILazySession>>().WasToldTo(x => x.Query(new AllTodoItemsForUser { UserId = 1 }));

		Because of = () =>
			Subject.GetAllTodoItems(1);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForALazySession>();
			With<ConfigForAData<ILazySession>>();
		};
	}

	[Subject(typeof(TodoItemsService3))]
	class When_getting_todo_items_due_this_month : WithSubject<TodoItemsService3>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService3>> transaction_created_committed_and_disposed;

		It should_query_for_todo_items_for_the_specified_user_due_by_the_end_of_the_month = () =>
			The<IData<ILazySession>>().WasToldTo(x => x.Query(new TodoItemsForUserDueBy { UserId = 1, DueDate = endOfMonth }));

		Because of = () =>
			Subject.GetTodoItemsDueThisMonth(1);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForALazySession>();
			With<ConfigForAData<ILazySession>>();
			var today = DateTime.Today;
			endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
		};

		static DateTime endOfMonth;
	}

	[Subject(typeof(TodoItemsService3))]
	class When_getting_upcoming_todo_items : WithSubject<TodoItemsService3>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService3>> transaction_created_committed_and_disposed;

		It should_query_for_upcoming_todo_items_with_the_specified_priority_for_the_specified_user = () =>
			The<IData<ILazySession>>().WasToldTo(x => x.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = 1, Priority = Priority.Normal }, CacheOption.Default));

		Because of = () =>
			Subject.GetUpcomingTodoItems(1, Priority.Normal);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForALazySession>();
			With<ConfigForAData<ILazySession>>();
		};
	}

	[Subject(typeof(TodoItemsService3))]
	class When_completing_a_todo_item : WithSubject<TodoItemsService3>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService3>> transaction_created_committed_and_disposed;

		It should_execute_a_complete_todo_item_command_for_the_specified_todo_item = () =>
			The<IData<ILazySession>>().WasToldTo(x => x.Command(new CompleteTodoItem { Id = 1 }));

		Because of = () =>
			Subject.CompleteTodoItem(1);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForALazySession>();
			With<ConfigForAData<ILazySession>>();
		};
	}
}
#pragma warning restore 0169
