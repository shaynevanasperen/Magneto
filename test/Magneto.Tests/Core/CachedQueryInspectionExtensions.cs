using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Magneto.Core;

namespace Magneto.Tests.Core;

[EditorBrowsable(EditorBrowsableState.Never)]
static class CachedQueryInspectionExtensions
{
	public static string PeekCacheKey<TContext, TCacheEntryOptions, TCachedResult>(this CachedQuery<TContext, TCacheEntryOptions, TCachedResult> cachedQuery) =>
		cachedQuery.GetStateProperty<string>("CacheKey");

	public static TCacheEntryOptions PeekCacheEntryOptions<TContext, TCacheEntryOptions, TCachedResult>(this CachedQuery<TContext, TCacheEntryOptions, TCachedResult> cachedQuery) =>
		cachedQuery.GetStateProperty<TCacheEntryOptions>("CacheEntryOptions");

	public static TCachedResult PeekCachedResult<TContext, TCacheEntryOptions, TCachedResult>(this CachedQuery<TContext, TCacheEntryOptions, TCachedResult> cachedQuery) =>
		cachedQuery.GetStateProperty<TCachedResult>("CachedResult");

	static T GetStateProperty<T>(this object cachedQuery, string name)
	{
		var stateProperty = cachedQuery.GetType().GetField("State", BindingFlags.Instance | BindingFlags.NonPublic);
		Debug.Assert(stateProperty != null, nameof(stateProperty) + " != null");
		var state = stateProperty.GetValue(cachedQuery);
		Debug.Assert(state != null, nameof(state) + " != null");
		var type = state.GetType();
		var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);
		Debug.Assert(property != null, nameof(property) + " != null");
		return (T)property.GetValue(state)!;
	}
}
