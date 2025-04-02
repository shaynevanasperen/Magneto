using System;
using System.Threading;
using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto.Core;

/// <inheritdoc cref="ISyncCachedQuery{TCacheEntryOptions}"/>
public abstract class SyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult> : CachedQuery<TContext, TCacheEntryOptions, TCachedResult>, ISyncCachedQuery<TCacheEntryOptions>
{
	/// <inheritdoc cref="ISyncQuery{TContext,TResult}.Execute"/>
	protected abstract TCachedResult Query(TContext context);

	/// <inheritdoc cref="ISyncCachedQuery{TContext,TCacheEntryOptions,TResult}.Execute"/>
	protected virtual TCachedResult GetCachedResult(TContext context, ISyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption)
	{
		ArgumentNullException.ThrowIfNull(context);
		ArgumentNullException.ThrowIfNull(cacheStore);

		State.SetContext(context);

		if (cacheOption == CacheOption.Default)
		{
			var cacheEntry = cacheStore.GetEntry<TCachedResult>(State.CacheKey);
			if (cacheEntry != null)
				return State.SetCachedResult(cacheEntry.Value);
		}

		var result = Query(context);
		cacheStore.SetEntry(State.CacheKey, result.ToCacheEntry(), State.CacheEntryOptions);
		return State.SetCachedResult(result);
	}

	/// <inheritdoc cref="ISyncCachedQuery{TCacheEntryOptions}.EvictCachedResult"/>
	public virtual void EvictCachedResult(ISyncCacheStore<TCacheEntryOptions> cacheStore)
	{
		ArgumentNullException.ThrowIfNull(cacheStore);

		cacheStore.RemoveEntry(State.CacheKey);
	}

	/// <inheritdoc cref="ISyncCachedQuery{TCacheEntryOptions}.UpdateCachedResult"/>
	public virtual void UpdateCachedResult(ISyncCacheStore<TCacheEntryOptions> cacheStore)
	{
		ArgumentNullException.ThrowIfNull(cacheStore);

		cacheStore.SetEntry(State.CacheKey, State.CachedResult.ToCacheEntry(), State.CacheEntryOptions);
	}
}

/// <inheritdoc cref="IAsyncCachedQuery{TCacheEntryOptions}"/>
public abstract class AsyncCachedQuery<TContext, TCacheEntryOptions, TCachedResult> : CachedQuery<TContext, TCacheEntryOptions, TCachedResult>, IAsyncCachedQuery<TCacheEntryOptions>
{
	/// <inheritdoc cref="IAsyncQuery{TContext,TResult}.Execute"/>
	protected abstract Task<TCachedResult> Query(TContext context, CancellationToken cancellationToken);

	/// <inheritdoc cref="IAsyncCachedQuery{TContext,TCacheEntryOptions,TResult}.Execute"/>
	protected virtual async Task<TCachedResult> GetCachedResult(TContext context, IAsyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(context);
		ArgumentNullException.ThrowIfNull(cacheStore);

		State.SetContext(context);

		if (cacheOption == CacheOption.Default)
		{
			var cacheEntry = await cacheStore.GetEntryAsync<TCachedResult>(State.CacheKey, cancellationToken).ConfigureAwait(false);
			if (cacheEntry != null)
				return State.SetCachedResult(cacheEntry.Value);
		}

		var result = await Query(context, cancellationToken).ConfigureAwait(false);
		await cacheStore.SetEntryAsync(State.CacheKey, result.ToCacheEntry(), State.CacheEntryOptions, cancellationToken).ConfigureAwait(false);
		return State.SetCachedResult(result);
	}

	/// <inheritdoc cref="IAsyncCachedQuery{TCacheEntryOptions}.EvictCachedResult"/>
	public virtual Task EvictCachedResult(IAsyncCacheStore<TCacheEntryOptions> cacheStore, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(cacheStore);

		return cacheStore.RemoveEntryAsync(State.CacheKey, cancellationToken);
	}

	/// <inheritdoc cref="IAsyncCachedQuery{TCacheEntryOptions}.UpdateCachedResult"/>
	public virtual Task UpdateCachedResult(IAsyncCacheStore<TCacheEntryOptions> cacheStore, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(cacheStore);

		return cacheStore.SetEntryAsync(State.CacheKey, State.CachedResult.ToCacheEntry(), State.CacheEntryOptions, cancellationToken);
	}
}

/// <summary>
/// A root base class for building cached queries.
/// </summary>
/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the query.</typeparam>
/// <typeparam name="TCachedResult">The type of the query result.</typeparam>
public abstract class CachedQuery<TContext, TCacheEntryOptions, TCachedResult> : Operation
{
	/// <summary>
	/// Creates a new instance of <see cref="CachedQuery{TContext,TCacheEntryOptions,TCachedResult}"/> and initializes the internal state.
	/// </summary>
	protected CachedQuery() => State = new(this);

	internal readonly Store State;

	/// <summary>
	/// Configures details for constructing a cache key.
	/// </summary>
	/// <param name="key">The configuration object.</param>
	protected abstract void CacheKey(IKey key);

	/// <summary>
	/// <para>Returns options pertaining to the cache entry (such as expiration policy).</para>
	/// <para>Implementors must override this method in order to specify the behaviour of cache entries.</para>
	/// </summary>
	/// <param name="context">The context with which the query will execute.</param>
	/// <returns>The options pertaining to the cache entry.</returns>
	protected abstract TCacheEntryOptions CacheEntryOptions(TContext context);

	internal class Store
	{
		readonly CachedQuery<TContext, TCacheEntryOptions, TCachedResult> _query;
		
		internal Store(CachedQuery<TContext, TCacheEntryOptions, TCachedResult> query)
		{
			_query = query;
			SetContext(default!);
		}

		internal void SetContext(TContext context)
		{
			_hasCachedResult = false;
			_cacheKey = new(() => new CacheKey(_query.GetType().FullName).Configure(_query.CacheKey).Value);
			_cacheEntryOptions = new(() => _query.CacheEntryOptions(context));
		}

		Lazy<string> _cacheKey = null!;
		internal string CacheKey => _cacheKey.Value;

		Lazy<TCacheEntryOptions> _cacheEntryOptions = null!;
		internal TCacheEntryOptions CacheEntryOptions => _cacheEntryOptions.Value;

		internal TCachedResult SetCachedResult(TCachedResult value)
		{
			_cachedResult = value;
			_hasCachedResult = true;
			return value;
		}

		bool _hasCachedResult;
		TCachedResult? _cachedResult;
		internal TCachedResult? CachedResult => !_hasCachedResult
			? throw new InvalidOperationException("Cached result is not available")
			: _cachedResult;
	}
}
