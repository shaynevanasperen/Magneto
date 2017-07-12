using System;
using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto.Configuration
{
	public interface IQueryCache<in TCacheEntryOptions> : ISyncQueryCache<TCacheEntryOptions>, IAsyncQueryCache<TCacheEntryOptions> { }

	public interface ISyncQueryCache<in TCacheEntryOptions>
	{
		T Get<T>(Func<T> executeQuery, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions);
		void Set<T>(T queryResult, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions);
		void Evict(string key);
	}

	public interface IAsyncQueryCache<in TCacheEntryOptions>
	{
		Task<T> GetAsync<T>(Func<Task<T>> executeQueryAsync, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions);
		Task SetAsync<T>(T queryResult, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions);
		Task EvictAsync(string key);
	}
}