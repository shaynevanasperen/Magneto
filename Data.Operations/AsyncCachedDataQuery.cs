using System.Threading.Tasks;

namespace Data.Operations
{
	public abstract class AsyncCachedDataQuery<TContext, TResult>
		: AbstractAsyncCachedDataQuery<TContext, TResult>, IAsyncCachedDataQuery<TContext, TResult, TResult>
	{
		public virtual Task<TResult> ExecuteAsync(TContext context, IAsyncDataQueryCache dataQueryCache = null, CacheOption cacheOption = CacheOption.Default)
		{
			return GetCachedResultAsync(context, dataQueryCache, cacheOption);
		}
	}
}
