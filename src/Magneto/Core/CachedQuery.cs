using System;
using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto.Core
{
	public abstract class SyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult> : CachedQuery<TContext, TCacheEntryOptions, TCachedResult>, ISyncCachedQuery<TCacheEntryOptions>
	{
		protected abstract TCachedResult Query(TContext context);

		protected virtual TCachedResult GetCachedResult(TContext context, ISyncDecorator decorator, ISyncQueryCache<TCacheEntryOptions> queryCache, CacheOption cacheOption = CacheOption.Default)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (decorator == null) throw new ArgumentNullException(nameof(decorator));
			if (queryCache == null) throw new ArgumentNullException(nameof(queryCache));

			State.Inject(context);
			TCachedResult query() => decorator.Decorate(this, context, Query);

			if (cacheOption == CacheOption.Default)
				return State.CachedResult = queryCache.Get(query, State.CacheInfo, State.GetCacheEntryOptions);

			State.CachedResult = query();
			queryCache.Set(State.CachedResult, State.CacheInfo, State.GetCacheEntryOptions);
			return State.CachedResult;
		}

		public virtual void EvictCachedResult(ISyncQueryCache<TCacheEntryOptions> queryCache)
		{
			if (queryCache == null) throw new ArgumentNullException(nameof(queryCache));

			queryCache.Evict(State.CacheInfo.Key);
		}

		public virtual void UpdateCachedResult(ISyncQueryCache<TCacheEntryOptions> queryCache)
		{
			if (queryCache == null) throw new ArgumentNullException(nameof(queryCache));

			queryCache.Set(State.CachedResult, State.CacheInfo, State.GetCacheEntryOptions);
		}
	}

	public abstract class AsyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult> : CachedQuery<TContext, TCacheEntryOptions, TCachedResult>, IAsyncCachedQuery<TCacheEntryOptions>
	{
		protected abstract Task<TCachedResult> QueryAsync(TContext context);

		protected virtual async Task<TCachedResult> GetCachedResultAsync(TContext context, IAsyncDecorator decorator, IAsyncQueryCache<TCacheEntryOptions> queryCache, CacheOption cacheOption = CacheOption.Default)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (decorator == null) throw new ArgumentNullException(nameof(decorator));
			if (queryCache == null) throw new ArgumentNullException(nameof(queryCache));

			State.Inject(context);
			Task<TCachedResult> queryAsync() => decorator.Decorate(this, context, QueryAsync);

			if (cacheOption == CacheOption.Default)
				return State.CachedResult = await queryCache.GetAsync(queryAsync, State.CacheInfo, State.GetCacheEntryOptions).ConfigureAwait(false);

			State.CachedResult = await queryAsync().ConfigureAwait(false);
			await queryCache.SetAsync(State.CachedResult, State.CacheInfo, State.GetCacheEntryOptions).ConfigureAwait(false);
			return State.CachedResult;
		}

		public virtual Task EvictCachedResultAsync(IAsyncQueryCache<TCacheEntryOptions> queryCache)
		{
			if (queryCache == null) throw new ArgumentNullException(nameof(queryCache));

			return queryCache.EvictAsync(State.CacheInfo.Key);
		}

		public virtual Task UpdateCachedResultAsync(IAsyncQueryCache<TCacheEntryOptions> queryCache)
		{
			if (queryCache == null) throw new ArgumentNullException(nameof(queryCache));

			return queryCache.SetAsync(State.CachedResult, State.CacheInfo, State.GetCacheEntryOptions);
		}
	}

	public abstract class CachedQuery<TContext, TCacheEntryOptions, TCachedResult> : Operation
	{
		protected CachedQuery() => State = new Store(this, getCacheInfo);

		CacheInfo getCacheInfo()
		{
			var cacheInfo = new CacheInfo(GetType().FullName);
			ConfigureCache(cacheInfo);
			return cacheInfo;
		}

		internal readonly Store State;

		protected virtual void ConfigureCache(ICacheConfig cacheConfig) { }

		protected abstract TCacheEntryOptions GetCacheEntryOptions(TContext context);

		internal class Store
		{
			readonly CachedQuery<TContext, TCacheEntryOptions, TCachedResult> _query;

			public Store(CachedQuery<TContext, TCacheEntryOptions, TCachedResult> query, Func<ICacheInfo> makeCacheInfo)
			{
				_query = query;
				_cacheInfo = new Lazy<ICacheInfo>(makeCacheInfo);
			}

			internal void Inject(TContext context)
			{
				_cacheEntryOptions = new Lazy<TCacheEntryOptions>(() => _query.GetCacheEntryOptions(context));
			}

			Lazy<TCacheEntryOptions> _cacheEntryOptions;
			internal TCacheEntryOptions GetCacheEntryOptions() => _cacheEntryOptions.Value;

			readonly Lazy<ICacheInfo> _cacheInfo;
			internal ICacheInfo CacheInfo => _cacheInfo.Value;

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