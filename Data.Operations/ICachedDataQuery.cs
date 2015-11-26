namespace Data.Operations
{
	public interface ICachedDataQuery<in TContext, out TCachedResult, out TResult> : ICachedDataQueryBase<TCachedResult>
	{
		TResult Execute(TContext context, ISyncDataQueryCache dataQueryCache = null, CacheOption cacheOption = CacheOption.Default);
	}
}