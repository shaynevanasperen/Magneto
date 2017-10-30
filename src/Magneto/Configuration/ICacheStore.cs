using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto.Configuration
{
	public interface ICacheStore<in TCacheEntryOptions> : ISyncCacheStore<TCacheEntryOptions>, IAsyncCacheStore<TCacheEntryOptions> { }

	/// <summary>
	/// A store for managing cache entries synchronously
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
		CacheEntry<T> Get<T>(string key);

		/// <summary>
		/// Adds or updates an entry in the cache with the specified <paramref name="key"/> and <paramref name="cacheEntryOptions"/>.
		/// </summary>
		/// <typeparam name="T">The type of the cached item.</typeparam>
		/// <param name="key">The key for the cached item.</param>
		/// <param name="item">The value to be added/updated.</param>
		/// <param name="cacheEntryOptions">Options pertaining to the cache entry (such as expiration policy).</param>
		void Set<T>(string key, CacheEntry<T> item, TCacheEntryOptions cacheEntryOptions);

		/// <summary>
		/// Removes an entry with the specified <paramref name="key"/> from the cache.
		/// </summary>
		/// <param name="key">The key for the cached item.</param>
		void Remove(string key);
	}

	/// <summary>
	/// A store for managing cache entries asynchronously
	/// </summary>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options relating to cache entries.</typeparam>
	public interface IAsyncCacheStore<in TCacheEntryOptions>
	{
		/// <summary>
		/// Gets an entry from the cache with the specified <paramref name="key"/>.
		/// </summary>
		/// <typeparam name="T">The type of the cached item.</typeparam>
		/// <param name="key">The key for the cached item.</param>
		/// <returns>A <see cref="CacheEntry{T}"/> if it exits in the cache, otherwise <c>null</c>.</returns>
		Task<CacheEntry<T>> GetAsync<T>(string key);

		/// <summary>
		/// Adds or updates an entry in the cache with the specified <paramref name="key"/> and <paramref name="cacheEntryOptions"/>.
		/// </summary>
		/// <typeparam name="T">The type of the cached item.</typeparam>
		/// <param name="key">The key for the cached item.</param>
		/// <param name="item">The value to be added/updated.</param>
		/// <param name="cacheEntryOptions">Options pertaining to the cache entry (such as expiration policy).</param>
		/// <returns>A task representing the add/update operation.</returns>
		Task SetAsync<T>(string key, CacheEntry<T> item, TCacheEntryOptions cacheEntryOptions);

		/// <summary>
		/// Removes an entry with the specified <paramref name="key"/> from the cache.
		/// </summary>
		/// <param name="key">The key for the cached item.</param>
		/// <returns>A task representing the remove operation.</returns>
		Task RemoveAsync(string key);
	}
}