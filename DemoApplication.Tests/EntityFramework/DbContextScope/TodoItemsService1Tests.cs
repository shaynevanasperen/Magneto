using System;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.EntityFramework.DbContextScope;
using Machine.Fakes;
using Machine.Specifications;
using Mehdime.Entity;

// ReSharper disable once CheckNamespace
namespace DemoApplication.Tests.EntityFramework.DbContextScope.TodoItemsService1Tests
{
	[Subject(typeof(TodoItemsService1))]
	class When_getting_todo_items_due_this_week : WithSubject<TodoItemsService1>
	{
		It should_query_for_todo_items_for_the_specified_user_due_by_the_end_of_the_week = () =>
			The<IData>().WasToldTo(x => x.QueryAsync(new TodoItemsForUserDueBy { UserId = Guid.Empty, DueDate = endOfWeek }, The<IAmbientDbContextLocator>()));

		Because of = () =>
			Subject.GetTodoItemsDueThisWeekAsync(Guid.Empty).Wait();

		Establish context = () =>
		{
			endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
		};

		static DateTime endOfWeek;
	}

	[Subject(typeof(TodoItemsService1))]
	class When_getting_todo_items_completed_last_week : WithSubject<TodoItemsService1>
	{
		It should_query_for_todo_items_for_the_specified_user_completed_last_week = () =>
			The<IData>().WasToldTo(x => x.QueryAsync(new TodoItemsForUserCompletedLastWeek { UserId = Guid.Empty }, The<IAmbientDbContextLocator>(), CacheOption.Default));

		Because of = () =>
			Subject.GetTodoItemsCompletedLastWeekAsync(Guid.Empty).Wait();
	}

	[Subject(typeof(TodoItemsService1))]
	class When_getting_upcoming_todo_items : WithSubject<TodoItemsService1>
	{
		It should_query_for_upcoming_todo_items_with_the_specified_priority_for_the_specified_user = () =>
			The<IData>().WasToldTo(x => x.QueryAsync(new UpcomingTodoItemsWithPriorityForUser { UserId = Guid.Empty, Priority = Priority.Normal }, The<IAmbientDbContextLocator>(), CacheOption.Default));

		Because of = () =>
			Subject.GetUpcomingTodoItemsAsync(Guid.Empty, Priority.Normal).Wait();
	}
}
