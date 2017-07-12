namespace Magneto
{
	public interface ICacheConfig
	{
		/// <summary>
		/// Specifies whether or not <c>null</c> items should be cached. Defaults to <c>true</c>.
		/// </summary>
		bool CacheNulls { set; }

		/// <summary>
		/// A value to be combined with <see cref="VaryBy"/> to form the cache key.
		/// Defaults to the fully qualified type name of the query class.
		/// </summary>
		string KeyPrefix { set; }

		/// <summary>
		/// An object or collection specifying values to be combined with <see cref="KeyPrefix"/>
		/// to form the cache key.<br/>
		/// <c>VaryBy = new { Value1, Reference1 }</c><br/>
		/// <c>VaryBy = new[] { Value1, Value2 }</c><br/>
		/// <c>VaryBy = string.Format("{0}_{1}", Value1, Reference1.Id)</c><br/>
		/// <c>VaryBy = Value1</c>
		/// </summary>
		object VaryBy { set; }
	}
}