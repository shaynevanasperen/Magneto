using System.Threading.Tasks;

namespace Data.Operations
{
	public abstract class AbstractAsyncCachedDataQuery<TContext, TCachedResult> : CachedDataQueryBase<TCachedResult>
	{
		protected abstract Task<TCachedResult> QueryAsync(TContext context);

		protected virtual async Task<TCachedResult> GetCachedResultAsync(TContext context, IAsyncDataQueryCache dataQueryCache = null, CacheOption cacheOption = CacheOption.Default)
		{
			if (dataQueryCache == null)
				return await QueryAsync(context).ConfigureAwait(false);

			var cacheInfo = GetCacheInfo();
			if (cacheInfo.Disabled)
				return await QueryAsync(context).ConfigureAwait(false);

			if (cacheOption == CacheOption.Default)
				return CachedResult = await dataQueryCache.GetAsync(() => QueryAsync(context), cacheInfo).ConfigureAwait(false);

			CachedResult = await QueryAsync(context).ConfigureAwait(false);
			await dataQueryCache.RefreshAsync(CachedResult, cacheInfo).ConfigureAwait(false);
			return CachedResult;
		}
	}
}
