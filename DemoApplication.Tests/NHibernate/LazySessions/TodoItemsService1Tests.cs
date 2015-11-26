using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.NHibernate.LazySessions;
using Machine.Fakes;
using Machine.Specifications;
using NHibernate;

#pragma warning disable 0169 // For MSpec behaviour fields
// ReSharper disable once CheckNamespace
namespace DemoApplication.Tests.NHibernate.LazySessions.TodoItemsService1Tests
{
	[Subject(typeof(TodoItemsService1))]
	class When_getting_all_todo_items : WithSubject<TodoItemsService1>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService1>> transaction_created_committed_and_disposed;

		It should_query_for_all_todo_items_for_the_specified_user = () =>
			The<IData>().WasToldTo(x => x.Query(new AllTodoItemsForUser { UserId = Guid.Empty }, The<ITodoItemsContext>()));

		Because of = () =>
			Subject.GetAllTodoItems(Guid.Empty);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForATodoItemsContext>();
			DataFactory.SetFactory<ISession>(x => The<IData<ISession>>());
		};
	}

	[Subject(typeof(TodoItemsService1))]
	class When_getting_todo_items_due_this_month : WithSubject<TodoItemsService1>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService1>> transaction_created_committed_and_disposed;

		It should_query_for_todo_items_for_the_specified_user_due_by_the_end_of_the_month = () =>
			The<IData>().WasToldTo(x => x.Query(new TodoItemsForUserDueBy { UserId = Guid.Empty, DueDate = endOfMonth }, The<ITodoItemsContext>()));

		Because of = () =>
			Subject.GetTodoItemsDueThisMonth(Guid.Empty);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForATodoItemsContext>();
			DataFactory.SetFactory<ISession>(x => The<IData<ISession>>());
			var today = DateTime.Today;
			endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
		};

		static DateTime endOfMonth;
	}

	[Subject(typeof(TodoItemsService1))]
	class When_getting_upcoming_todo_items : WithSubject<TodoItemsService1>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService1>> transaction_created_committed_and_disposed;

		It should_query_for_upcoming_todo_items_with_the_specified_priority_for_the_specified_user = () =>
			The<IData>().WasToldTo(x => x.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = Guid.Empty, Priority = Priority.Normal }, The<ITodoItemsContext>(), CacheOption.Default));

		Because of = () =>
			Subject.GetUpcomingTodoItems(Guid.Empty, Priority.Normal);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForATodoItemsContext>();
			DataFactory.SetFactory<ISession>(x => The<IData<ISession>>());
		};
	}

	[Subject(typeof(TodoItemsService1))]
	class When_completing_a_todo_item : WithSubject<TodoItemsService1>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService1>> transaction_created_committed_and_disposed;

		It should_execute_a_complete_todo_item_command_for_the_specified_todo_item = () =>
			The<IData>().WasToldTo(x => x.Command(new CompleteTodoItem { Id = 1 }, The<ITodoItemsContext>()));

		Because of = () =>
			Subject.CompleteTodoItem(1);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForATodoItemsContext>();
			DataFactory.SetFactory<ISession>(x => The<IData<ISession>>());
		};
	}
}
#pragma warning restore 0169
