namespace Magneto
{
	/// <summary>
	/// An option designating whether or not the cache should be read when executing a query.
	/// </summary>
	public enum CacheOption
	{
		/// <summary>
		/// Specifies that the cache should be read when executing a cached query (read-write).
		/// </summary>
		Default,

		/// <summary>
		/// Specifies that the cache should not be read when executing a cached query (write-only).
		/// </summary>
		Refresh
	}
}
