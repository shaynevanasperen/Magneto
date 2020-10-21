using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Magneto.Configuration;
using Magneto.Core;

namespace Magneto
{
	/// <summary>
	/// A lower level abstraction for invoking queries and commands by passing the context manually. If an <see cref="IServiceProvider"/> is provided,
	/// it will be used to obtain instances of the various cache stores required by any cached queries. If the <see cref="IServiceProvider"/> is not
	/// able to provide the required <see cref="ISyncCacheStore{TCacheEntryOptions}"/> or <see cref="IAsyncCacheStore{TCacheEntryOptions}"/> for a
	/// given cached query, or if the <see cref="IServiceProvider"/> wasn't provided, caching functionality is disabled for that cached query.
	/// </summary>
	public class Mediary : IMediary
	{
		static readonly ConcurrentDictionary<Type, object> NullCacheStores = new ConcurrentDictionary<Type, object>();

		/// <summary>
		/// Creates a new instance of <see cref="Mediary"/>. The contained <see cref="IDecorator"/> is initialized from obtaining an instance of
		/// <see cref="IDecorator"/> from the given <see cref="IServiceProvider"/>, or a new instance of <see cref="NullDecorator"/> if the
		/// <see cref="IServiceProvider"/> was not provided or couldn't provide it.
		/// </summary>
		/// <param name="serviceProvider">Used for obtaining instances of cache store objects with which cached queries are invoked.</param>
		/// <param name="decorator">Used for decorating invocations in order to apply cross-cutting concerns.</param>
		public Mediary(IServiceProvider serviceProvider = null, IDecorator decorator = null)
		{
			ServiceProvider = serviceProvider;
			Decorator = decorator ?? serviceProvider?.GetService<IDecorator>() ?? NullDecorator.Instance;
		}

		/// <summary>
		/// Exposes the <see cref="IServiceProvider"/> provided in the constructor.
		/// </summary>
		protected IServiceProvider ServiceProvider { get; }

		/// <summary>
		/// Exposes the <see cref="IDecorator"/> provided in the constructor or from the <see cref="IServiceProvider"/>, or a <see cref="NullDecorator"/> if neither exist.
		/// </summary>
		protected IDecorator Decorator { get; }

		/// <summary>
		/// Gets an instance of <see cref="ICacheStore{TCacheEntryOptions}"/> from the <see cref="ServiceProvider"/>,
		/// or a <see cref="NullCacheStore{TCacheEntryOptions}"/> if the <see cref="ServiceProvider"/> doesn't have one.
		/// </summary>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options closed over by the requested <see cref="ICacheStore{TCacheEntryOptions}"/></typeparam>
		/// <returns>An instance of <see cref="ICacheStore{TCacheEntryOptions}"/>.</returns>
		protected virtual ICacheStore<TCacheEntryOptions> GetCacheStore<TCacheEntryOptions>() =>
			ServiceProvider?.GetService<ICacheStore<TCacheEntryOptions>>() ?? NullCacheStores.GetOrAdd<ICacheStore<TCacheEntryOptions>>(() => new NullCacheStore<TCacheEntryOptions>());

		/// <summary>
		/// Gets an instance of <see cref="ISyncCacheStore{TCacheEntryOptions}"/> from the <see cref="ServiceProvider"/>,
		/// or a <see cref="NullCacheStore{TCacheEntryOptions}"/> if the <see cref="ServiceProvider"/> doesn't have one.
		/// </summary>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options closed over by the requested <see cref="ISyncCacheStore{TCacheEntryOptions}"/></typeparam>
		/// <returns>An instance of <see cref="ISyncCacheStore{TCacheEntryOptions}"/>.</returns>
		protected virtual ISyncCacheStore<TCacheEntryOptions> GetSyncCacheStore<TCacheEntryOptions>() => GetCacheStore<TCacheEntryOptions>();

		/// <summary>
		/// Gets an instance of <see cref="IAsyncCacheStore{TCacheEntryOptions}"/> from the <see cref="ServiceProvider"/>,
		/// or a <see cref="NullCacheStore{TCacheEntryOptions}"/> if the <see cref="ServiceProvider"/> doesn't have one.
		/// </summary>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options closed over by the requested <see cref="IAsyncCacheStore{TCacheEntryOptions}"/></typeparam>
		/// <returns>An instance of <see cref="IAsyncCacheStore{TCacheEntryOptions}"/>.</returns>
		protected virtual IAsyncCacheStore<TCacheEntryOptions> GetAsyncCacheStore<TCacheEntryOptions>() => GetCacheStore<TCacheEntryOptions>();

		/// <summary>
		/// Creates a name from the given <paramref name="instance"/> and <paramref name="methodName"/>.
		/// </summary>
		/// <param name="instance">The object from which to retrieve the full type name.</param>
		/// <param name="methodName">The name of a method on the given object.</param>
		/// <returns>A string made up of the full type name and the given method name, joined by a dot.</returns>
		protected virtual string GetOperationName(object instance, string methodName)
		{
			if (instance == null) throw new ArgumentNullException(nameof(instance));

			return $"{instance.GetType().FullName}.{methodName}";
		}

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

			return Decorator.Decorate(GetOperationName(query, nameof(query.Execute)), () => query.Execute(context, cancellationToken));
		}

		/// <inheritdoc cref="ISyncQueryMediary.Query{TContext,TCacheEntryOptions,TResult}"/>
		public virtual TResult Query<TContext, TCacheEntryOptions, TResult>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(GetOperationName(query, nameof(query.Execute)), () => query.Execute(context, GetSyncCacheStore<TCacheEntryOptions>(), cacheOption));
		}

		/// <inheritdoc cref="IAsyncQueryMediary.QueryAsync{TContext,TCacheEntryOptions,TResult}"/>
		public virtual Task<TResult> QueryAsync<TContext, TCacheEntryOptions, TResult>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption, CancellationToken cancellationToken = default)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(GetOperationName(query, nameof(query.Execute)), () => query.Execute(context, GetAsyncCacheStore<TCacheEntryOptions>(), cacheOption, cancellationToken));
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

			return Decorator.Decorate(GetOperationName(query, nameof(query.EvictCachedResult)), () => query.EvictCachedResult(GetAsyncCacheStore<TCacheEntryOptions>(), cancellationToken));
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

			return Decorator.Decorate(GetOperationName(executedQuery, nameof(executedQuery.UpdateCachedResult)), () => executedQuery.UpdateCachedResult(GetAsyncCacheStore<TCacheEntryOptions>(), cancellationToken));
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

			return Decorator.Decorate(GetOperationName(command, nameof(command.Execute)), () => command.Execute(context, cancellationToken));
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

			return Decorator.Decorate(GetOperationName(command, nameof(command.Execute)), () => command.Execute(context, cancellationToken));
		}
	}
}
