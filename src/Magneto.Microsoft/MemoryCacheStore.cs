using System;
using System.Threading.Tasks;
using Magneto.Configuration;
using Magneto.Core;
using Microsoft.Extensions.Caching.Memory;

namespace Magneto.Microsoft
{
	public class MemoryCacheStore : ICacheStore<MemoryCacheEntryOptions>
	{
		readonly IMemoryCache _memoryCache;

		public MemoryCacheStore(IMemoryCache memoryCache) => _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

		public CacheEntry<T> Get<T>(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			return _memoryCache.Get<CacheEntry<T>>(key);
		}

		public void Set<T>(string key, CacheEntry<T> item, MemoryCacheEntryOptions cacheEntryOptions)
		{
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (cacheEntryOptions == null) throw new ArgumentNullException(nameof(cacheEntryOptions));

			_memoryCache.Set(key, item, cacheEntryOptions);
		}

		public void Remove(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			_memoryCache.Remove(key);
		}

		public Task<CacheEntry<T>> GetAsync<T>(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			return Task.FromResult(Get<T>(key));
		}

		public Task SetAsync<T>(string key, CacheEntry<T> item, MemoryCacheEntryOptions cacheEntryOptions)
		{
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (cacheEntryOptions == null) throw new ArgumentNullException(nameof(cacheEntryOptions));

			Set(key, item, cacheEntryOptions);
#if NETSTANDARD1_3
			return Task.CompletedTask;
#else
			return Task.FromResult(0);
#endif
		}

		public Task RemoveAsync(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			Remove(key);
#if NETSTANDARD1_3
			return Task.CompletedTask;
#else
			return Task.FromResult(0);
#endif
		}
	}
}
