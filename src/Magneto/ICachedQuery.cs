using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto
{
	public interface ISyncCachedQuery<in TContext, out TCacheEntryOptions, out TResult> : ISyncCachedQuery<TCacheEntryOptions>
	{
		TResult Execute(TContext context, ISyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption = CacheOption.Default);
	}

	public interface IAsyncCachedQuery<in TContext, out TCacheEntryOptions, TResult> : IAsyncCachedQuery<TCacheEntryOptions>
	{
		Task<TResult> ExecuteAsync(TContext context, IAsyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption = CacheOption.Default);
	}

	public interface ISyncCachedQuery<out TCacheEntryOptions>
	{
		void EvictCachedResult(ISyncCacheStore<TCacheEntryOptions> cacheStore);
		void UpdateCachedResult(ISyncCacheStore<TCacheEntryOptions> cacheStore);
	}

	public interface IAsyncCachedQuery<out TCacheEntryOptions>
	{
		Task EvictCachedResultAsync(IAsyncCacheStore<TCacheEntryOptions> cacheStore);
		Task UpdateCachedResultAsync(IAsyncCacheStore<TCacheEntryOptions> cacheStore);
	}
}