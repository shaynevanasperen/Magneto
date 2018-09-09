using System;
using System.Collections.Concurrent;

namespace Magneto.Core
{
	static class ConcurrentDictionaryExtensions
	{
		internal static T GetOrAdd<T>(this ConcurrentDictionary<Type, object> concurrentDictionary, Func<T> valueFactory) =>
			(T)concurrentDictionary.GetOrAdd(typeof(T), x => valueFactory());
	}
}
