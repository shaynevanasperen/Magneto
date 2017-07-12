using System;
using System.Threading.Tasks;
using Magneto.Configuration;
using Magneto.Core;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Magneto.Microsoft
{
	public class DistributedCacheStore : ICacheStore<DistributedCacheEntryOptions>
	{
		readonly IDistributedCache _distributedCache;

		public DistributedCacheStore(IDistributedCache distributedCache) => _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

		public CacheEntry<T> Get<T>(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			var json = _distributedCache.GetString(key);
			if (json == null)
				return null;

			try
			{
				return JsonConvert.DeserializeObject<CacheEntry<T>>(json);
			}
			catch (JsonSerializationException)
			{
				Remove(key);
				throw;
			}
		}

		public void Set<T>(string key, CacheEntry<T> item, DistributedCacheEntryOptions cacheEntryOptions)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (cacheEntryOptions == null) throw new ArgumentNullException(nameof(cacheEntryOptions));

			var json = JsonConvert.SerializeObject(item);
			_distributedCache.SetString(key, json, cacheEntryOptions);
		}

		public void Remove(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			_distributedCache.Remove(key);
		}

		public async Task<CacheEntry<T>> GetAsync<T>(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			var json = await _distributedCache.GetStringAsync(key).ConfigureAwait(false);
			if (json == null)
				return null;

			try
			{
				return JsonConvert.DeserializeObject<CacheEntry<T>>(json);
			}
			catch (JsonSerializationException)
			{
				await RemoveAsync(key).ConfigureAwait(false);
				throw;
			}
		}

		public Task SetAsync<T>(string key, CacheEntry<T> item, DistributedCacheEntryOptions cacheEntryOptions)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (cacheEntryOptions == null) throw new ArgumentNullException(nameof(cacheEntryOptions));

			var json = JsonConvert.SerializeObject(item);
			return _distributedCache.SetStringAsync(key, json, cacheEntryOptions);
		}

		public Task RemoveAsync(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			return _distributedCache.RemoveAsync(key);
		}
	}
}
