using System;
using System.Threading;
using System.Threading.Tasks;
using Magneto.Configuration;
using Magneto.Core;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Magneto.Microsoft
{
	/// <summary>
	/// An implementation of <see cref="ICacheStore{TCacheEntryOptions}"/> backed by <see cref="IDistributedCache"/>.
	/// </summary>
	public class DistributedCacheStore : ICacheStore<DistributedCacheEntryOptions>
	{
		readonly IDistributedCache _distributedCache;

		/// <summary>
		/// Creates a new instance of <see cref="DistributedCacheStore"/> using the given <see cref="IDistributedCache"/>.
		/// </summary>
		/// <param name="distributedCache">The underlying cache implementation.</param>
		public DistributedCacheStore(IDistributedCache distributedCache) => _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

		/// <inheritdoc cref="ISyncCacheStore{DistributedCacheEntryOptions}.Get{T}"/>
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

		/// <inheritdoc cref="ISyncCacheStore{DistributedCacheEntryOptions}.Set{T}"/>
		public void Set<T>(string key, CacheEntry<T> item, DistributedCacheEntryOptions cacheEntryOptions)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (cacheEntryOptions == null) throw new ArgumentNullException(nameof(cacheEntryOptions));

			var json = JsonConvert.SerializeObject(item);
			_distributedCache.SetString(key, json, cacheEntryOptions);
		}

		/// <inheritdoc cref="ISyncCacheStore{DistributedCacheEntryOptions}.Remove"/>
		public void Remove(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			_distributedCache.Remove(key);
		}

		/// <inheritdoc cref="IAsyncCacheStore{DistributedCacheEntryOptions}.GetAsync{T}"/>
		public async Task<CacheEntry<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			var json = await _distributedCache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);
			if (json == null)
				return null;

			try
			{
				return JsonConvert.DeserializeObject<CacheEntry<T>>(json);
			}
			catch (JsonSerializationException)
			{
				await RemoveAsync(key, cancellationToken).ConfigureAwait(false);
				throw;
			}
		}

		/// <inheritdoc cref="IAsyncCacheStore{DistributedCacheEntryOptions}.SetAsync{T}"/>
		public Task SetAsync<T>(string key, CacheEntry<T> item, DistributedCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken = default)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (cacheEntryOptions == null) throw new ArgumentNullException(nameof(cacheEntryOptions));

			var json = JsonConvert.SerializeObject(item);
			return _distributedCache.SetStringAsync(key, json, cacheEntryOptions, cancellationToken);
		}

		/// <inheritdoc cref="IAsyncCacheStore{DistributedCacheEntryOptions}.RemoveAsync"/>
		public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			return _distributedCache.RemoveAsync(key, cancellationToken);
		}
	}
}
