using System.Threading;
using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto.Core;

class NullCacheStore<TCacheEntryOptions> : ICacheStore<TCacheEntryOptions>
{
	public CacheEntry<T>? GetEntry<T>(string key) => null;

	public Task<CacheEntry<T>?> GetEntryAsync<T>(string key, CancellationToken cancellationToken) => Task.FromResult<CacheEntry<T>?>(null);

	public void SetEntry<T>(string key, CacheEntry<T> item, TCacheEntryOptions cacheEntryOptions) { }

	public Task SetEntryAsync<T>(string key, CacheEntry<T> item, TCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken) => Task.CompletedTask;

	public void RemoveEntry(string key) { }

	public Task RemoveEntryAsync(string key, CancellationToken cancellationToken) => Task.CompletedTask;
}
