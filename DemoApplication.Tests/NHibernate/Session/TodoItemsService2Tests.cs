using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.NHibernate.Session;
using Machine.Fakes;
using Machine.Specifications;
using NHibernate;

#pragma warning disable 0169 // For MSpec behaviour fields
// ReSharper disable once CheckNamespace
namespace DemoApplication.Tests.NHibernate.Session.TodoItemsService2Tests
{
	[Subject(typeof(TodoItemsService2))]
	class When_getting_all_todo_items : WithSubject<TodoItemsService2>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService2>> transaction_created_committed_and_disposed;

		It should_query_for_all_todo_items_for_the_specified_user = () =>
			The<IData<ISession>>().WasToldTo(x => x.Query(new AllTodoItemsForUser { UserId = 1 }));

		Because of = () =>
			Subject.GetAllTodoItems(1);

		Establish context = () =>
		{
			With<ConfigForASession>();
			DataFactory.SetFactory<ISession>(x => The<IData<ISession>>());
		};
	}

	[Subject(typeof(TodoItemsService2))]
	class When_getting_todo_items_due_this_month : WithSubject<TodoItemsService2>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService2>> transaction_created_committed_and_disposed;

		It should_query_for_todo_items_for_the_specified_user_due_by_the_end_of_the_month = () =>
			The<IData<ISession>>().WasToldTo(x => x.Query(new TodoItemsForUserDueBy { UserId = 1, DueDate = endOfMonth }));

		Because of = () =>
			Subject.GetTodoItemsDueThisMonth(1);

		Establish context = () =>
		{
			With<ConfigForASession>();
			DataFactory.SetFactory<ISession>(x => The<IData<ISession>>());
			var today = DateTime.Today;
			endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
		};

		static DateTime endOfMonth;
	}

	[Subject(typeof(TodoItemsService2))]
	class When_getting_upcoming_todo_items : WithSubject<TodoItemsService2>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService2>> transaction_created_committed_and_disposed;

		It should_query_for_upcoming_todo_items_with_the_specified_priority_for_the_specified_user = () =>
			The<IData<ISession>>().WasToldTo(x => x.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = 1, Priority = Priority.Normal }, CacheOption.Default));

		Because of = () =>
			Subject.GetUpcomingTodoItems(1, Priority.Normal);

		Establish context = () =>
		{
			With<ConfigForASession>();
			DataFactory.SetFactory<ISession>(x => The<IData<ISession>>());
		};
	}

	[Subject(typeof(TodoItemsService2))]
	class When_completing_a_todo_item : WithSubject<TodoItemsService2>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService2>> transaction_created_committed_and_disposed;

		It should_execute_a_complete_todo_item_command_for_the_specified_todo_item = () =>
			The<IData<ISession>>().WasToldTo(x => x.Command(new CompleteTodoItem { Id = 1 }));

		Because of = () =>
			Subject.CompleteTodoItem(1);

		Establish context = () =>
		{
			With<ConfigForASession>();
			DataFactory.SetFactory<ISession>(x => The<IData<ISession>>());
		};
	}
}
#pragma warning restore 0169
