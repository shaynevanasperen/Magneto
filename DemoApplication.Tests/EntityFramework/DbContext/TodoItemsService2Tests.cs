using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.EntityFramework.DbContext;
using Machine.Fakes;
using Machine.Specifications;

// ReSharper disable once CheckNamespace
namespace DemoApplication.Tests.EntityFramework.DbContext.TodoItemsService2Tests
{
	[Subject(typeof(TodoItemsService2))]
	class When_getting_all_todo_items : WithSubject<TodoItemsService2>
	{
		It should_query_for_all_todo_items_for_the_specified_user = () =>
			The<IData<DemoContext>>().WasToldTo(x => x.QueryAsync(new AllTodoItemsForUser { UserId = 1 }));

		Because of = () =>
			Subject.GetAllTodoItemsAsync(1).Wait();

		Establish context = () =>
			DataFactory.SetFactory<DemoContext>(x => The<IData<DemoContext>>());
	}

	[Subject(typeof(TodoItemsService2))]
	class When_getting_todo_items_due_this_month : WithSubject<TodoItemsService2>
	{
		It should_query_for_todo_items_for_the_specified_user_due_by_the_end_of_the_month = () =>
			The<IData<DemoContext>>().WasToldTo(x => x.QueryAsync(new TodoItemsForUserDueBy { UserId = 1, DueDate = endOfMonth }));

		Because of = () =>
			Subject.GetTodoItemsDueThisMonthAsync(1).Wait();

		Establish context = () =>
		{
			DataFactory.SetFactory<DemoContext>(x => The<IData<DemoContext>>());
			var today = DateTime.Today;
			endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
		};

		static DateTime endOfMonth;
	}

	[Subject(typeof(TodoItemsService2))]
	class When_getting_upcoming_todo_items : WithSubject<TodoItemsService2>
	{
		It should_query_for_upcoming_todo_items_with_the_specified_priority_for_the_specified_user = () =>
			The<IData<DemoContext>>().WasToldTo(x => x.QueryAsync(new UpcomingTodoItemsWithPriorityForUser { UserId = 1, Priority = Priority.Normal }, CacheOption.Default));

		Because of = () =>
			Subject.GetUpcomingTodoItemsAsync(1, Priority.Normal).Wait();

		Establish context = () =>
			DataFactory.SetFactory<DemoContext>(x => The<IData<DemoContext>>());
	}

	[Subject(typeof(TodoItemsService2))]
	class When_completing_a_todo_item : WithSubject<TodoItemsService2>
	{
		It should_execute_a_complete_todo_item_command_for_the_specified_todo_item = () =>
			The<IData<DemoContext>>().WasToldTo(x => x.CommandAsync(new CompleteTodoItem { Id = 1 }));

		Because of = () =>
			Subject.CompleteTodoItemAsync(1).Wait();

		Establish context = () =>
			DataFactory.SetFactory<DemoContext>(x => The<IData<DemoContext>>());
	}
}
