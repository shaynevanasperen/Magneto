using System;
using System.Linq;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;
using NHibernate.Linq;
using NHibernate.Sessions;

namespace DemoApplication.NHibernate.LazySession
{
	class AllUsers : DataQuery<ILazySession, User[]>
	{
		public override User[] Execute(ILazySession context)
		{
			return context.Current.Query<User>().ToArray();
		}
	}

	class AllTodoItemsForUser : DataQuery<ILazySession, TodoItem[]>
	{
		public override TodoItem[] Execute(ILazySession context)
		{
			return context.Current.Query<TodoItem>().Where(x => x.User.Id == UserId).ToArray();
		}

		public int UserId { get; set; }
	}

	class TodoItemsForUserDueBy : DataQuery<ILazySession, TodoItem[]>
	{
		public override TodoItem[] Execute(ILazySession context)
		{
			return context.Current.Query<TodoItem>().Where(x => x.User.Id == UserId && x.DueDate <= DueDate).ToArray();
		}

		public int UserId { get; set; }
		public DateTime DueDate { get; set; }
	}

	class TodoItemsForUserCompletedLastWeek : CachedDataQuery<ILazySession, TodoItem[]>
	{
		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			cacheInfo.VaryBy = UserId;
			cacheInfo.AbsoluteDuration = DateTime.Today.AddDays(1) - DateTime.Now;
		}

		protected override TodoItem[] Query(ILazySession context)
		{
			var lastWeekStart = DateTime.Today.AddDays(-7);
			while (lastWeekStart.DayOfWeek != DayOfWeek.Monday)
				lastWeekStart = lastWeekStart.AddDays(-1);
			return context.Current.Query<TodoItem>().Where(x => x.User.Id == UserId && x.DateCompleted >= lastWeekStart && x.DateCompleted < lastWeekStart.AddDays(7)).ToArray();
		}

		public int UserId { get; set; }
	}

	class UpcomingTodoItemsWithPriorityForUser : TransformedCachedDataQuery<ILazySession, TodoItem[], TodoItem[]>
	{
		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			cacheInfo.VaryBy = UserId;
			cacheInfo.AbsoluteDuration = TimeSpan.FromSeconds(10);
		}

		protected override TodoItem[] Query(ILazySession context)
		{
			return context.Current.Query<TodoItem>().Where(x => x.User.Id == UserId && !x.DateCompleted.HasValue && x.DueDate >= DateTime.Now).ToArray();
		}

		protected override TodoItem[] TransformCachedResult(TodoItem[] cachedResult)
		{
			return cachedResult.Where(x => x.Priority == Priority).ToArray();
		}

		public int UserId { get; set; }
		public Priority Priority { get; set; }
	}

	class CompleteTodoItem : DataCommand<ILazySession, bool>
	{
		public override bool Execute(ILazySession context)
		{
			return context.Current
				.CreateQuery("update TodoItem t set t.DateCompleted = :dateCompleted where t.Id = :id and t.DateCompleted = null")
				.SetInt32("id", Id)
				.SetDateTime("dateCompleted", DateTime.Now)
				.ExecuteUpdate() == 1;
		}

		public int Id { get; set; }
	}
}
