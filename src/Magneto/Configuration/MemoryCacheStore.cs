using System;
using System.Threading;
using System.Threading.Tasks;
using Magneto.Core;
using Microsoft.Extensions.Caching.Memory;

namespace Magneto.Configuration;

/// <summary>
/// An implementation of <see cref="ICacheStore{TCacheEntryOptions}"/> backed by <see cref="IMemoryCache"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="MemoryCacheStore"/> using the given <see cref="IMemoryCache"/>.
/// </remarks>
/// <param name="memoryCache">The underlying cache implementation.</param>
public class MemoryCacheStore(IMemoryCache memoryCache) : ICacheStore<MemoryCacheEntryOptions>
{
	readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

	/// <inheritdoc cref="ISyncCacheStore{TCacheEntryOptions}.GetEntry{T}"/>
	public CacheEntry<T>? GetEntry<T>(string key)
	{
		ArgumentNullException.ThrowIfNull(key);

		return _memoryCache.Get<CacheEntry<T>>(key);
	}

	/// <inheritdoc cref="IAsyncCacheStore{TCacheEntryOptions}.GetEntryAsync{T}"/>
	public Task<CacheEntry<T>?> GetEntryAsync<T>(string key, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(key);

		return Task.FromResult(_memoryCache.Get<CacheEntry<T>>(key));
	}

	/// <inheritdoc cref="ISyncCacheStore{TCacheEntryOptions}.SetEntry{T}"/>
	public void SetEntry<T>(string key, CacheEntry<T> item, MemoryCacheEntryOptions cacheEntryOptions)
	{
		ArgumentNullException.ThrowIfNull(key);
		ArgumentNullException.ThrowIfNull(item);
		ArgumentNullException.ThrowIfNull(cacheEntryOptions);

		_memoryCache.Set(key, item, cacheEntryOptions);
	}

	/// <inheritdoc cref="IAsyncCacheStore{TCacheEntryOptions}.SetEntryAsync{T}"/>
	public Task SetEntryAsync<T>(string key, CacheEntry<T> item, MemoryCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(key);
		ArgumentNullException.ThrowIfNull(item);
		ArgumentNullException.ThrowIfNull(cacheEntryOptions);

		_memoryCache.Set(key, item, cacheEntryOptions);
		return Task.CompletedTask;
	}

	/// <inheritdoc cref="ISyncCacheStore{TCacheEntryOptions}.RemoveEntry"/>
	public void RemoveEntry(string key)
	{
		ArgumentNullException.ThrowIfNull(key);

		_memoryCache.Remove(key);
	}

	/// <inheritdoc cref="IAsyncCacheStore{TCacheEntryOptions}.RemoveEntryAsync"/>
	public Task RemoveEntryAsync(string key, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(key);

		_memoryCache.Remove(key);
		return Task.CompletedTask;
	}
}
