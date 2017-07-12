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

	public interface IInvoker<out TContext> : ISyncInvoker<TContext>, IAsyncInvoker<TContext>, IQueryInvoker<TContext>, ICommandInvoker<TContext>, ICacheManager where TContext : class { }

	public interface IQueryInvoker<out TContext> : ISyncQueryInvoker<TContext>, IAsyncQueryInvoker<TContext> where TContext : class { }

	public interface ICommandInvoker<out TContext> : ISyncCommandInvoker<TContext>, IAsyncCommandInvoker<TContext> where TContext : class { }

	public interface ISyncInvoker<out TContext> : ISyncQueryInvoker<TContext>, ISyncCommandInvoker<TContext>, ISyncCacheManager where TContext : class { }

	public interface IAsyncInvoker<out TContext> : IAsyncQueryInvoker<TContext>, IAsyncCommandInvoker<TContext>, IAsyncCacheManager where TContext : class { }

	public interface ISyncQueryInvoker<out TContext> : IContextInvoker<TContext> where TContext : class
	{
		TResult Query<TResult>(ISyncQuery<TContext, TResult> query);
		TResult Query<TResult, TCacheEntryOptions>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption = CacheOption.Default);
	}

	public interface IAsyncQueryInvoker<out TContext> : IContextInvoker<TContext> where TContext : class
	{
		Task<TResult> QueryAsync<TResult>(IAsyncQuery<TContext, TResult> query);
		Task<TResult> QueryAsync<TResult, TCacheEntryOptions>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption = CacheOption.Default);
	}

	public interface ISyncCommandInvoker<out TContext> : IContextInvoker<TContext> where TContext : class
	{
		void Command(ISyncCommand<TContext> command);
		TResult Command<TResult>(ISyncCommand<TContext, TResult> command);
	}

	public interface IAsyncCommandInvoker<out TContext> : IContextInvoker<TContext> where TContext : class
	{
		Task CommandAsync(IAsyncCommand<TContext> command);
		Task<TResult> CommandAsync<TResult>(IAsyncCommand<TContext, TResult> command);
	}

	public interface IContextInvoker<out TContext> : IDisposable where TContext : class
	{
		TContext Context { get; }
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
