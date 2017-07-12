using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto.Configuration
{
	public interface ICacheStore<in TCacheEntryOptions> : ISyncCacheStore<TCacheEntryOptions>, IAsyncCacheStore<TCacheEntryOptions> { }

	public interface ISyncCacheStore<in TCacheEntryOptions>
	{
		CacheEntry<T> Get<T>(string key);
		void Set<T>(string key, CacheEntry<T> item, TCacheEntryOptions cacheEntryOptions);
		void Remove(string key);
	}

	public interface IAsyncCacheStore<in TCacheEntryOptions>
	{
		Task<CacheEntry<T>> GetAsync<T>(string key);
		Task SetAsync<T>(string key, CacheEntry<T> item, TCacheEntryOptions cacheEntryOptions);
		Task RemoveAsync(string key);
	}
}