using System;
using System.Threading.Tasks;

namespace Data.Operations
{
	public class DataQueryCache : IDataQueryCache
	{
		readonly ICacheStore _cacheStore;

		public DataQueryCache(ICacheStore cacheStore)
		{
			_cacheStore = cacheStore;
		}

		public virtual T Get<T>(Func<T> executeQuery, ICacheInfo cacheInfo)
		{
			var item = _cacheStore.GetItem(cacheInfo.CacheKey);
			if (item != null)
			{
				if (item is NullToken)
					return default(T);
				if (item is T)
					return (T)item;
			}

			var result = executeQuery();
			if (result != null || cacheInfo.CacheNulls)
				_cacheStore.SetItem(cacheInfo.CacheKey, (object)result ?? NullToken.Instance, cacheInfo.AbsoluteDuration);
			return result;
		}

		public virtual async Task<T> GetAsync<T>(Func<Task<T>> executeQueryAsync, ICacheInfo cacheInfo)
		{
			var item = await _cacheStore.GetItemAsync(cacheInfo.CacheKey).ConfigureAwait(false);
			if (item != null)
			{
				if (item is NullToken)
					return default(T);
				if (item is T)
					return (T)item;
			}

			var result = await executeQueryAsync().ConfigureAwait(false);
			if (result != null || cacheInfo.CacheNulls)
				await _cacheStore.SetItemAsync(cacheInfo.CacheKey, (object)result ?? NullToken.Instance, cacheInfo.AbsoluteDuration).ConfigureAwait(false);
			return result;
		}

		public virtual void Refresh<T>(T queryResult, ICacheInfo cacheInfo)
		{
			if (queryResult != null || cacheInfo.CacheNulls)
				_cacheStore.SetItem(cacheInfo.CacheKey, (object)queryResult ?? NullToken.Instance, cacheInfo.AbsoluteDuration);
			else
				_cacheStore.RemoveItem(cacheInfo.CacheKey);
		}

		public virtual Task RefreshAsync<T>(T queryResult, ICacheInfo cacheInfo)
		{
			if (queryResult != null || cacheInfo.CacheNulls)
				return _cacheStore.SetItemAsync(cacheInfo.CacheKey, (object)queryResult ?? NullToken.Instance, cacheInfo.AbsoluteDuration);
			return _cacheStore.RemoveItemAsync(cacheInfo.CacheKey);
		}

		public virtual void Evict(ICacheInfo cacheInfo)
		{
			_cacheStore.RemoveItem(cacheInfo.CacheKey);
		}

		public virtual Task EvictAsync(ICacheInfo cacheInfo)
		{
			return _cacheStore.RemoveItemAsync(cacheInfo.CacheKey);
		}

		internal class NullToken
		{
			public static readonly NullToken Instance = new NullToken();
		}
	}
}
