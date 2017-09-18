using System;
using System.Threading.Tasks;

namespace Magneto
{
	public interface IInvoker : ISyncInvoker, IAsyncInvoker, IQueryInvoker, ICommandInvoker, ICacheManager { }

	public interface IQueryInvoker : ISyncQueryInvoker, IAsyncQueryInvoker { }

	public interface ICommandInvoker : ISyncCommandInvoker, IAsyncCommandInvoker { }

	public interface ISyncInvoker : ISyncQueryInvoker, ISyncCommandInvoker, ISyncCacheManager { }

	public interface IAsyncInvoker : IAsyncQueryInvoker, IAsyncCommandInvoker, IAsyncCacheManager { }

	public interface ISyncQueryInvoker
	{
		TResult Query<TContext, TResult>(ISyncQuery<TContext, TResult> query, TContext context);
		TResult Query<TContext, TCacheEntryOptions, TResult>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default);
	}

	public interface IAsyncQueryInvoker
	{
		Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query, TContext context);
		Task<TResult> QueryAsync<TContext, TCacheEntryOptions, TResult>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default);
	}

	public interface ISyncCommandInvoker
	{
		void Command<TContext>(ISyncCommand<TContext> command, TContext context);
		TResult Command<TContext, TResult>(ISyncCommand<TContext, TResult> command, TContext context);
	}

	public interface IAsyncCommandInvoker
	{
		Task CommandAsync<TContext>(IAsyncCommand<TContext> command, TContext context);
		Task<TResult> CommandAsync<TContext, TResult>(IAsyncCommand<TContext, TResult> command, TContext context);
	}

	public interface ICacheManager : ISyncCacheManager, IAsyncCacheManager { }

	public interface ISyncCacheManager
	{
		void EvictCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> query);
		void UpdateCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> executedQuery);
	}

	public interface IAsyncCacheManager
	{
		Task EvictCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> query);
		Task UpdateCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> executedQuery);
	}
}
