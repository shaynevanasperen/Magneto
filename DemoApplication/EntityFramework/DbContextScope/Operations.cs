using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Data.Operations;
using DemoApplication.DomainModel;
using DemoApplication.DomainModel.MultipleDatabases;
using Mehdime.Entity;

namespace DemoApplication.EntityFramework.DbContextScope
{
	class AllUsers : AsyncDataQuery<IAmbientDbContextLocator, User[]>
	{
		public override Task<User[]> ExecuteAsync(IAmbientDbContextLocator context)
		{
			return context.Get<UsersContext>().Users.ToArrayAsync();
		}
	}

	class AllTodoItemsForUser : AsyncDataQuery<IAmbientDbContextLocator, TodoItem[]>
	{
		public override Task<TodoItem[]> ExecuteAsync(IAmbientDbContextLocator context)
		{
			return context.Get<TodoItemsContext>().TodoItems.Where(x => x.UserId == UserId).ToArrayAsync();
		}

		public Guid UserId { get; set; }
	}

	class TodoItemsForUserDueBy : AsyncDataQuery<IAmbientDbContextLocator, TodoItem[]>
	{
		public override Task<TodoItem[]> ExecuteAsync(IAmbientDbContextLocator context)
		{
			return context.Get<TodoItemsContext>().TodoItems.Where(x => x.UserId == UserId && x.DueDate <= DueDate).ToArrayAsync();
		}

		public Guid UserId { get; set; }
		public DateTime DueDate { get; set; }
	}

	class TodoItemsForUserCompletedLastWeek : AsyncCachedDataQuery<IAmbientDbContextLocator, TodoItem[]>
	{
		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			cacheInfo.VaryBy = UserId;
			cacheInfo.AbsoluteDuration = DateTime.Today.AddDays(1) - DateTime.Now;
		}

		protected override Task<TodoItem[]> QueryAsync(IAmbientDbContextLocator context)
		{
			var lastWeekStart = DateTime.Today.AddDays(-7);
			while (lastWeekStart.DayOfWeek != DayOfWeek.Monday)
				lastWeekStart = lastWeekStart.AddDays(-1);
			var lastWeekEnd = lastWeekStart.AddDays(7);
			return context.Get<TodoItemsContext>().TodoItems.Where(x => x.UserId == UserId && x.DateCompleted >= lastWeekStart && x.DateCompleted < lastWeekEnd).ToArrayAsync();
		}

		public Guid UserId { get; set; }
	}

	class UpcomingTodoItemsWithPriorityForUser : AsyncTransformedCachedDataQuery<IAmbientDbContextLocator, TodoItem[], TodoItem[]>
	{
		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			cacheInfo.VaryBy = UserId;
			cacheInfo.AbsoluteDuration = TimeSpan.FromSeconds(10);
		}

		protected override Task<TodoItem[]> QueryAsync(IAmbientDbContextLocator context)
		{
			return context.Get<TodoItemsContext>().TodoItems.Where(x => x.UserId == UserId && x.DueDate >= DateTime.Now).ToArrayAsync();
		}

		protected override Task<TodoItem[]> TransformCachedResultAsync(TodoItem[] cachedResult)
		{
			return Task.FromResult(cachedResult.Where(x => x.Priority == Priority).ToArray());
		}

		public Guid UserId { get; set; }
		public Priority Priority { get; set; }
	}

	class CompleteTodoItem : AsyncDataCommand<IAmbientDbContextLocator, bool>
	{
		public async override Task<bool> ExecuteAsync(IAmbientDbContextLocator context)
		{
			return await context.Get<TodoItemsContext>().Database
				.ExecuteSqlCommandAsync("UPDATE dbo.TodoItem SET DateCompleted = @p0 WHERE Id = @p1 and DateCompleted IS NULL", DateTime.Now, Id)
				.ConfigureAwait(false) == 1;
		}

		public int Id { get; set; }
	}
}
