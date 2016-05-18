using System;
using System.Runtime.Caching;

namespace Data.Operations
{
	public interface ICacheInfo
	{
		/// <summary>
		/// A value to be combined with <see cref="VaryBy"/> to produce <see cref="CacheKey"/>.
		/// </summary>
		string CacheKeyPrefix { get; set; }

		/// <summary>
		/// This is formed from a combination of <see cref="CacheKeyPrefix"/> and <see cref="VaryBy"/>.
		/// </summary>
		string CacheKey { get; }

		/// <summary>
		/// The duration for which cached items should remain in the cache.
		/// </summary>
		//TimeSpan AbsoluteDuration { get; set; }

        /// <summary>
        /// The eviction and expiration details for caches items.
        /// </summary>
        CacheItemPolicy CacheItemPolicy { get; }

        /// <summary>
        /// An object or collection specifying values to be combined with <see cref="CacheKeyPrefix"/>
        /// to form the cache key.<br/>
        /// <c>VaryBy = new { Value1, Reference1 }</c><br/>
        /// <c>VaryBy = new[] { Value1, Value2 }</c><br/>
        /// <c>VaryBy = string.Format("{0}_{1}", Value1, Reference1.Id)</c><br/>
        /// <c>VaryBy = Value1</c>
        /// </summary>
        object VaryBy { get; set; }

		/// <summary>
		/// Specifies whether or not <c>null</c> items should be cached. Defaults to <c>true</c>.
		/// </summary>
		bool CacheNulls { get; set; }

		/// <summary>
		/// True if <c>AbsoluteDuration == TimeSpan.Zero</c>.
		/// </summary>
		bool Disabled { get; }
	}
}