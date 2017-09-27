using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Magneto.Configuration;
using Magneto.Core;

namespace Magneto
{
	/// <summary>
	/// If using an IoC container, it is highly recommended that this be registered as a scoped service
	/// so that the injected <see cref="IServiceProvider"/> is scoped appropriately.
	/// </summary>
	public class Invoker : IInvoker
	{
		readonly ConcurrentDictionary<Type, object> _nullQueryCaches = new ConcurrentDictionary<Type, object>();

		public Invoker(IServiceProvider serviceProvider = null, IDecorator decorator = null)
		{
			ServiceProvider = serviceProvider;
			Decorator = decorator ?? NullDecorator.Instance;
		}

		protected IServiceProvider ServiceProvider { get; }

		protected IDecorator Decorator { get; }

		protected virtual IQueryCache<TCacheEntryOptions> GetQueryCache<TCacheEntryOptions>()
		{
			return (IQueryCache<TCacheEntryOptions>)(ServiceProvider?.GetService(typeof(IQueryCache<TCacheEntryOptions>)) ?? _nullQueryCaches.GetOrAdd(typeof(TCacheEntryOptions), x => new NullQueryCache<TCacheEntryOptions>()));
		}

		protected virtual ISyncQueryCache<TCacheEntryOptions> GetSyncQueryCache<TCacheEntryOptions>() => GetQueryCache<TCacheEntryOptions>();

		protected virtual IAsyncQueryCache<TCacheEntryOptions> GetAsyncQueryCache<TCacheEntryOptions>() => GetQueryCache<TCacheEntryOptions>();

		public virtual TResult Query<TContext, TResult>(ISyncQuery<TContext, TResult> query, TContext context)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(query, () => query.Execute(context));
		}

		public virtual Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query, TContext context)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(query, () => query.ExecuteAsync(context));
		}

		public virtual TResult Query<TContext, TCacheEntryOptions, TResult>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return query.Execute(context, Decorator, GetSyncQueryCache<TCacheEntryOptions>(), cacheOption);
		}

		public virtual Task<TResult> QueryAsync<TContext, TCacheEntryOptions, TResult>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return query.ExecuteAsync(context, Decorator, GetAsyncQueryCache<TCacheEntryOptions>(), cacheOption);
		}

		public virtual void EvictCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> query)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));

			query.EvictCachedResult(GetSyncQueryCache<TCacheEntryOptions>());
		}

		public virtual Task EvictCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> query)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));

			return query.EvictCachedResultAsync(GetAsyncQueryCache<TCacheEntryOptions>());
		}

		public virtual void UpdateCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> executedQuery)
		{
			if (executedQuery == null) throw new ArgumentNullException(nameof(executedQuery));

			executedQuery.UpdateCachedResult(GetSyncQueryCache<TCacheEntryOptions>());
		}

		public virtual Task UpdateCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> executedQuery)
		{
			if (executedQuery == null) throw new ArgumentNullException(nameof(executedQuery));

			return executedQuery.UpdateCachedResultAsync(GetAsyncQueryCache<TCacheEntryOptions>());
		}

		public virtual void Command<TContext>(ISyncCommand<TContext> command, TContext context)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			Decorator.Decorate(command, () => command.Execute(context));
		}

		public virtual Task CommandAsync<TContext>(IAsyncCommand<TContext> command, TContext context)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(command, () => command.ExecuteAsync(context));
		}

		public virtual TResult Command<TContext, TResult>(ISyncCommand<TContext, TResult> command, TContext context)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(command, () => command.Execute(context));
		}

		public virtual Task<TResult> CommandAsync<TContext, TResult>(IAsyncCommand<TContext, TResult> command, TContext context)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(command, () => command.ExecuteAsync(context));
		}
	}
}