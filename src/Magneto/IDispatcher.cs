using System.Threading.Tasks;

namespace Magneto
{
	public interface IDispatcher : ISyncDispatcher, IAsyncDispatcher, IQueryDispatcher, ICommandDispatcher, ICacheManager { }

	public interface IQueryDispatcher : ISyncQueryDispatcher, IAsyncQueryDispatcher { }

	public interface ICommandDispatcher : ISyncCommandDispatcher, IAsyncCommandDispatcher { }

	public interface ISyncDispatcher : ISyncQueryDispatcher, ISyncCommandDispatcher, ISyncCacheManager { }

	public interface IAsyncDispatcher : IAsyncQueryDispatcher, IAsyncCommandDispatcher, IAsyncCacheManager { }

	public interface ISyncQueryDispatcher
	{
		TResult Query<TContext, TResult>(ISyncQuery<TContext, TResult> query);
		TResult Query<TContext, TResult, TCacheEntryOptions>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption = CacheOption.Default);
	}

	public interface IAsyncQueryDispatcher
	{
		Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query);
		Task<TResult> QueryAsync<TContext, TResult, TCacheEntryOptions>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption = CacheOption.Default);
	}

	public interface ISyncCommandDispatcher
	{
		void Command<TContext>(ISyncCommand<TContext> command);
		TResult Command<TContext, TResult>(ISyncCommand<TContext, TResult> command);
	}

	public interface IAsyncCommandDispatcher
	{
		Task CommandAsync<TContext>(IAsyncCommand<TContext> command);
		Task<TResult> CommandAsync<TContext, TResult>(IAsyncCommand<TContext, TResult> command);
	}
}
