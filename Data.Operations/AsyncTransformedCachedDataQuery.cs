using System.Threading.Tasks;

namespace Data.Operations
{
	public abstract class AsyncTransformedCachedDataQuery<TContext, TCachedResult, TTransformedResult>
		: AbstractAsyncCachedDataQuery<TContext, TCachedResult>, IAsyncCachedDataQuery<TContext, TCachedResult, TTransformedResult>
	{
		protected abstract Task<TTransformedResult> TransformCachedResultAsync(TCachedResult cachedResult);

		public virtual async Task<TTransformedResult> ExecuteAsync(TContext context, IAsyncDataQueryCache dataQueryCache = null, CacheOption cacheOption = CacheOption.Default)
		{
			return await TransformCachedResultAsync(await GetCachedResultAsync(context, dataQueryCache, cacheOption).ConfigureAwait(false))
				.ConfigureAwait(false);
		}
	}
}
