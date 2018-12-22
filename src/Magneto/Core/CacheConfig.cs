using System;
using Code.Extensions.Object;

namespace Magneto.Core
{
	/// <inheritdoc />
	public class CacheConfig : ICacheConfig
	{
		string _key;
		string _keyPrefix;
		object _varyBy;
		readonly Func<string, object, string> _createKey;

		/// <summary>
		/// Create a new instance of <see cref="CacheConfig"/> with the given key prefix and an optional callback for creating the key.
		/// </summary>
		/// <param name="keyPrefix">The prefix to use for generated keys (when not given <paramref name="createKey"/> callback).</param>
		/// <param name="createKey">An optional callback used to create the key (used for testing purposes).</param>
		public CacheConfig(string keyPrefix, Func<string, object, string> createKey = null)
		{
			KeyPrefix = keyPrefix;
			_createKey = createKey ?? buildKey;
		}

		static string buildKey(string keyPrefix, object varyBy)
		{
			return varyBy == null
				? keyPrefix
				: $"{keyPrefix}_{string.Join("_", varyBy.Flatten())}";
		}

		/// <summary>
		/// The key that is generated from a combination of the <see cref="KeyPrefix"/> and <see cref="VaryBy"/>.
		/// </summary>
		public string Key => _key ?? (_key = _createKey(KeyPrefix, VaryBy));

		/// <inheritdoc />
		public string KeyPrefix
		{
			get => _keyPrefix;
			set
			{
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentException($"{nameof(KeyPrefix)} cannot be null or whitespace");
				_keyPrefix = value;
				_key = null;
			}
		}

		/// <inheritdoc />
		public object VaryBy
		{
			get => _varyBy;
			set
			{
				_varyBy = value;
				_key = null;
			}
		}
	}
}
