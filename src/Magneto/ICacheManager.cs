using System.Threading;
using System.Threading.Tasks;

namespace Magneto
{
	/// <summary>
	/// A mediary for managing cached results of queries.
	/// </summary>
	public interface ICacheManager : ISyncCacheManager, IAsyncCacheManager { }

	/// <summary>
	/// A mediary for managing cached results of queries synchronously.
	/// </summary>
	public interface ISyncCacheManager
	{
		/// <summary>
		/// Removes a previously cached result for <paramref name="query"/> from the cache.
		/// </summary>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the <paramref name="query"/>.</typeparam>
		/// <param name="query">The query object for which a cached result should be removed.</param>
		void EvictCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> query);

		/// <summary>
		/// <para>Updates the previously cached result of <paramref name="executedQuery"/> in the cache. This is typically used only when working with a distributed cache.</para>
		/// <para>Not required when using an in-memory cache because any mutations to the cached result would be reflected in the cache.</para>
		/// <remarks>Should only be used with instances of query objects which have already been executed.</remarks>
		/// </summary>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the <paramref name="executedQuery"/>.</typeparam>
		/// <param name="executedQuery">The previously executed query object for which the cached result should be updated.</param>
		void UpdateCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> executedQuery);
	}

	/// <summary>
	/// A mediary for managing cached results of queries asynchronously.
	/// </summary>
	public interface IAsyncCacheManager
	{
		/// <summary>
		/// Removes a previously cached result for <paramref name="query"/> from the cache.
		/// </summary>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the <paramref name="query"/>.</typeparam>
		/// <param name="query">The query object for which a cached result should be removed.</param>
		/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
		/// <returns>A task representing the eviction of the cached result.</returns>
		Task EvictCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> query, CancellationToken cancellationToken = default);

		/// <summary>
		/// <para>Updates the previously cached result of <paramref name="executedQuery"/> in the cache. This is typically used only when working with a distributed cache.</para>
		/// <para>Not required when using an in-memory cache because any mutations to the cached result would be reflected in the cache.</para>
		/// <remarks>Should only be used with instances of query objects which have already been executed.</remarks>
		/// </summary>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the <paramref name="executedQuery"/>.</typeparam>
		/// <param name="executedQuery">The previously executed query object for which the cached result should be updated.</param>
		/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
		/// <returns>A task representing the eviction of the cached result.</returns>
		Task UpdateCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> executedQuery, CancellationToken cancellationToken = default);
	}
}