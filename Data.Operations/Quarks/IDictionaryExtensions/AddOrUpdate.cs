using System.Collections.Generic;

namespace Quarks.IDictionaryExtensions
{
	static partial class DictionaryExtension
	{
		internal static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			if (dictionary.ContainsKey(key))
				dictionary[key] = value;
			else
				dictionary.Add(key, value);
		}
	}
}
