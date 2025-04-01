using System;
using System.Threading;
using System.Threading.Tasks;
using Magneto.Core;
using Microsoft.Extensions.Caching.Distributed;

namespace Magneto.Configuration;

/// <summary>
/// An implementation of <see cref="ICacheStore{TCacheEntryOptions}"/> backed by <see cref="IDistributedCache"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="DistributedCacheStore"/> using the given <see cref="IDistributedCache"/>.
/// </remarks>
/// <param name="distributedCache">The underlying cache implementation.</param>
/// <param name="stringSerializer">The serializer to use for cached objects.</param>
public class DistributedCacheStore(IDistributedCache distributedCache, IStringSerializer stringSerializer) : ICacheStore<DistributedCacheEntryOptions>
{
	readonly IDistributedCache _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
	readonly IStringSerializer _stringSerializer = stringSerializer ?? throw new ArgumentNullException(nameof(stringSerializer));

	/// <inheritdoc cref="ISyncCacheStore{TCacheEntryOptions}.GetEntry{T}"/>
	public CacheEntry<T>? GetEntry<T>(string key)
	{
		ArgumentNullException.ThrowIfNull(key);

		var value = _distributedCache.GetString(key);
		if (value == null)
			return null;

		try
		{
			return _stringSerializer.Deserialize<CacheEntry<T>>(value);
		}
		catch
		{
			_distributedCache.Remove(key);
			throw;
		}
	}

	/// <inheritdoc cref="IAsyncCacheStore{TCacheEntryOptions}.GetEntryAsync{T}"/>
	public async Task<CacheEntry<T>?> GetEntryAsync<T>(string key, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(key);

		var value = await _distributedCache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);
		if (value == null)
			return null;

		try
		{
			return _stringSerializer.Deserialize<CacheEntry<T>>(value);
		}
		catch
		{
			await _distributedCache.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
			throw;
		}
	}

	/// <inheritdoc cref="ISyncCacheStore{TCacheEntryOptions}.SetEntry{T}"/>
	public void SetEntry<T>(string key, CacheEntry<T> item, DistributedCacheEntryOptions cacheEntryOptions)
	{
		ArgumentNullException.ThrowIfNull(key);
		ArgumentNullException.ThrowIfNull(item);
		ArgumentNullException.ThrowIfNull(cacheEntryOptions);

		var value = _stringSerializer.Serialize(item);
		_distributedCache.SetString(key, value, cacheEntryOptions);
	}

	/// <inheritdoc cref="IAsyncCacheStore{TCacheEntryOptions}.SetEntryAsync{T}"/>
	public Task SetEntryAsync<T>(string key, CacheEntry<T> item, DistributedCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(key);
		ArgumentNullException.ThrowIfNull(item);
		ArgumentNullException.ThrowIfNull(cacheEntryOptions);

		var value = _stringSerializer.Serialize(item);
		return _distributedCache.SetStringAsync(key, value, cacheEntryOptions, cancellationToken);
	}

	/// <inheritdoc cref="ISyncCacheStore{TCacheEntryOptions}.RemoveEntry"/>
	public void RemoveEntry(string key)
	{
		ArgumentNullException.ThrowIfNull(key);

		_distributedCache.Remove(key);
	}

	/// <inheritdoc cref="IAsyncCacheStore{TCacheEntryOptions}.RemoveEntryAsync"/>
	public Task RemoveEntryAsync(string key, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(key);

		return _distributedCache.RemoveAsync(key, cancellationToken);
	}
}
