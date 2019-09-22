using System;
using System.Threading;
using System.Threading.Tasks;
using Magneto.Configuration;
using Magneto.Core;
using Microsoft.Extensions.Caching.Memory;

namespace Magneto.Microsoft
{
	/// <summary>
	/// An implementation of <see cref="ICacheStore{TCacheEntryOptions}"/> backed by <see cref="IMemoryCache"/>.
	/// </summary>
	public class MemoryCacheStore : ICacheStore<MemoryCacheEntryOptions>
	{
		readonly IMemoryCache _memoryCache;

		/// <summary>
		/// Creates a new instance of <see cref="MemoryCacheStore"/> using the given <see cref="IMemoryCache"/>.
		/// </summary>
		/// <param name="memoryCache">The underlying cache implementation.</param>
		public MemoryCacheStore(IMemoryCache memoryCache) => _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

		/// <inheritdoc cref="ISyncCacheStore{DistributedCacheEntryOptions}.Get{T}"/>
		public CacheEntry<T> Get<T>(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			return _memoryCache.Get<CacheEntry<T>>(key);
		}

		/// <inheritdoc cref="IAsyncCacheStore{DistributedCacheEntryOptions}.GetAsync{T}"/>
		public Task<CacheEntry<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			return Task.FromResult(_memoryCache.Get<CacheEntry<T>>(key));
		}

		/// <inheritdoc cref="ISyncCacheStore{DistributedCacheEntryOptions}.Set{T}"/>
		public void Set<T>(string key, CacheEntry<T> item, MemoryCacheEntryOptions cacheEntryOptions)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (cacheEntryOptions == null) throw new ArgumentNullException(nameof(cacheEntryOptions));

			_memoryCache.Set(key, item, cacheEntryOptions);
		}

		/// <inheritdoc cref="IAsyncCacheStore{DistributedCacheEntryOptions}.SetAsync{T}"/>
		public Task SetAsync<T>(string key, CacheEntry<T> item, MemoryCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken = default)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (cacheEntryOptions == null) throw new ArgumentNullException(nameof(cacheEntryOptions));

			_memoryCache.Set(key, item, cacheEntryOptions);
			return Task.CompletedTask;
		}

		/// <inheritdoc cref="ISyncCacheStore{DistributedCacheEntryOptions}.Remove"/>
		public void Remove(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			_memoryCache.Remove(key);
		}

		/// <inheritdoc cref="IAsyncCacheStore{DistributedCacheEntryOptions}.RemoveAsync"/>
		public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			_memoryCache.Remove(key);
			return Task.CompletedTask;
		}
	}
}
