using System;
using System.Threading.Tasks;

namespace Magneto
{
    public class Dispatcher : IDispatcher
    {
		public Dispatcher(IServiceProvider serviceProvider, IInvoker invoker)
		{
			ServiceProvider = serviceProvider;
			Invoker = invoker;
		}

	    protected IServiceProvider ServiceProvider { get; }

		protected IInvoker Invoker { get; }

		protected virtual TContext GetContext<TContext>()
	    {
		    return (TContext)ServiceProvider.GetService(typeof(TContext));
	    }

		public virtual TResult Query<TContext, TResult>(ISyncQuery<TContext, TResult> query)
		{
			return Invoker.Query(query, GetContext<TContext>());
		}

		public virtual Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query)
	    {
			return Invoker.QueryAsync(query, GetContext<TContext>());
		}

		public virtual TResult Query<TContext, TResult, TCacheEntryOptions>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption = CacheOption.Default)
	    {
			return Invoker.Query(query, GetContext<TContext>(), cacheOption);
		}

		public virtual Task<TResult> QueryAsync<TContext, TResult, TCacheEntryOptions>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption = CacheOption.Default)
	    {
			return Invoker.QueryAsync(query, GetContext<TContext>(), cacheOption);
		}

		public virtual void EvictCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> query)
	    {
		    Invoker.EvictCachedResult(query);
		}

		public virtual Task EvictCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> query)
	    {
			return Invoker.EvictCachedResultAsync(query);
		}

		public virtual void UpdateCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> executedQuery)
	    {
		    Invoker.UpdateCachedResult(executedQuery);
		}

		public virtual Task UpdateCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> executedQuery)
	    {
		    return Invoker.UpdateCachedResultAsync(executedQuery);
		}

		public virtual void Command<TContext>(ISyncCommand<TContext> command)
		{
			Invoker.Command(command, GetContext<TContext>());
		}

		public virtual Task CommandAsync<TContext>(IAsyncCommand<TContext> command)
	    {
		    return Invoker.CommandAsync(command, GetContext<TContext>());
		}

		public virtual TResult Command<TContext, TResult>(ISyncCommand<TContext, TResult> command)
	    {
			return Invoker.Command(command, GetContext<TContext>());
		}

		public virtual Task<TResult> CommandAsync<TContext, TResult>(IAsyncCommand<TContext, TResult> command)
	    {
			return Invoker.CommandAsync(command, GetContext<TContext>());
		}
    }
}
