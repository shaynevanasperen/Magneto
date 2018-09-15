using System;
using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto.Core
{
	/// <inheritdoc cref="ISyncCachedQuery{TCacheEntryOptions}"/>
	public abstract class SyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult> : CachedQuery<TContext, TCacheEntryOptions, TCachedResult>, ISyncCachedQuery<TCacheEntryOptions>
	{
		/// <inheritdoc cref="ISyncQuery{TContext,TResult}.Execute"/>
		protected abstract TCachedResult Query(TContext context);

		/// <inheritdoc cref="ISyncCachedQuery{TContext,TCacheEntryOptions,TResult}.Execute"/>
		protected virtual TCachedResult GetCachedResult(TContext context, ISyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption = CacheOption.Default)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (cacheStore == null) throw new ArgumentNullException(nameof(cacheStore));

			State.Inject(context);

			if (cacheOption == CacheOption.Default)
			{
				var cacheEntry = cacheStore.Get<TCachedResult>(State.CacheKey);
				if (cacheEntry != null)
					return State.CachedResult = cacheEntry.Value;
			}

			State.CachedResult = Query(context);
			cacheStore.Set(State.CacheKey, State.CachedResult.ToCacheEntry(), State.GetCacheEntryOptions());
			return State.CachedResult;
		}

		/// <inheritdoc cref="ISyncCachedQuery{TCacheEntryOptions}.EvictCachedResult"/>
		public virtual void EvictCachedResult(ISyncCacheStore<TCacheEntryOptions> cacheStore)
		{
			if (cacheStore == null) throw new ArgumentNullException(nameof(cacheStore));

			cacheStore.Remove(State.CacheKey);
		}

		/// <inheritdoc cref="ISyncCachedQuery{TCacheEntryOptions}.UpdateCachedResult"/>
		public virtual void UpdateCachedResult(ISyncCacheStore<TCacheEntryOptions> cacheStore)
		{
			if (cacheStore == null) throw new ArgumentNullException(nameof(cacheStore));

			cacheStore.Set(State.CacheKey, State.CachedResult.ToCacheEntry(), State.GetCacheEntryOptions());
		}
	}

	/// <inheritdoc cref="IAsyncCachedQuery{TCacheEntryOptions}"/>
	public abstract class AsyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult> : CachedQuery<TContext, TCacheEntryOptions, TCachedResult>, IAsyncCachedQuery<TCacheEntryOptions>
	{
		/// <inheritdoc cref="IAsyncQuery{TContext,TResult}.ExecuteAsync"/>
		protected abstract Task<TCachedResult> QueryAsync(TContext context);

		/// <inheritdoc cref="IAsyncCachedQuery{TContext,TCacheEntryOptions,TResult}.ExecuteAsync"/>
		protected virtual async Task<TCachedResult> GetCachedResultAsync(TContext context, IAsyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption = CacheOption.Default)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (cacheStore == null) throw new ArgumentNullException(nameof(cacheStore));

			State.Inject(context);

			if (cacheOption == CacheOption.Default)
			{
				var cacheEntry = await cacheStore.GetAsync<TCachedResult>(State.CacheKey).ConfigureAwait(false);
				if (cacheEntry != null)
					return State.CachedResult = cacheEntry.Value;
			}

			State.CachedResult = await QueryAsync(context).ConfigureAwait(false);
			await cacheStore.SetAsync(State.CacheKey, State.CachedResult.ToCacheEntry(), State.GetCacheEntryOptions()).ConfigureAwait(false);
			return State.CachedResult;
		}

		/// <inheritdoc cref="IAsyncCachedQuery{TCacheEntryOptions}.EvictCachedResultAsync"/>
		public virtual Task EvictCachedResultAsync(IAsyncCacheStore<TCacheEntryOptions> cacheStore)
		{
			if (cacheStore == null) throw new ArgumentNullException(nameof(cacheStore));

			return cacheStore.RemoveAsync(State.CacheKey);
		}

		/// <inheritdoc cref="IAsyncCachedQuery{TCacheEntryOptions}.UpdateCachedResultAsync"/>
		public virtual Task UpdateCachedResultAsync(IAsyncCacheStore<TCacheEntryOptions> cacheStore)
		{
			if (cacheStore == null) throw new ArgumentNullException(nameof(cacheStore));

			return cacheStore.SetAsync(State.CacheKey, State.CachedResult.ToCacheEntry(), State.GetCacheEntryOptions());
		}
	}

	/// <summary>
	/// A root base class for building cached queries.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the query.</typeparam>
	/// <typeparam name="TCachedResult">The type of the query result.</typeparam>
	public abstract class CachedQuery<TContext, TCacheEntryOptions, TCachedResult> : Operation
	{
		protected CachedQuery() => State = new Store(this, getCacheKey);

		string getCacheKey()
		{
			var cacheConfig = new CacheConfig(GetType().FullName);
			ConfigureCache(cacheConfig);
			return cacheConfig.Key;
		}

		internal readonly Store State;

		/// <summary>
		/// <para>Configures details for constructing a cache key.</para>
		/// <para>Implementors can choose not to override this method if the cache key doesn't need to vary by anything.</para>
		/// </summary>
		/// <param name="cacheConfig">The configuration object.</param>
		protected virtual void ConfigureCache(ICacheConfig cacheConfig) { }

		/// <summary>
		/// <para>Returns options pertaining to the cache entry (such as expiration policy).</para>
		/// <para>Implementors must override this method in order to specify the behaviour of cache entries.</para>
		/// </summary>
		/// <param name="context">The context with which the query will execute.</param>
		/// <returns>The options pertaining to the cache entry.</returns>
		protected abstract TCacheEntryOptions GetCacheEntryOptions(TContext context);

		internal class Store
		{
			readonly CachedQuery<TContext, TCacheEntryOptions, TCachedResult> _query;

			public Store(CachedQuery<TContext, TCacheEntryOptions, TCachedResult> query, Func<string> makeCacheKey)
			{
				_query = query;
				_cacheKey = new Lazy<string>(makeCacheKey);
			}

			internal void Inject(TContext context)
			{
				_cacheEntryOptions = new Lazy<TCacheEntryOptions>(() => _query.GetCacheEntryOptions(context));
			}

			Lazy<TCacheEntryOptions> _cacheEntryOptions;
			internal TCacheEntryOptions GetCacheEntryOptions() => _cacheEntryOptions.Value;

			readonly Lazy<string> _cacheKey;
			internal string CacheKey => _cacheKey.Value;

			bool _hasCachedResult;
			TCachedResult _cachedResult;
			internal TCachedResult CachedResult
			{
				get
				{
					if (!_hasCachedResult)
						throw new InvalidOperationException("Cached result is not available");
					return _cachedResult;
				}
				set
				{
					_cachedResult = value;
					_hasCachedResult = true;
				}
			}
		}
	}
}