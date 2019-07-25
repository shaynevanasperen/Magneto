using System.Reflection;
using Magneto.Core;

namespace Magneto.Tests.Core
{
	public static class CachedQueryExtensions
	{
		public static string GetCacheKey<TContext, TCacheEntryOptions, TCachedResult>(this CachedQuery<TContext, TCacheEntryOptions, TCachedResult> cachedQuery)
		{
			return cachedQuery.getStateProperty<string>("CacheKey");
		}

		public static TCachedResult GetCachedResult<TContext, TCacheEntryOptions, TCachedResult>(this CachedQuery<TContext, TCacheEntryOptions, TCachedResult> cachedQuery)
		{
			return cachedQuery.getStateProperty<TCachedResult>("CachedResult");
		}

		static T getStateProperty<T>(this object cachedQuery, string name)
		{
			var stateProperty = cachedQuery.GetType().GetField("State", BindingFlags.Instance | BindingFlags.NonPublic);
			var state = stateProperty.GetValue(cachedQuery);
			var type = state.GetType();
			var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);
			return (T)property.GetValue(state);
		}
	}
}
