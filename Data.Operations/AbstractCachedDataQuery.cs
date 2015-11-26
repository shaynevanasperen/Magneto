namespace Data.Operations
{
	public abstract class AbstractCachedDataQuery<TContext, TCachedResult> : CachedDataQueryBase<TCachedResult>
	{
		protected abstract TCachedResult Query(TContext context);

		protected virtual TCachedResult GetCachedResult(TContext context, ISyncDataQueryCache dataQueryCache = null, CacheOption cacheOption = CacheOption.Default)
		{
			if (dataQueryCache == null)
				return Query(context);

			var cacheInfo = GetCacheInfo();
			if (cacheInfo.Disabled)
				return Query(context);

			if (cacheOption == CacheOption.Default)
				return CachedResult = dataQueryCache.Get(() => Query(context), cacheInfo);

			CachedResult = Query(context);
            dataQueryCache.Refresh(CachedResult, cacheInfo);
			return CachedResult;
		}
	}
}