using System;
using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto.Core
{
	public abstract class SyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult> : CachedQuery<TContext, TCacheEntryOptions, TCachedResult>, ISyncCachedQuery<TCacheEntryOptions>
	{
		protected abstract TCachedResult Query(TContext context);

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

		public virtual void EvictCachedResult(ISyncCacheStore<TCacheEntryOptions> cacheStore)
		{
			if (cacheStore == null) throw new ArgumentNullException(nameof(cacheStore));

			cacheStore.Remove(State.CacheKey);
		}

		public virtual void UpdateCachedResult(ISyncCacheStore<TCacheEntryOptions> cacheStore)
		{
			if (cacheStore == null) throw new ArgumentNullException(nameof(cacheStore));

			cacheStore.Set(State.CacheKey, State.CachedResult.ToCacheEntry(), State.GetCacheEntryOptions());
		}
	}

	public abstract class AsyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult> : CachedQuery<TContext, TCacheEntryOptions, TCachedResult>, IAsyncCachedQuery<TCacheEntryOptions>
	{
		protected abstract Task<TCachedResult> QueryAsync(TContext context);

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

		public virtual Task EvictCachedResultAsync(IAsyncCacheStore<TCacheEntryOptions> cacheStore)
		{
			if (cacheStore == null) throw new ArgumentNullException(nameof(cacheStore));

			return cacheStore.RemoveAsync(State.CacheKey);
		}

		public virtual Task UpdateCachedResultAsync(IAsyncCacheStore<TCacheEntryOptions> cacheStore)
		{
			if (cacheStore == null) throw new ArgumentNullException(nameof(cacheStore));

			return cacheStore.SetAsync(State.CacheKey, State.CachedResult.ToCacheEntry(), State.GetCacheEntryOptions());
		}
	}

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
		/// Configures details for constructing a cache key.
		/// <para>Implementors can choose not to override this method if the cache key doesn't need to vary by anything.</para>
		/// </summary>
		/// <param name="cacheConfig">The configuration object.</param>
		protected virtual void ConfigureCache(ICacheConfig cacheConfig) { }

		/// <summary>
		/// Returns options pertaining to the cache entry (such as expiration policy).
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