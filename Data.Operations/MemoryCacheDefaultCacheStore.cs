using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Data.Operations
{
	public class MemoryCacheDefaultCacheStore : ICacheStore
	{
		public virtual object GetItem(string cacheKey)
		{
			return MemoryCache.Default.Get(cacheKey);
		}

		public virtual Task<object> GetItemAsync(string cacheKey)
		{
			return Task.FromResult(GetItem(cacheKey));
		}

		public virtual void SetItem(string cacheKey, object item, TimeSpan absoluteDuration)
		{
			MemoryCache.Default.Set(cacheKey, item, DateTimeOffset.Now.Add(absoluteDuration));
		}

		public virtual Task SetItemAsync(string cacheKey, object item, TimeSpan absoluteDuration)
		{
			SetItem(cacheKey, item, absoluteDuration);
			return Task.FromResult(0);
		}

		public virtual void RemoveItem(string cacheKey)
		{
			MemoryCache.Default.Remove(cacheKey);
		}

		public virtual Task RemoveItemAsync(string cacheKey)
		{
			RemoveItem(cacheKey);
			return Task.FromResult(0);
		}
	}
}
