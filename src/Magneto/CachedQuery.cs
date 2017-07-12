using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto
{
	public abstract class SyncCachedQuery<TContext, TCacheEntryOptions, TResult> :
		Core.SyncCachedQuery<TContext, TCacheEntryOptions, TResult>, ISyncCachedQuery<TContext, TCacheEntryOptions, TResult>
	{
		public virtual TResult Execute(TContext context, ISyncDecorator decorator, ISyncQueryCache<TCacheEntryOptions> queryCache, CacheOption cacheOption = CacheOption.Default) =>
			GetCachedResult(context, decorator, queryCache, cacheOption);
	}

	public abstract class SyncTransformedCachedQuery<TContext, TCacheEntryOptions, TCachedResult, TTransformedResult> :
		Core.SyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult>, ISyncCachedQuery<TContext, TCacheEntryOptions, TTransformedResult>
	{
		protected abstract TTransformedResult TransformCachedResult(TCachedResult cachedResult);

		public virtual TTransformedResult Execute(TContext context, ISyncDecorator decorator, ISyncQueryCache<TCacheEntryOptions> queryCache, CacheOption cacheOption = CacheOption.Default)
		{
			var cachedResult = GetCachedResult(context, decorator, queryCache, cacheOption);
			return TransformCachedResult(cachedResult);
		}
	}

	public abstract class AsyncCachedQuery<TContext, TCacheEntryOptions, TResult> :
		Core.AsyncCachedQuery<TContext, TCacheEntryOptions, TResult>, IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult>
	{
		public virtual Task<TResult> ExecuteAsync(TContext context, IAsyncDecorator decorator, IAsyncQueryCache<TCacheEntryOptions> queryCache, CacheOption cacheOption = CacheOption.Default) =>
			GetCachedResultAsync(context, decorator, queryCache, cacheOption);
	}

	public abstract class AsyncTransformedCachedQuery<TContext, TCacheEntryOptions, TCachedResult, TTransformedResult> :
		Core.AsyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult>, IAsyncCachedQuery<TContext, TCacheEntryOptions, TTransformedResult>
	{
		protected abstract Task<TTransformedResult> TransformCachedResultAsync(TCachedResult cachedResult);

		public virtual async Task<TTransformedResult> ExecuteAsync(TContext context, IAsyncDecorator decorator, IAsyncQueryCache<TCacheEntryOptions> queryCache, CacheOption cacheOption = CacheOption.Default)
		{
			var cachedResult = await GetCachedResultAsync(context, decorator, queryCache, cacheOption).ConfigureAwait(false);
			return await TransformCachedResultAsync(cachedResult).ConfigureAwait(false);
		}
	}
}
