using System.Threading;
using System.Threading.Tasks;
using Magneto.Configuration;
using Magneto.Core;

namespace Magneto
{
	/// <summary>
	/// <para>A base class for synchronous queries which can have their results cached.</para>
	/// <para>Implementors can optionally override <see cref="CachedQuery{TContext,TCacheEntryOptions,TCachedResult}.CacheKey"/>
	/// in order to specify a custom cache key prefix and/or which values should be used to construct a cache key.</para>
	/// <para>Implementors must override <see cref="CachedQuery{TContext,TCacheEntryOptions,TCachedResult}.CacheEntryOptions"/> in order to specify options for cache entries (such as expiration policy).</para>
	/// <para>Implementors must override <see cref="Core.SyncCachedQuery{TContext,TCacheEntryOptions,TResult}.Query"/> in order to define how the query is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the query.</typeparam>
	/// <typeparam name="TResult">The type of the query result.</typeparam>
	public abstract class SyncCachedQuery<TContext, TCacheEntryOptions, TResult> :
		Core.SyncCachedQuery<TContext, TCacheEntryOptions, TResult>, ISyncCachedQuery<TContext, TCacheEntryOptions, TResult>
	{
		/// <inheritdoc cref="ISyncCachedQuery{TContext,TCacheEntryOptions,TResult}.Execute"/>
		public virtual TResult Execute(TContext context, ISyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption = CacheOption.Default) =>
			GetCachedResult(context, cacheStore, cacheOption);
	}

	/// <summary>
	/// <para>A base class for synchronous queries which can have intermediate results cached and then transformed into final results.</para>
	/// <para>Implementors can optionally override <see cref="CachedQuery{TContext,TCacheEntryOptions,TCachedResult}.CacheKey"/>
	/// in order to specify a custom cache key prefix and/or which values should be used to construct a cache key.</para>
	/// <para>Implementors must override <see cref="CachedQuery{TContext,TCacheEntryOptions,TCachedResult}.CacheEntryOptions"/> in order to specify options for cache entries (such as expiration policy).</para>
	/// <para>Implementors must override <see cref="Core.SyncCachedQuery{TContext,TCacheEntryOptions,TResult}.Query"/> in order to define how the query is executed to obtain the intermediate result.</para>
	/// <para>Implementors must override <see cref="TransformCachedResult"/> in order to define how the intermediate result is transformed into a final result.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the query.</typeparam>
	/// <typeparam name="TCachedResult">The type of the intermediate result (which will be what gets cached).</typeparam>
	/// <typeparam name="TTransformedResult">The type of the final result (transformed from the intermediate result).</typeparam>
	public abstract class SyncTransformedCachedQuery<TContext, TCacheEntryOptions, TCachedResult, TTransformedResult> :
		Core.SyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult>, ISyncCachedQuery<TContext, TCacheEntryOptions, TTransformedResult>
	{
		/// <summary>
		/// Transforms an intermediate result to a final result.
		/// </summary>
		/// <param name="cachedResult">The intermediate result to be transformed.</param>
		/// <returns>The result of transforming the intermediate result.</returns>
		protected abstract TTransformedResult TransformCachedResult(TCachedResult cachedResult);

		/// <inheritdoc cref="ISyncCachedQuery{TContext,TCacheEntryOptions,TResult}.Execute"/>
		public virtual TTransformedResult Execute(TContext context, ISyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption = CacheOption.Default)
		{
			var cachedResult = GetCachedResult(context, cacheStore, cacheOption);
			return TransformCachedResult(cachedResult);
		}
	}

	/// <summary>
	/// <para>A base class for asynchronous queries which can have their results cached.</para>
	/// <para>Implementors can optionally override <see cref="CachedQuery{TContext,TCacheEntryOptions,TCachedResult}.CacheKey"/>
	/// in order to specify a custom cache key prefix and/or which values should be used to construct a cache key.</para>
	/// <para>Implementors must override <see cref="CachedQuery{TContext,TCacheEntryOptions,TCachedResult}.CacheEntryOptions"/> in order to specify options
	/// for cache entries (such as expiration policy).</para>
	/// <para>Implementors must override <see cref="Core.AsyncCachedQuery{TContext,TCacheEntryOptions,TResult}.Query"/> in order to define how the query is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the query.</typeparam>
	/// <typeparam name="TResult">The type of the query result.</typeparam>
	public abstract class AsyncCachedQuery<TContext, TCacheEntryOptions, TResult> :
		Core.AsyncCachedQuery<TContext, TCacheEntryOptions, TResult>, IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult>
	{
		/// <inheritdoc cref="IAsyncCachedQuery{TContext,TCacheEntryOptions,TResult}.Execute"/>
		public virtual Task<TResult> Execute(TContext context, IAsyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption = CacheOption.Default, CancellationToken cancellationToken = default) =>
			GetCachedResult(context, cacheStore, cacheOption, cancellationToken);
	}

	/// <summary>
	/// <para>A base class for asynchronous queries which can have intermediate results cached and then transformed into final results.</para>
	/// <para>Implementors can optionally override <see cref="CachedQuery{TContext,TCacheEntryOptions,TCachedResult}.CacheKey"/>
	/// in order to specify a custom cache key prefix and/or which values should be used to construct a cache key.</para>
	/// <para>Implementors must override <see cref="CachedQuery{TContext,TCacheEntryOptions,TCachedResult}.CacheEntryOptions"/> in order to specify options for cache entries (such as expiration policy).</para>
	/// <para>Implementors must override <see cref="Core.AsyncCachedQuery{TContext,TCacheEntryOptions,TResult}.Query"/> in order to define how the query is executed to obtain the intermediate result.</para>
	/// <para>Implementors must override <see cref="TransformCachedResult"/> in order to define how the intermediate result is transformed into a final result.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the query.</typeparam>
	/// <typeparam name="TCachedResult">The type of the intermediate result (which will be what gets cached).</typeparam>
	/// <typeparam name="TTransformedResult">The type of the final result (transformed from the intermediate result).</typeparam>
	public abstract class AsyncTransformedCachedQuery<TContext, TCacheEntryOptions, TCachedResult, TTransformedResult> :
		Core.AsyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult>, IAsyncCachedQuery<TContext, TCacheEntryOptions, TTransformedResult>
	{
		/// <summary>
		/// Transforms an intermediate result to a final result.
		/// </summary>
		/// <param name="cachedResult">The intermediate result to be transformed.</param>
		/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
		/// <returns>The result of transforming the intermediate result.</returns>
		protected abstract Task<TTransformedResult> TransformCachedResult(TCachedResult cachedResult, CancellationToken cancellationToken = default);

		/// <inheritdoc cref="IAsyncCachedQuery{TContext,TCacheEntryOptions,TResult}.Execute"/>
		public virtual async Task<TTransformedResult> Execute(TContext context, IAsyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption = CacheOption.Default, CancellationToken cancellationToken = default)
		{
			var cachedResult = await GetCachedResult(context, cacheStore, cacheOption, cancellationToken).ConfigureAwait(false);
			return await TransformCachedResult(cachedResult, cancellationToken).ConfigureAwait(false);
		}
	}
}
