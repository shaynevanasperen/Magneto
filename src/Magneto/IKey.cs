using System;
using System.ComponentModel;
using System.Linq;

namespace Magneto;

/// <summary>
/// Configures values for constructing a cache key.
/// </summary>
public interface IKey
{
	/// <summary>
	/// A value to be combined with <see cref="VaryBy"/> to form the cache key.
	/// Defaults to the fully qualified type name of the query class.
	/// </summary>
	string? Prefix { get; set; }

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
	object? VaryBy { get; set; }
}

/// <summary>
/// An extension class for fluent configuration of <see cref="IKey"/> instances.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class KeyExtensions
{
	/// <summary>
	/// A value to be combined with <see cref="IKey.VaryBy"/> to form the cache key.
	/// Defaults to the fully qualified type name of the query class.
	/// </summary>
	public static IKey UsePrefix(this IKey key, string value)
	{
		ArgumentNullException.ThrowIfNull(key);
		key.Prefix = value;
		return key;
	}

	/// <summary>
	/// <para>
	/// An object or collection specifying values to be combined with <see cref="IKey.Prefix"/>
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
	public static IKey VaryBy(this IKey key, object firstValue, params object[] additionalValues)
	{
		ArgumentNullException.ThrowIfNull(key);
		key.VaryBy = new[] { firstValue }.Concat(additionalValues);
		return key;
	}

	/// <summary>
	/// A convenience method to express that the cache key should not vary by anything.
	/// </summary>
	public static IKey VaryByNothing(this IKey key)
	{
		ArgumentNullException.ThrowIfNull(key);
		return key;
	}
}
