namespace Magneto
{
	/// <summary>
	/// Configures details for constructing a cache key and whether or not <c>null</c> values should be cached.
	/// </summary>
	public interface ICacheConfig
	{
		/// <summary>
		/// A value to be combined with <see cref="VaryBy"/> to form the cache key.
		/// Defaults to the fully qualified type name of the query class.
		/// </summary>
		string KeyPrefix { set; }

		/// <summary>
		/// An object or collection specifying values to be combined with <see cref="KeyPrefix"/>
		/// to form the cache key.<br/>
		/// Examples:<br/>
		/// <c>VaryBy = new { Value1, Reference1 }</c><br/>
		/// <c>VaryBy = new[] { Value1, Value2 }</c><br/>
		/// <c>VaryBy = string.Format("{0}_{1}", Value1, Reference1.Id)</c><br/>
		/// <c>VaryBy = Value1</c>
		/// </summary>
		object VaryBy { set; }
	}

	public static class CacheConfigExtensions
	{
		/// <summary>
		/// A value to be combined with <see cref="ICacheConfig.VaryBy"/> to form the cache key.
		/// Defaults to the fully qualified type name of the query class.
		/// </summary>
		public static ICacheConfig UseKeyPrefix(this ICacheConfig cacheConfig, string value)
		{
			cacheConfig.KeyPrefix = value;
			return cacheConfig;
		}

		/// <summary>
		/// An object or collection specifying values to be combined with <see cref="ICacheConfig.KeyPrefix"/>
		/// to form the cache key.<br/>
		/// Examples:<br/>
		/// <c>VaryBy(new { Value1, Reference1 })</c><br/>
		/// <c>VaryBy(new[] { Value1, Value2 })</c><br/>
		/// <c>VaryBy(string.Format("{0}_{1}", Value1, Reference1.Id))</c><br/>
		/// <c>VaryBy(Value1)</c>
		/// </summary>
		public static ICacheConfig VaryBy(this ICacheConfig cacheConfig, object value)
		{
			cacheConfig.VaryBy = value;
			return cacheConfig;
		}
	}
}