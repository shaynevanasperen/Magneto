using System;
using System.Threading;
using System.Threading.Tasks;
using Magneto.Core;
using Microsoft.Extensions.Caching.Memory;

namespace Magneto.Configuration
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

		/// <inheritdoc cref="ISyncCacheStore{TCacheEntryOptions}.GetEntry{T}"/>
		public CacheEntry<T> GetEntry<T>(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			return _memoryCache.Get<CacheEntry<T>>(key);
		}

		/// <inheritdoc cref="IAsyncCacheStore{TCacheEntryOptions}.GetEntryAsync{T}"/>
		public Task<CacheEntry<T>> GetEntryAsync<T>(string key, CancellationToken cancellationToken)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			return Task.FromResult(_memoryCache.Get<CacheEntry<T>>(key));
		}

		/// <inheritdoc cref="ISyncCacheStore{TCacheEntryOptions}.SetEntry{T}"/>
		public void SetEntry<T>(string key, CacheEntry<T> item, MemoryCacheEntryOptions cacheEntryOptions)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (cacheEntryOptions == null) throw new ArgumentNullException(nameof(cacheEntryOptions));

			_memoryCache.Set(key, item, cacheEntryOptions);
		}

		/// <inheritdoc cref="IAsyncCacheStore{TCacheEntryOptions}.SetEntryAsync{T}"/>
		public Task SetEntryAsync<T>(string key, CacheEntry<T> item, MemoryCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (cacheEntryOptions == null) throw new ArgumentNullException(nameof(cacheEntryOptions));

			_memoryCache.Set(key, item, cacheEntryOptions);
			return Task.CompletedTask;
		}

		/// <inheritdoc cref="ISyncCacheStore{TCacheEntryOptions}.RemoveEntry"/>
		public void RemoveEntry(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			_memoryCache.Remove(key);
		}

		/// <inheritdoc cref="IAsyncCacheStore{TCacheEntryOptions}.RemoveEntryAsync"/>
		public Task RemoveEntryAsync(string key, CancellationToken cancellationToken)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			_memoryCache.Remove(key);
			return Task.CompletedTask;
		}
	}
}
