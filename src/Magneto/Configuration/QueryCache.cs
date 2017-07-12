using System;
using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto.Configuration
{
	public class QueryCache<TCacheEntryOptions> : IQueryCache<TCacheEntryOptions>
	{
		readonly ICacheStore<TCacheEntryOptions> _cacheStore;

		public QueryCache(ICacheStore<TCacheEntryOptions> cacheStore) => _cacheStore = cacheStore ?? throw new ArgumentNullException(nameof(cacheStore));

		public virtual T Get<T>(Func<T> executeQuery, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions)
		{
			if (executeQuery == null) throw new ArgumentNullException(nameof(executeQuery));
			if (cacheInfo == null) throw new ArgumentNullException(nameof(cacheInfo));
			if (getCacheEntryOptions == null) throw new ArgumentNullException(nameof(getCacheEntryOptions));

			var cacheEntry = _cacheStore.Get<T>(cacheInfo.Key);
			if (cacheEntry != null)
			{
				if (cacheEntry.Value != null || cacheInfo.CacheNulls)
					return cacheEntry.Value;
				_cacheStore.Remove(cacheInfo.Key);
			}

			var result = executeQuery();
			Set(result, cacheInfo, getCacheEntryOptions);
			return result;
		}

		public virtual async Task<T> GetAsync<T>(Func<Task<T>> executeQueryAsync, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions)
		{
			if (executeQueryAsync == null) throw new ArgumentNullException(nameof(executeQueryAsync));
			if (cacheInfo == null) throw new ArgumentNullException(nameof(cacheInfo));
			if (getCacheEntryOptions == null) throw new ArgumentNullException(nameof(getCacheEntryOptions));

			var cacheEntry = await _cacheStore.GetAsync<T>(cacheInfo.Key).ConfigureAwait(false);
			if (cacheEntry != null)
			{
				if (cacheEntry.Value != null || cacheInfo.CacheNulls)
					return cacheEntry.Value;
				await _cacheStore.RemoveAsync(cacheInfo.Key).ConfigureAwait(false);
			}

			var result = await executeQueryAsync().ConfigureAwait(false);
			await SetAsync(result, cacheInfo, getCacheEntryOptions).ConfigureAwait(false);
			return result;
		}

		public virtual void Set<T>(T queryResult, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions)
		{
			if (cacheInfo == null) throw new ArgumentNullException(nameof(cacheInfo));
			if (getCacheEntryOptions == null) throw new ArgumentNullException(nameof(getCacheEntryOptions));

			if (queryResult == null && !cacheInfo.CacheNulls)
				return;
			_cacheStore.Set(cacheInfo.Key, new CacheEntry<T> { Value = queryResult }, getCacheEntryOptions());
		}

		public virtual async Task SetAsync<T>(T queryResult, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions)
		{
			if (cacheInfo == null) throw new ArgumentNullException(nameof(cacheInfo));
			if (getCacheEntryOptions == null) throw new ArgumentNullException(nameof(getCacheEntryOptions));

			if (queryResult == null && !cacheInfo.CacheNulls)
				return;
			await _cacheStore.SetAsync(cacheInfo.Key, new CacheEntry<T> { Value = queryResult }, getCacheEntryOptions()).ConfigureAwait(false);
		}

		public virtual void Evict(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			_cacheStore.Remove(key);
		}

		public virtual Task EvictAsync(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			return _cacheStore.RemoveAsync(key);
		}
	}
}