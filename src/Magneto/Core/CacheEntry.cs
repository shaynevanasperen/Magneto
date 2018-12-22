using System.ComponentModel;

namespace Magneto.Core
{
	/// <summary>
	/// A wrapper class for holding cached values (so that we can cache nulls).
	/// </summary>
	/// <typeparam name="T">The type of value being cached.</typeparam>
	public class CacheEntry<T>
	{
		/// <summary>
		/// The cached value.
		/// </summary>
		public T Value { get; set; }
	}

	/// <summary>
	/// An extension class for creating a <see cref="CacheEntry{T}"/> from a value.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class CacheEntryExtensions
	{
		/// <summary>
		/// An extension method for creating an instance of <see cref="CacheEntry{T}"/> wrapping the given value.
		/// </summary>
		/// <param name="value">The value to be wrapped.</param>
		/// <typeparam name="T">The type of value being wrapped.</typeparam>
		/// <returns>An instance of <see cref="CacheEntry{T}"/> containing the value.</returns>
		public static CacheEntry<T> ToCacheEntry<T>(this T value) => new CacheEntry<T>
		{
			Value = value
		};
	}
}
