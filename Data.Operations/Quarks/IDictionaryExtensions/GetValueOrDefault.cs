using System;
using System.Collections.Generic;

namespace Quarks.IDictionaryExtensions
{
	static partial class DictionaryExtension
	{
		internal static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			TValue value;
			dictionary.TryGetValue(key, out value);
			return value;
		}

		internal static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue, bool cacheDefault = false)
		{
			TValue value;
			return !dictionary.TryGetValue(key, out value)
				? defaultValue
				: value;
		}

		internal static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> createDefaultValue)
		{
			TValue value;
			if (dictionary.TryGetValue(key, out value))
				return value;
			value = createDefaultValue();
			dictionary.Add(key, value);
			return value;
		}
	}
}
