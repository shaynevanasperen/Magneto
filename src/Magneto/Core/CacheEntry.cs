using System.ComponentModel;

namespace Magneto.Core
{
	public class CacheEntry<T>
	{
		public T Value { get; set; }
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class CacheEntryExtensions
	{
		public static CacheEntry<T> ToCacheEntry<T>(this T value) => new CacheEntry<T>
		{
			Value = value
		};
	}
}