using System;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.MultipleDatabases;
using DemoApplication.EntityFramework.DbContextScope;
using Machine.Fakes;
using Machine.Specifications;
using Mehdime.Entity;

// ReSharper disable once CheckNamespace
namespace DemoApplication.Tests.EntityFramework.DbContextScope.TodoItemsService2Tests
{
	[Subject(typeof(TodoItemsService2))]
	class When_getting_all_users : WithSubject<TodoItemsService2>
	{
		It should_query_for_all_users = () =>
			The<IData<IAmbientDbContextLocator>>().WasToldTo(x => x.QueryAsync(new AllUsers()));

		Because of = () =>
			Subject.GetAllUsersAsync().Wait();

		Establish context = () =>
			DataFactory.SetFactory<IAmbientDbContextLocator>(x => The<IData<IAmbientDbContextLocator>>());
	}

	[Subject(typeof(TodoItemsService2))]
	class When_getting_todo_items_due_this_week : WithSubject<TodoItemsService2>
	{
		It should_query_for_todo_items_for_the_specified_user_due_by_the_end_of_the_week = () =>
			The<IData<IAmbientDbContextLocator>>().WasToldTo(x => x.QueryAsync(new TodoItemsForUserDueBy { UserId = Guid.Empty, DueDate = endOfWeek }));

		Because of = () =>
			Subject.GetTodoItemsDueThisWeekAsync(Guid.Empty).Wait();

		Establish context = () =>
		{
			DataFactory.SetFactory<IAmbientDbContextLocator>(x => The<IData<IAmbientDbContextLocator>>());
			endOfWeek = DateTime.Today;
			while (endOfWeek.DayOfWeek != DayOfWeek.Monday)
				endOfWeek = endOfWeek.AddDays(1);
		};

		static DateTime endOfWeek;
	}

	[Subject(typeof(TodoItemsService2))]
	class When_getting_todo_items_completed_last_week : WithSubject<TodoItemsService2>
	{
		It should_query_for_todo_items_for_the_specified_user_completed_last_week = () =>
			The<IData<IAmbientDbContextLocator>>().WasToldTo(x => x.QueryAsync(new TodoItemsForUserCompletedLastWeek { UserId = Guid.Empty }, CacheOption.Default));

		Because of = () =>
			Subject.GetTodoItemsCompletedLastWeekAsync(Guid.Empty).Wait();

		Establish context = () =>
			DataFactory.SetFactory<IAmbientDbContextLocator>(x => The<IData<IAmbientDbContextLocator>>());
	}

	[Subject(typeof(TodoItemsService2))]
	class When_sending_alerts_for_todo_items_due_tomorrow : WithSubject<TodoItemsService2>
	{
		It should_send_alerts_only_for_todo_items_returned_from_querying_urgent_upcoming_todo_items_for_the_specified_user_which_are_due_tomorrow_or_earlier = () =>
		{
#pragma warning disable 4014
			todoItem1.WasToldTo(x => x.SendAlertAsync(The<IUserAlertService>()));
			todoItem2.WasToldTo(x => x.SendAlertAsync(The<IUserAlertService>()));
			todoItem3.WasNotToldTo(x => x.SendAlertAsync(Param.IsAny<IUserAlertService>()));
#pragma warning restore 4014
		};

		It should_save_changes = () =>
			The<IDbContextScope>().WasToldTo(x => x.SaveChangesAsync());

		Because of = () =>
			Subject.SendAlertsForTodoItemsDueTomorrowAsync(Guid.Empty).Wait();

		Establish context = () =>
		{
			DataFactory.SetFactory<IAmbientDbContextLocator>(x => The<IData<IAmbientDbContextLocator>>());
			todoItem1 = An<TodoItem>();
			todoItem1.WhenToldTo(x => x.DueDate).Return(DateTime.Today.AddDays(1));
			todoItem2 = An<TodoItem>();
			todoItem2.WhenToldTo(x => x.DueDate).Return(DateTime.Today.AddDays(2));
			todoItem3 = An<TodoItem>();
			todoItem3.WhenToldTo(x => x.DueDate).Return(DateTime.Today.AddDays(3));
			The<IData<IAmbientDbContextLocator>>()
				.WhenToldTo(x => x.QueryAsync(new UpcomingTodoItemsWithPriorityForUser { UserId = Guid.Empty, Priority = Priority.Urgent }, CacheOption.Refresh))
				.Return(Task.FromResult(new[] { todoItem1, todoItem2, todoItem3 }));
		};

		static TodoItem todoItem1, todoItem2, todoItem3;
	}
}
