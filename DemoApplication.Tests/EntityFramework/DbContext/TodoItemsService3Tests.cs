using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.EntityFramework.DbContext;
using Machine.Fakes;
using Machine.Specifications;

// ReSharper disable once CheckNamespace
namespace DemoApplication.Tests.EntityFramework.DbContext.TodoItemsService3Tests
{
	[Subject(typeof(TodoItemsService3))]
	class When_getting_todo_items_due_this_week : WithSubject<TodoItemsService3>
	{
		It should_query_for_todo_items_for_the_specified_user_due_by_the_end_of_the_week = () =>
			The<IData<DemoContext>>().WasToldTo(x => x.QueryAsync(new TodoItemsForUserDueBy { UserId = 1, DueDate = endOfWeek }));

		Because of = () =>
			Subject.GetTodoItemsDueThisWeekAsync(1).Wait();

		Establish context = () =>
		{
			With<ConfigForAData<DemoContext>>();
			endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
		};

		static DateTime endOfWeek;
	}

	[Subject(typeof(TodoItemsService3))]
	class When_getting_todo_items_completed_last_week : WithSubject<TodoItemsService3>
	{
		It should_query_for_todo_items_for_the_specified_user_completed_last_week = () =>
			The<IData<DemoContext>>().WasToldTo(x => x.QueryAsync(new TodoItemsForUserCompletedLastWeek { UserId = 1 }, CacheOption.Default));

		Because of = () =>
			Subject.GetTodoItemsCompletedLastWeekAsync(1).Wait();

		Establish context = () =>
			With<ConfigForAData<DemoContext>>();
	}

	[Subject(typeof(TodoItemsService3))]
	class When_getting_upcoming_todo_items : WithSubject<TodoItemsService3>
	{
		It should_query_for_upcoming_todo_items_with_the_specified_priority_for_the_specified_user = () =>
			The<IData<DemoContext>>().WasToldTo(x => x.QueryAsync(new UpcomingTodoItemsWithPriorityForUser { UserId = 1, Priority = Priority.Normal }, CacheOption.Default));

		Because of = () =>
			Subject.GetUpcomingTodoItemsAsync(1, Priority.Normal).Wait();

		Establish context = () =>
			With<ConfigForAData<DemoContext>>();
	}
}
