using System;
using System.Threading.Tasks;

namespace Magneto
{
	/// <summary>
	/// If using an IoC container, it is highly recommended that this be registered as a scoped service
	/// so that the injected <see cref="IServiceProvider"/> is scoped appropriately.
	/// </summary>
	public class Dispatcher : IDispatcher
	{
		public Dispatcher(IServiceProvider serviceProvider, IInvoker invoker)
		{
			ServiceProvider = serviceProvider;
			Invoker = invoker;
		}

		protected IServiceProvider ServiceProvider { get; }

		protected IInvoker Invoker { get; }

		protected virtual TContext GetContext<TContext>() =>
			(TContext)ServiceProvider.GetService(typeof(TContext));

		/// <inheritdoc cref="ISyncQueryDispatcher.Query{TContext,TResult}"/>
		public virtual TResult Query<TContext, TResult>(ISyncQuery<TContext, TResult> query) =>
			Invoker.Query(query, GetContext<TContext>());

		/// <inheritdoc cref="IAsyncQueryDispatcher.QueryAsync{TContext,TResult}"/>
		public virtual Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query) =>
			Invoker.QueryAsync(query, GetContext<TContext>());

		/// <inheritdoc cref="ISyncQueryDispatcher.Query{TContext,TCacheEntryOptions,TResult}"/>
		public virtual TResult Query<TContext, TCacheEntryOptions, TResult>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption = CacheOption.Default) =>
			Invoker.Query(query, GetContext<TContext>(), cacheOption);

		/// <inheritdoc cref="IAsyncQueryDispatcher.QueryAsync{TContext,TCacheEntryOptions,TResult}"/>
		public virtual Task<TResult> QueryAsync<TContext, TCacheEntryOptions, TResult>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption = CacheOption.Default) =>
			Invoker.QueryAsync(query, GetContext<TContext>(), cacheOption);

		/// <inheritdoc cref="ISyncCacheManager.EvictCachedResult{TCacheEntryOptions}"/>
		public virtual void EvictCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> query) =>
			Invoker.EvictCachedResult(query);

		/// <inheritdoc cref="IAsyncCacheManager.EvictCachedResultAsync{TCacheEntryOptions}"/>
		public virtual Task EvictCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> query) =>
			Invoker.EvictCachedResultAsync(query);

		/// <inheritdoc cref="ISyncCacheManager.UpdateCachedResult{TCacheEntryOptions}"/>
		public virtual void UpdateCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> executedQuery) =>
			Invoker.UpdateCachedResult(executedQuery);

		/// <inheritdoc cref="IAsyncCacheManager.UpdateCachedResultAsync{TCacheEntryOptions}"/>
		public virtual Task UpdateCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> executedQuery) =>
			Invoker.UpdateCachedResultAsync(executedQuery);

		/// <inheritdoc cref="ISyncCommandDispatcher.Command{TContext}"/>
		public virtual void Command<TContext>(ISyncCommand<TContext> command) =>
			Invoker.Command(command, GetContext<TContext>());

		/// <inheritdoc cref="IAsyncCommandDispatcher.CommandAsync{TContext}"/>
		public virtual Task CommandAsync<TContext>(IAsyncCommand<TContext> command) =>
			Invoker.CommandAsync(command, GetContext<TContext>());

		/// <inheritdoc cref="ISyncCommandDispatcher.Command{TContext,TResult}"/>
		public virtual TResult Command<TContext, TResult>(ISyncCommand<TContext, TResult> command) =>
			Invoker.Command(command, GetContext<TContext>());

		/// <inheritdoc cref="IAsyncCommandDispatcher.CommandAsync{TContext,TResult}"/>
		public virtual Task<TResult> CommandAsync<TContext, TResult>(IAsyncCommand<TContext, TResult> command) =>
			Invoker.CommandAsync(command, GetContext<TContext>());
	}
}
