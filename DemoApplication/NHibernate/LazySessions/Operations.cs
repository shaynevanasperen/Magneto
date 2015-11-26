using System;
using System.Linq;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.MultipleDatabases;
using NHibernate.Linq;

namespace DemoApplication.NHibernate.LazySessions
{
	class AllUsers : DataQuery<IUsersContext, User[]>
	{
		public override User[] Execute(IUsersContext context)
		{
			return context.Session.Query<User>().ToArray();
		}
	}

	class AllTodoItemsForUser : DataQuery<ITodoItemsContext, TodoItem[]>
	{
		public override TodoItem[] Execute(ITodoItemsContext context)
		{
			return context.Session.Query<TodoItem>().Where(x => x.UserId == UserId).ToArray();
		}

		public Guid UserId { get; set; }
	}

	class TodoItemsForUserDueBy : DataQuery<ITodoItemsContext, TodoItem[]>
	{
		public override TodoItem[] Execute(ITodoItemsContext context)
		{
			return context.Session.Query<TodoItem>().Where(x => x.UserId == UserId && x.DueDate <= DueDate).ToArray();
		}

		public Guid UserId { get; set; }
		public DateTime DueDate { get; set; }
	}

	class TodoItemsForUserCompletedLastWeek : CachedDataQuery<ITodoItemsContext, TodoItem[]>
	{
		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			cacheInfo.VaryBy = UserId;
			cacheInfo.AbsoluteDuration = DateTime.Today.AddDays(1) - DateTime.Now;
		}

		protected override TodoItem[] Query(ITodoItemsContext context)
		{
			var lastWeekStart = DateTime.Today.AddDays(-7);
			while (lastWeekStart.DayOfWeek != DayOfWeek.Monday)
				lastWeekStart = lastWeekStart.AddDays(-1);
			return context.Session.Query<TodoItem>().Where(x => x.UserId == UserId && x.DateCompleted >= lastWeekStart && x.DateCompleted < lastWeekStart.AddDays(7)).ToArray();
		}

		public Guid UserId { get; set; }
	}

	class UpcomingTodoItemsWithPriorityForUser : TransformedCachedDataQuery<ITodoItemsContext, TodoItem[], TodoItem[]>
	{
		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			cacheInfo.VaryBy = UserId;
			cacheInfo.AbsoluteDuration = TimeSpan.FromSeconds(10);
		}

		protected override TodoItem[] Query(ITodoItemsContext context)
		{
			return context.Session.Query<TodoItem>().Where(x => x.UserId == UserId && !x.DateCompleted.HasValue && x.DueDate >= DateTime.Now).ToArray();
		}

		protected override TodoItem[] TransformCachedResult(TodoItem[] cachedResult)
		{
			return cachedResult.Where(x => x.Priority == Priority).ToArray();
		}

		public Guid UserId { get; set; }
		public Priority Priority { get; set; }
	}

	class CompleteTodoItem : DataCommand<ITodoItemsContext, bool>
	{
		public override bool Execute(ITodoItemsContext context)
		{
			return context.Session
				.CreateQuery("update TodoItem t set t.DateCompleted = :dateCompleted where t.Id = :id and t.DateCompleted = null")
				.SetInt32("id", Id)
				.SetDateTime("dateCompleted", DateTime.Now)
				.ExecuteUpdate() == 1;
		}

		public int Id { get; set; }
	}
}
