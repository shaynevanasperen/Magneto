using System;
using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto.Configuration
{
	public interface IQueryCache<in TCacheEntryOptions> : ISyncQueryCache<TCacheEntryOptions>, IAsyncQueryCache<TCacheEntryOptions> { }

	/// <summary>
	/// An abstraction for managing the invocation of synchronous queries and the storage and retrieval of their associated results in a cache.
	/// </summary>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options relating to cache entries.</typeparam>
	public interface ISyncQueryCache<in TCacheEntryOptions>
	{
		/// <summary>
		/// Gets the query result from the cache if it exists, otherwise executes the query and stores the result in the cache.
		/// If the query result is found in the cache and its value is <c>null</c> but the <paramref name="cacheInfo"/> does
		/// not allow caching of nulls, the item is removed from the cache and the query is executed.
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="executeQuery">A delegate for executing the query in the case that the result was not found in the cache.</param>
		/// <param name="cacheInfo">Information pertaining to the caching of the query result.</param>
		/// <param name="getCacheEntryOptions">A callback delegate for obtaining options pertaining to caching of the query result,
		/// in the case that the result was not found in the cache.</param>
		/// <returns>The result from either the cache or the query execution.</returns>
		T Get<T>(Func<T> executeQuery, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions);

		/// <summary>
		/// Adds or updates the given query result in the cache (if the <paramref name="queryResult"/> is not <c>null</c>
		/// or the <paramref name="cacheInfo"/> allows caching of nulls).
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="queryResult">The value to be added/updated.</param>
		/// <param name="cacheInfo">Information pertaining to the caching of the query result.</param>
		/// <param name="getCacheEntryOptions">A callback delegate for obtaining options pertaining to caching of the <paramref name="queryResult"/>,
		/// in the case that its value is not <c>null</c> or the <paramref name="cacheInfo"/> allows caching of nulls.</param>
		void Set<T>(T queryResult, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions);

		/// <summary>
		/// Removes an entry with the specified <paramref name="key"/> from the cache.
		/// </summary>
		/// <param name="key">The key for the cached item.</param>
		void Evict(string key);
	}

	/// <summary>
	/// An abstraction for managing the invocation of asynchronous queries and the storage and retrieval of their associated results in a cache.
	/// </summary>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options relating to cache entries.</typeparam>
	public interface IAsyncQueryCache<in TCacheEntryOptions>
	{
		/// <summary>
		/// Gets the query result from the cache if it exists, otherwise executes the query and stores the result in the cache.
		/// If the query result is found in the cache and its value is <c>null</c> but the <paramref name="cacheInfo"/> does
		/// not allow caching of nulls, the item is removed from the cache and the query is executed.
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="executeQueryAsync">A delegate for executing the query in the case that the result was not found in the cache.</param>
		/// <param name="cacheInfo">Information pertaining to the caching of the query result.</param>
		/// <param name="getCacheEntryOptions">A callback delegate for obtaining options pertaining to caching of the query result,
		/// in the case that the result was not found in the cache.</param>
		/// <returns>The result from either the cache or the query execution.</returns>
		Task<T> GetAsync<T>(Func<Task<T>> executeQueryAsync, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions);

		/// <summary>
		/// Adds or updates the given query result in the cache (if the <paramref name="queryResult"/> is not <c>null</c>
		/// or the <paramref name="cacheInfo"/> allows caching of nulls).
		/// </summary>
		/// <typeparam name="T">The type of the query result.</typeparam>
		/// <param name="queryResult">The value to be added/updated.</param>
		/// <param name="cacheInfo">Information pertaining to the caching of the query result.</param>
		/// <param name="getCacheEntryOptions">A callback delegate for obtaining options pertaining to caching of the <paramref name="queryResult"/>,
		/// in the case that its value is not <c>null</c> or the <paramref name="cacheInfo"/> allows caching of nulls.</param>
		/// <returns>A task representing the add/update operation.</returns>
		Task SetAsync<T>(T queryResult, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions);

		/// <summary>
		/// Removes an entry with the specified <paramref name="key"/> from the cache.
		/// </summary>
		/// <param name="key">The key for the cached item.</param>
		/// <returns>A task representing the remove operation.</returns>
		Task EvictAsync(string key);
	}
}