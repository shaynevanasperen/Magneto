using System;
using System.ComponentModel;
using System.Linq;

namespace Magneto
{
	/// <summary>
	/// Configures values for constructing a cache key.
	/// </summary>
	public interface ICache
	{
		/// <summary>
		/// A value to be combined with <see cref="VaryBy"/> to form the cache key.
		/// Defaults to the fully qualified type name of the query class.
		/// </summary>
		string Prefix { get; set; }

		/// <summary>
		/// <para>
		/// An object or collection specifying values to be combined with <see cref="Prefix"/>
		/// to form the cache key.
		/// </para>
		/// <para>
		/// Examples:<br/>
		/// cache.VaryBy = Foo;<br/>
		/// cache.VaryBy = (Foo, Bar);<br/>
		/// cache.VaryBy = new { Foo, Bar };<br/>
		/// cache.VaryBy = new object[] { Foo, Bar };<br/>
		/// cache.VaryBy = $"{Foo}_{Baz.Id}";<br/>
		/// </para>
		/// </summary>
		object VaryBy { get; set; }
	}

	/// <summary>
	/// An extension class for fluent configuration of <see cref="ICache"/> instances.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class CacheExtensions
	{
		/// <summary>
		/// A value to be combined with <see cref="ICache.VaryBy"/> to form the cache key.
		/// Defaults to the fully qualified type name of the query class.
		/// </summary>
		public static ICache UsePrefix(this ICache cache, string value)
		{
			if (cache == null) throw new ArgumentNullException(nameof(cache));
			cache.Prefix = value;
			return cache;
		}

		/// <summary>
		/// <para>
		/// An object or collection specifying values to be combined with <see cref="ICache.Prefix"/>
		/// to form the cache key.
		/// </para>
		/// <para>
		/// Examples:<br/>
		/// cache.VaryBy(Foo)<br/>
		/// cache.VaryBy(Foo, Bar)<br/>
		/// cache.VaryBy(new { Foo, Bar })<br/>
		/// cache.VaryBy($"{Foo}_{Baz.Id}")<br/>
		/// </para>
		/// </summary>
		public static ICache VaryBy(this ICache cache, object firstValue, params object[] additionalValues)
		{
			if (cache == null) throw new ArgumentNullException(nameof(cache));
			cache.VaryBy = new[] { firstValue }.Concat(additionalValues);
			return cache;
		}

		/// <summary>
		/// A convenience method to express that the cache key should not vary by anything.
		/// </summary>
		public static ICache VaryByNothing(this ICache cache)
		{
			if (cache == null) throw new ArgumentNullException(nameof(cache));
			return cache;
		}
	}
}
