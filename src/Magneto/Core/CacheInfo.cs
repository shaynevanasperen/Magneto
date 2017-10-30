using System;
using System.Collections.Generic;
using Code.Extensions.Object;

namespace Magneto.Core
{
	public class CacheInfo : ICacheInfo, ICacheConfig
	{
		readonly Func<string, object, string> _createKey;

		public CacheInfo(string keyPrefix, Func<string, object, string> createKey = null)
		{
			KeyPrefix = keyPrefix;
			_createKey = createKey ?? buildKey;
		}

		/// <inheritdoc cref="ICacheConfig.CacheNulls"/>
		public virtual bool CacheNulls { get; set; } = true;

		/// <inheritdoc />
		public string KeyPrefix
		{
			get => _keyPrefix;
			set
			{
				if (string.IsNullOrWhiteSpace(value))
					throw new Exception($"{nameof(KeyPrefix)} cannot be null or whitespace");
				_keyPrefix = value;
				_key = null;
			}
		}
		string _keyPrefix;

		/// <inheritdoc />
		public virtual object VaryBy
		{
			get => _varyBy;
			set
			{
				_varyBy = value;
				_key = null;
			}
		}
		object _varyBy;

		/// <inheritdoc />
		public virtual string Key => _key ?? (_key = _createKey(KeyPrefix, VaryBy));

		string buildKey(string keyPrefix, object varyBy)
		{
			if (VaryBy == null)
				return KeyPrefix;

			var segments = new List<object> { KeyPrefix };

			segments.AddRange(VaryBy.Flatten());

			return string.Join("_", segments);
		}
		string _key;
	}
}