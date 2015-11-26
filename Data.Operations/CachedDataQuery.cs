namespace Data.Operations
{
	public abstract class CachedDataQuery<TContext, TResult> : AbstractCachedDataQuery<TContext, TResult>, ICachedDataQuery<TContext, TResult, TResult>
	{
		public virtual TResult Execute(TContext context, ISyncDataQueryCache dataQueryCache = null, CacheOption cacheOption = CacheOption.Default)
		{
			return GetCachedResult(context, dataQueryCache, cacheOption);
		}
	}
}