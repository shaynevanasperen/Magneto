namespace Magneto.Core
{
	public interface ICacheInfo
	{
		/// <summary>
		/// Specifies whether or not <c>null</c> items should be cached.
		/// </summary>
		bool CacheNulls { get; }

		/// <summary>
		/// The unique key to be used for each cache entry.
		/// </summary>
		string Key { get; }
	}
}