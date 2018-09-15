using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Magneto.Configuration;
using Magneto.Core;

namespace Magneto
{
	/// <summary>
	/// If using an IoC container, it's highly recommended that this be registered as a scoped service
	/// so that the injected <see cref="IServiceProvider"/> is scoped appropriately.
	/// </summary>
	public class Mediary : IMediary
	{
		static readonly ConcurrentDictionary<Type, object> NullCacheStores = new ConcurrentDictionary<Type, object>();

		/// <summary>
		/// Creates a new instance of <see cref="Mediary"/>.
		/// </summary>
		/// <param name="serviceProvider">Used for obtaining instances of cache store objects with which cached queries are invoked.</param>
		/// <param name="decorator">Used for decorating invocations in order to apply cross-cutting concerns.</param>
		public Mediary(IServiceProvider serviceProvider = null, IDecorator decorator = null)
		{
			ServiceProvider = serviceProvider;
			Decorator = decorator ?? serviceProvider?.GetService<IDecorator>() ?? NullDecorator.Instance;
		}

		protected IServiceProvider ServiceProvider { get; }

		protected IDecorator Decorator { get; }

		protected virtual ICacheStore<TCacheEntryOptions> GetCacheStore<TCacheEntryOptions>() =>
			ServiceProvider?.GetService<ICacheStore<TCacheEntryOptions>>() ?? NullCacheStores.GetOrAdd<ICacheStore<TCacheEntryOptions>>(() => new NullCacheStore<TCacheEntryOptions>());

		protected virtual ISyncCacheStore<TCacheEntryOptions> GetSyncCacheStore<TCacheEntryOptions>() => GetCacheStore<TCacheEntryOptions>();

		protected virtual IAsyncCacheStore<TCacheEntryOptions> GetAsyncCacheStore<TCacheEntryOptions>() => GetCacheStore<TCacheEntryOptions>();

		protected virtual string GetOperationName(object instance, string methodName) => $"{instance.GetType().FullName}.{methodName}";

		/// <inheritdoc cref="ISyncQueryMediary.Query{TContext,TResult}"/>
		public virtual TResult Query<TContext, TResult>(ISyncQuery<TContext, TResult> query, TContext context)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(GetOperationName(query, nameof(query.Execute)), () => query.Execute(context));
		}

		/// <inheritdoc cref="IAsyncQueryMediary.QueryAsync{TContext,TResult}"/>
		public virtual Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query, TContext context, CancellationToken cancellationToken = default)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(GetOperationName(query, nameof(query.ExecuteAsync)), () => query.ExecuteAsync(context, cancellationToken));
		}

		/// <inheritdoc cref="ISyncQueryMediary.Query{TContext,TCacheEntryOptions,TResult}"/>
		public virtual TResult Query<TContext, TCacheEntryOptions, TResult>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(GetOperationName(query, nameof(query.Execute)), () => query.Execute(context, GetSyncCacheStore<TCacheEntryOptions>(), cacheOption));
		}

		/// <inheritdoc cref="IAsyncQueryMediary.QueryAsync{TContext,TCacheEntryOptions,TResult}"/>
		public virtual Task<TResult> QueryAsync<TContext, TCacheEntryOptions, TResult>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default, CancellationToken cancellationToken = default)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(GetOperationName(query, nameof(query.ExecuteAsync)), () => query.ExecuteAsync(context, GetAsyncCacheStore<TCacheEntryOptions>(), cacheOption, cancellationToken));
		}

		/// <inheritdoc cref="ISyncCacheManager.EvictCachedResult{TCacheEntryOptions}"/>
		public virtual void EvictCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> query)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));

			Decorator.Decorate(GetOperationName(query, nameof(query.EvictCachedResult)), () => query.EvictCachedResult(GetSyncCacheStore<TCacheEntryOptions>()));
		}

		/// <inheritdoc cref="IAsyncCacheManager.EvictCachedResultAsync{TCacheEntryOptions}"/>
		public virtual Task EvictCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> query, CancellationToken cancellationToken = default)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));

			return Decorator.Decorate(GetOperationName(query, nameof(query.EvictCachedResultAsync)), () => query.EvictCachedResultAsync(GetAsyncCacheStore<TCacheEntryOptions>(), cancellationToken));
		}

		/// <inheritdoc cref="ISyncCacheManager.UpdateCachedResult{TCacheEntryOptions}"/>
		public virtual void UpdateCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> executedQuery)
		{
			if (executedQuery == null) throw new ArgumentNullException(nameof(executedQuery));

			Decorator.Decorate(GetOperationName(executedQuery, nameof(executedQuery.UpdateCachedResult)), () => executedQuery.UpdateCachedResult(GetSyncCacheStore<TCacheEntryOptions>()));
		}

		/// <inheritdoc cref="IAsyncCacheManager.UpdateCachedResultAsync{TCacheEntryOptions}"/>
		public virtual Task UpdateCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> executedQuery, CancellationToken cancellationToken = default)
		{
			if (executedQuery == null) throw new ArgumentNullException(nameof(executedQuery));

			return Decorator.Decorate(GetOperationName(executedQuery, nameof(executedQuery.UpdateCachedResultAsync)), () => executedQuery.UpdateCachedResultAsync(GetAsyncCacheStore<TCacheEntryOptions>(), cancellationToken));
		}

		/// <inheritdoc cref="ISyncCommandMediary.Command{TContext}"/>
		public virtual void Command<TContext>(ISyncCommand<TContext> command, TContext context)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			Decorator.Decorate(GetOperationName(command, nameof(command.Execute)), () => command.Execute(context));
		}

		/// <inheritdoc cref="IAsyncCommandMediary.CommandAsync{TContext}"/>
		public virtual Task CommandAsync<TContext>(IAsyncCommand<TContext> command, TContext context, CancellationToken cancellationToken = default)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(GetOperationName(command, nameof(command.ExecuteAsync)), () => command.ExecuteAsync(context, cancellationToken));
		}

		/// <inheritdoc cref="ISyncCommandMediary.Command{TContext,TResult}"/>
		public virtual TResult Command<TContext, TResult>(ISyncCommand<TContext, TResult> command, TContext context)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(GetOperationName(command, nameof(command.Execute)), () => command.Execute(context));
		}

		/// <inheritdoc cref="IAsyncCommandMediary.CommandAsync{TContext,TResult}"/>
		public virtual Task<TResult> CommandAsync<TContext, TResult>(IAsyncCommand<TContext, TResult> command, TContext context, CancellationToken cancellationToken = default)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(GetOperationName(command, nameof(command.ExecuteAsync)), () => command.ExecuteAsync(context, cancellationToken));
		}
	}
}