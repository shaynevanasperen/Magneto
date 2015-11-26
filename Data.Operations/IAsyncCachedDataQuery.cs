using System.Threading.Tasks;

namespace Data.Operations
{
	public interface IAsyncCachedDataQuery<in TContext, out TCachedResult, TResult> : ICachedDataQueryBase<TCachedResult>
	{
		Task<TResult> ExecuteAsync(TContext context, IAsyncDataQueryCache dataQueryCache = null, CacheOption cacheOption = CacheOption.Default);
	}
}
