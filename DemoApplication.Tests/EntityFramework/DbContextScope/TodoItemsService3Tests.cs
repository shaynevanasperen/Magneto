using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.EntityFramework.DbContextScope;
using Machine.Fakes;
using Machine.Specifications;
using Mehdime.Entity;

// ReSharper disable once CheckNamespace
namespace DemoApplication.Tests.EntityFramework.DbContextScope.TodoItemsService3Tests
{
	[Subject(typeof(TodoItemsService3))]
	class When_getting_all_todo_items : WithSubject<TodoItemsService3>
	{
		It should_query_for_all_todo_items_for_the_specified_user = () =>
			The<IData<IAmbientDbContextLocator>>().WasToldTo(x => x.QueryAsync(new AllTodoItemsForUser { UserId = Guid.Empty }));

		Because of = () =>
			Subject.GetAllTodoItemsAsync(Guid.Empty).Wait();

		Establish context = () =>
			With<ConfigForAData<IAmbientDbContextLocator>>();
	}

	[Subject(typeof(TodoItemsService3))]
	class When_getting_todo_items_due_this_month : WithSubject<TodoItemsService3>
	{
		It should_query_for_todo_items_for_the_specified_user_due_by_the_end_of_the_month = () =>
			The<IData<IAmbientDbContextLocator>>().WasToldTo(x => x.QueryAsync(new TodoItemsForUserDueBy { UserId = Guid.Empty, DueDate = endOfMonth }));

		Because of = () =>
			Subject.GetTodoItemsDueThisMonthAsync(Guid.Empty).Wait();

		Establish context = () =>
		{
			With<ConfigForAData<IAmbientDbContextLocator>>();
			var today = DateTime.Today;
			endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
		};

		static DateTime endOfMonth;
	}

	[Subject(typeof(TodoItemsService3))]
	class When_getting_upcoming_todo_items : WithSubject<TodoItemsService3>
	{
		It should_query_for_upcoming_todo_items_with_the_specified_priority_for_the_specified_user = () =>
			The<IData<IAmbientDbContextLocator>>().WasToldTo(x => x.QueryAsync(new UpcomingTodoItemsWithPriorityForUser { UserId = Guid.Empty, Priority = Priority.Normal }, CacheOption.Default));

		Because of = () =>
			Subject.GetUpcomingTodoItemsAsync(Guid.Empty, Priority.Normal).Wait();

		Establish context = () =>
			With<ConfigForAData<IAmbientDbContextLocator>>();
	}

	[Subject(typeof(TodoItemsService3))]
	class When_completing_a_todo_item : WithSubject<TodoItemsService3>
	{
		It should_execute_a_complete_todo_item_command_for_the_specified_todo_item = () =>
			The<IData<IAmbientDbContextLocator>>().WasToldTo(x => x.CommandAsync(new CompleteTodoItem { Id = 1 }));

		Because of = () =>
			Subject.CompleteTodoItemAsync(1).Wait();

		Establish context = () =>
			With<ConfigForAData<IAmbientDbContextLocator>>();
	}
}
