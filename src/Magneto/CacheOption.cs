namespace Magneto;

/// <summary>
/// An option designating whether the cache should be checked when executing a query.
/// Use <see cref="Refresh"/> to skip reading from the cache and ensure a fresh result.
/// </summary>
public enum CacheOption
{
	/// <summary>
	/// Specifies that the cache should be checked when executing a cached query (read-write).
	/// In the case of a cache-miss, the fresh result is written to the cache.
	/// </summary>
	Default,

	/// <summary>
	/// Specifies that the cache should not be checked when executing a cached query (write-only).
	/// The fresh result will be written to the cache.
	/// </summary>
	Refresh
}
