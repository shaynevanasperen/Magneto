using System;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace Magneto.Core;

[EditorBrowsable(EditorBrowsableState.Never)]
static class ConcurrentDictionaryExtensions
{
	internal static T GetOrAdd<T>(this ConcurrentDictionary<Type, object> concurrentDictionary, Func<T> valueFactory) =>
		(T)concurrentDictionary.GetOrAdd(typeof(T), _ => valueFactory()!);
}
