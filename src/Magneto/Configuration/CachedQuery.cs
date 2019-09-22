using System;
using Code.Extensions.Object;
using Magneto.Core;

namespace Magneto.Configuration
{
	/// <summary>
	/// A class used to configure how cache keys are created.
	/// </summary>
	public static class CachedQuery
	{
		/// <summary>
		/// Specifies a method for creating a cache key from a prefix and and an object representing query parameters.
		/// </summary>
		/// <param name="createKey">The method to use for creating cache keys.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="createKey"/> is null.</exception>
		public static void UseKeyCreator(Func<string, object, string> createKey)
		{
			if (createKey == null) throw new ArgumentNullException(nameof(createKey));

			KeyConfig.CreateKey = createKey;
		}

		/// <summary>
		/// The default method of creating cache keys. Uses reflection to serialize the <paramref name="varyBy"/> argument.
		/// </summary>
		/// <param name="prefix">The prefix to be used for the resultant key.</param>
		/// <param name="varyBy">The object to serialize into a string that will be appended to <paramref name="prefix"/>.</param>
		/// <returns></returns>
		public static string DefaultKeyCreator(string prefix, object varyBy) => varyBy == null
			? prefix
			: $"{prefix}_{string.Join("_", varyBy.Flatten())}";
	}
}
