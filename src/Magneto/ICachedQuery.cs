using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto
{
	public interface ISyncCachedQuery<in TContext, out TCacheEntryOptions, out TResult> : ISyncCachedQuery<TCacheEntryOptions>
	{
		TResult Execute(TContext context, ISyncDecorator decorator, ISyncQueryCache<TCacheEntryOptions> queryCache, CacheOption cacheOption = CacheOption.Default);
	}

	public interface IAsyncCachedQuery<in TContext, out TCacheEntryOptions, TResult> : IAsyncCachedQuery<TCacheEntryOptions>
	{
		Task<TResult> ExecuteAsync(TContext context, IAsyncDecorator decorator, IAsyncQueryCache<TCacheEntryOptions> queryCache, CacheOption cacheOption = CacheOption.Default);
	}

	public interface ISyncCachedQuery<out TCacheEntryOptions>
	{
		void EvictCachedResult(ISyncQueryCache<TCacheEntryOptions> queryCache);
		void UpdateCachedResult(ISyncQueryCache<TCacheEntryOptions> queryCache);
	}

	public interface IAsyncCachedQuery<out TCacheEntryOptions>
	{
		Task EvictCachedResultAsync(IAsyncQueryCache<TCacheEntryOptions> queryCache);
		Task UpdateCachedResultAsync(IAsyncQueryCache<TCacheEntryOptions> queryCache);
	}
}