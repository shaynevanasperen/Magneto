namespace Magneto
{
	public enum CacheOption
	{
		/// <summary>
		/// Specifies that the cache <c>should be read</c> when executing a cached query <c>(read-write)</c>.
		/// </summary>
		Default,

		/// <summary>
		/// Specifies that the cache <c>should not be read</c> when executing a cached query <c>(write-only)</c>.
		/// </summary>
		Refresh
	}
}
