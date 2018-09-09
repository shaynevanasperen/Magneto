using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto.Core
{
	class NullCacheStore<TCacheEntryOptions> : ICacheStore<TCacheEntryOptions>
	{
		public CacheEntry<T> Get<T>(string key) => null;

		public Task<CacheEntry<T>> GetAsync<T>(string key) => Task.FromResult<CacheEntry<T>>(null);

		public void Set<T>(string key, CacheEntry<T> item, TCacheEntryOptions cacheEntryOptions) { }

		public Task SetAsync<T>(string key, CacheEntry<T> item, TCacheEntryOptions cacheEntryOptions) => Task.CompletedTask;

		public void Remove(string key) { }

		public Task RemoveAsync(string key) => Task.CompletedTask;
	}
}
