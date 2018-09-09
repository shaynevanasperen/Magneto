using System;
using Code.Extensions.Object;

namespace Magneto.Core
{
	public class CacheConfig : ICacheConfig
	{
		string _key;
		string _keyPrefix;
		object _varyBy;
		readonly Func<string, object, string> _createKey;

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