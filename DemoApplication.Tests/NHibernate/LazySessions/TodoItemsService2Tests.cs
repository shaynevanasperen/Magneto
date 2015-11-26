using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.NHibernate.LazySessions;
using Machine.Fakes;
using Machine.Specifications;

#pragma warning disable 0169 // For MSpec behaviour fields
// ReSharper disable once CheckNamespace
namespace DemoApplication.Tests.NHibernate.LazySessions.TodoItemsService2Tests
{
	[Subject(typeof(TodoItemsService2))]
	class When_getting_todo_items_due_this_week : WithSubject<TodoItemsService2>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService2>> transaction_created_committed_and_disposed;

		It should_query_for_todo_items_for_the_specified_user_due_by_the_end_of_the_week = () =>
			The<IData<ITodoItemsContext>>().WasToldTo(x => x.Query(new TodoItemsForUserDueBy { UserId = Guid.Empty, DueDate = endOfWeek }));

		Because of = () =>
			Subject.GetTodoItemsDueThisWeek(Guid.Empty);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForATodoItemsContext>();
			DataFactory.SetFactory<ITodoItemsContext>(x => The<IData<ITodoItemsContext>>());
			endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
		};

		static DateTime endOfWeek;
	}

	[Subject(typeof(TodoItemsService2))]
	class When_getting_todo_items_completed_last_week : WithSubject<TodoItemsService2>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService2>> transaction_created_committed_and_disposed;

		It should_query_for_todo_items_for_the_specified_user_completed_last_week = () =>
			The<IData<ITodoItemsContext>>().WasToldTo(x => x.Query(new TodoItemsForUserCompletedLastWeek { UserId = Guid.Empty }, CacheOption.Default));

		Because of = () =>
			Subject.GetTodoItemsCompletedLastWeek(Guid.Empty);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForATodoItemsContext>();
			DataFactory.SetFactory<ITodoItemsContext>(x => The<IData<ITodoItemsContext>>());
		};
	}

	[Subject(typeof(TodoItemsService2))]
	class When_getting_upcoming_todo_items : WithSubject<TodoItemsService2>
	{
		Behaves_like<TransactionCreatedCommittedAndDisposed<TodoItemsService2>> transaction_created_committed_and_disposed;

		It should_query_for_upcoming_todo_items_with_the_specified_priority_for_the_specified_user = () =>
			The<IData<ITodoItemsContext>>().WasToldTo(x => x.Query(new UpcomingTodoItemsWithPriorityForUser { UserId = Guid.Empty, Priority = Priority.Normal }, CacheOption.Default));

		Because of = () =>
			Subject.GetUpcomingTodoItems(Guid.Empty, Priority.Normal);

		Establish context = () =>
		{
			With<ConfigForASession>();
			With<ConfigForATodoItemsContext>();
			DataFactory.SetFactory<ITodoItemsContext>(x => The<IData<ITodoItemsContext>>());
		};
	}
}
#pragma warning restore 0169
