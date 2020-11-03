using System;
using System.ComponentModel;
using System.Linq;

namespace Magneto
{
	/// <summary>
	/// Configures values for constructing a cache key.
	/// </summary>
	public interface IKeyConfig
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
		/// keyConfig.VaryBy = Foo;<br/>
		/// keyConfig.VaryBy = (Foo, Bar);<br/>
		/// keyConfig.VaryBy = new { Foo, Bar };<br/>
		/// keyConfig.VaryBy = new object[] { Foo, Bar };<br/>
		/// keyConfig.VaryBy = $"{Foo}_{Baz.Id}";<br/>
		/// </para>
		/// </summary>
		object VaryBy { get; set; }
	}

	/// <summary>
	/// An extension class for fluent configuration of <see cref="IKeyConfig"/> instances.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class KeyConfigExtensions
	{
		/// <summary>
		/// A value to be combined with <see cref="IKeyConfig.VaryBy"/> to form the cache key.
		/// Defaults to the fully qualified type name of the query class.
		/// </summary>
		public static IKeyConfig UsePrefix(this IKeyConfig keyConfig, string value)
		{
			if (keyConfig == null) throw new ArgumentNullException(nameof(keyConfig));
			keyConfig.Prefix = value;
			return keyConfig;
		}

		/// <summary>
		/// <para>
		/// An object or collection specifying values to be combined with <see cref="IKeyConfig.Prefix"/>
		/// to form the cache key.
		/// </para>
		/// <para>
		/// Examples:<br/>
		/// keyConfig.VaryBy(Foo)<br/>
		/// keyConfig.VaryBy(Foo, Bar)<br/>
		/// keyConfig.VaryBy(new { Foo, Bar })<br/>
		/// keyConfig.VaryBy($"{Foo}_{Baz.Id}")<br/>
		/// </para>
		/// </summary>
		public static IKeyConfig VaryBy(this IKeyConfig keyConfig, object firstValue, params object[] additionalValues)
		{
			if (keyConfig == null) throw new ArgumentNullException(nameof(keyConfig));
			keyConfig.VaryBy = new[] { firstValue }.Concat(additionalValues);
			return keyConfig;
		}

		/// <summary>
		/// A convenience method to express that the cache key should not vary by anything.
		/// </summary>
		public static IKeyConfig VaryByNothing(this IKeyConfig keyConfig)
		{
			if (keyConfig == null) throw new ArgumentNullException(nameof(keyConfig));
			return keyConfig;
		}
	}
}
