using System;
using System.Linq;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;
using NHibernate;
using NHibernate.Linq;

namespace DemoApplication.NHibernate.Session
{
	class AllUsers : DataQuery<ISession, User[]>
	{
		public override User[] Execute(ISession context)
		{
			return context.Query<User>().ToArray();
		}
	}

	class AllTodoItemsForUser : DataQuery<ISession, TodoItem[]>
	{
		public override TodoItem[] Execute(ISession context)
		{
			return context.Query<TodoItem>().Where(x => x.User.Id == UserId).ToArray();
		}

		public int UserId { get; set; }
	}

	class TodoItemsForUserDueBy : DataQuery<ISession, TodoItem[]>
	{
		public override TodoItem[] Execute(ISession context)
		{
			return context.Query<TodoItem>().Where(x => x.User.Id == UserId && x.DueDate <= DueDate).ToArray();
		}

		public int UserId { get; set; }
		public DateTime DueDate { get; set; }
	}

	class TodoItemsForUserCompletedLastWeek : CachedDataQuery<ISession, TodoItem[]>
	{
		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			cacheInfo.VaryBy = UserId;
			cacheInfo.CacheItemPolicy.AbsoluteExpiration = new DateTimeOffset(DateTime.Today.AddDays(1));
		}

		protected override TodoItem[] Query(ISession context)
		{
			var lastWeekStart = DateTime.Today.AddDays(-7);
			while (lastWeekStart.DayOfWeek != DayOfWeek.Monday)
				lastWeekStart = lastWeekStart.AddDays(-1);
			return context.Query<TodoItem>().Where(x => x.User.Id == UserId && x.DateCompleted >= lastWeekStart && x.DateCompleted < lastWeekStart.AddDays(7)).ToArray();
		}

		public int UserId { get; set; }
	}

	class UpcomingTodoItemsWithPriorityForUser : TransformedCachedDataQuery<ISession, TodoItem[], TodoItem[]>
	{
		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			cacheInfo.VaryBy = UserId;
			cacheInfo.CacheItemPolicy.SlidingExpiration = TimeSpan.FromSeconds(10);
		}

		protected override TodoItem[] Query(ISession context)
		{
			return context.Query<TodoItem>().Where(x => x.User.Id == UserId && !x.DateCompleted.HasValue && x.DueDate >= DateTime.Now).ToArray();
		}

		protected override TodoItem[] TransformCachedResult(TodoItem[] cachedResult)
		{
			return cachedResult.Where(x => x.Priority == Priority).ToArray();
		}

		public int UserId { get; set; }
		public Priority Priority { get; set; }
	}

	class CompleteTodoItem : DataCommand<ISession, bool>
	{
		public override bool Execute(ISession context)
		{
			return context
				.CreateQuery("update TodoItem t set t.DateCompleted = :dateCompleted where t.Id = :id and t.DateCompleted = null")
				.SetInt32("id", Id)
				.SetDateTime("dateCompleted", DateTime.Now)
				.ExecuteUpdate() == 1;
		}

		public int Id { get; set; }
	}
}
