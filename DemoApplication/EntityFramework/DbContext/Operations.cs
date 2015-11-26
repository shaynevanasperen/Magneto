using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.SingleDatabase;

namespace DemoApplication.EntityFramework.DbContext
{
	class AllUsers : AsyncDataQuery<DemoContext, User[]>
	{
		public override Task<User[]> ExecuteAsync(DemoContext context)
		{
			return context.Users.ToArrayAsync();
		}
	}

	class AllTodoItemsForUser : AsyncDataQuery<DemoContext, TodoItem[]>
	{
		public override Task<TodoItem[]> ExecuteAsync(DemoContext context)
		{
			return context.TodoItems.Where(x => x.User.Id == UserId).ToArrayAsync();
		}

		public int UserId { get; set; }
	}

	class TodoItemsForUserDueBy : AsyncDataQuery<DemoContext, TodoItem[]>
	{
		public override Task<TodoItem[]> ExecuteAsync(DemoContext context)
		{
			return context.TodoItems.Where(x => x.User.Id == UserId && x.DueDate <= DueDate).ToArrayAsync();
		}

		public int UserId { get; set; }
		public DateTime DueDate { get; set; }
	}

	class TodoItemsForUserCompletedLastWeek : AsyncCachedDataQuery<DemoContext, TodoItem[]>
	{
		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			cacheInfo.VaryBy = UserId;
			cacheInfo.AbsoluteDuration = DateTime.Today.AddDays(1) - DateTime.Now;
		}

		protected override Task<TodoItem[]> QueryAsync(DemoContext context)
		{
			var lastWeekStart = DateTime.Today.AddDays(-7);
			while (lastWeekStart.DayOfWeek != DayOfWeek.Monday)
				lastWeekStart = lastWeekStart.AddDays(-1);
			var lastWeekEnd = lastWeekStart.AddDays(7);
			return context.TodoItems.Where(x => x.User.Id == UserId && x.DateCompleted >= lastWeekStart && x.DateCompleted < lastWeekEnd).ToArrayAsync();
		}

		public int UserId { get; set; }
	}

	class UpcomingTodoItemsWithPriorityForUser : AsyncTransformedCachedDataQuery<DemoContext, TodoItem[], TodoItem[]>
	{
		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			cacheInfo.VaryBy = UserId;
			cacheInfo.AbsoluteDuration = TimeSpan.FromSeconds(10);
		}

		protected override Task<TodoItem[]> QueryAsync(DemoContext context)
		{
			return context.TodoItems.Where(x => x.User.Id == UserId && x.DueDate >= DateTime.Now).ToArrayAsync();
		}

		protected override Task<TodoItem[]> TransformCachedResultAsync(TodoItem[] cachedResult)
		{
			return Task.FromResult(cachedResult.Where(x => x.Priority == Priority).ToArray());
		}

		public int UserId { get; set; }
		public Priority Priority { get; set; }
	}

	class CompleteTodoItem : AsyncDataCommand<DemoContext, bool>
	{
		public async override Task<bool> ExecuteAsync(DemoContext context)
		{
			return await context.Database
				.ExecuteSqlCommandAsync("UPDATE dbo.TodoItem SET DateCompleted = @p0 WHERE Id = @p1 and DateCompleted IS NULL", DateTime.Now, Id)
				.ConfigureAwait(false) == 1;
		}

		public int Id { get; set; }
	}
}
