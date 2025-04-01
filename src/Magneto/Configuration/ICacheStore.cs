using System.Threading;
using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto.Configuration;

/// <summary>
/// A store for managing cache entries. We use a <see cref="CacheEntry{T}"/> for wrapping the cached values so
/// that we can distinguish between a cache-miss and a null value that was cached.
/// </summary>
/// <typeparam name="TCacheEntryOptions">The type of cache entry options relating to cache entries.</typeparam>
public interface ICacheStore<in TCacheEntryOptions> : ISyncCacheStore<TCacheEntryOptions>, IAsyncCacheStore<TCacheEntryOptions> { }

/// <summary>
/// A store for managing cache entries synchronously. We use a <see cref="CacheEntry{T}"/> for wrapping the cached values so
/// that we can distinguish between a cache-miss and a null value that was cached.
/// </summary>
/// <typeparam name="TCacheEntryOptions">The type of cache entry options relating to cache entries.</typeparam>
public interface ISyncCacheStore<in TCacheEntryOptions>
{
	/// <summary>
	/// Gets an entry from the cache with the specified <paramref name="key"/>.
	/// </summary>
	/// <typeparam name="T">The type of the cached item.</typeparam>
	/// <param name="key">The key for the cached item.</param>
	/// <returns>A <see cref="CacheEntry{T}"/> if it exits in the cache, otherwise <c>null</c>.</returns>
	CacheEntry<T>? GetEntry<T>(string key);

	/// <summary>
	/// Adds or updates an entry in the cache with the specified <paramref name="key"/> and <paramref name="cacheEntryOptions"/>.
	/// </summary>
	/// <typeparam name="T">The type of the cached item.</typeparam>
	/// <param name="key">The key for the cached item.</param>
	/// <param name="item">The value to be added/updated.</param>
	/// <param name="cacheEntryOptions">Options pertaining to the cache entry (such as expiration policy).</param>
	void SetEntry<T>(string key, CacheEntry<T> item, TCacheEntryOptions cacheEntryOptions);

	/// <summary>
	/// Removes an entry with the specified <paramref name="key"/> from the cache.
	/// </summary>
	/// <param name="key">The key for the cached item.</param>
	void RemoveEntry(string key);
}

/// <summary>
/// A store for managing cache entries asynchronously. We use a <see cref="CacheEntry{T}"/> for wrapping the cached values so
/// that we can distinguish between a cache-miss and a null value that was cached.
/// </summary>
/// <typeparam name="TCacheEntryOptions">The type of cache entry options relating to cache entries.</typeparam>
public interface IAsyncCacheStore<in TCacheEntryOptions>
{
	/// <summary>
	/// Gets an entry from the cache with the specified <paramref name="key"/>.
	/// </summary>
	/// <typeparam name="T">The type of the cached item.</typeparam>
	/// <param name="key">The key for the cached item.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the operation.</param>
	/// <returns>A <see cref="CacheEntry{T}"/> if it exits in the cache, otherwise <c>null</c>.</returns>
	Task<CacheEntry<T>?> GetEntryAsync<T>(string key, CancellationToken cancellationToken);

	/// <summary>
	/// Adds or updates an entry in the cache with the specified <paramref name="key"/> and <paramref name="cacheEntryOptions"/>.
	/// </summary>
	/// <typeparam name="T">The type of the cached item.</typeparam>
	/// <param name="key">The key for the cached item.</param>
	/// <param name="item">The value to be added/updated.</param>
	/// <param name="cacheEntryOptions">Options pertaining to the cache entry (such as expiration policy).</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the operation.</param>
	/// <returns>A task representing the add/update operation.</returns>
	Task SetEntryAsync<T>(string key, CacheEntry<T> item, TCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken);

	/// <summary>
	/// Removes an entry with the specified <paramref name="key"/> from the cache.
	/// </summary>
	/// <param name="key">The key for the cached item.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the operation.</param>
	/// <returns>A task representing the remove operation.</returns>
	Task RemoveEntryAsync(string key, CancellationToken cancellationToken);
}
