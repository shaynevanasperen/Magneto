namespace Data.Operations
{
	public abstract class TransformedCachedDataQuery<TContext, TCachedResult, TTransformedResult>
		: AbstractCachedDataQuery<TContext, TCachedResult>, ICachedDataQuery<TContext, TCachedResult, TTransformedResult>
	{
		protected abstract TTransformedResult TransformCachedResult(TCachedResult cachedResult);

		public virtual TTransformedResult Execute(TContext context, ISyncDataQueryCache dataQueryCache = null, CacheOption cacheOption = CacheOption.Default)
		{
			return TransformCachedResult(GetCachedResult(context, dataQueryCache, cacheOption));
		}
	}
}