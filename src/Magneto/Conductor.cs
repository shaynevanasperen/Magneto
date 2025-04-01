using System;
using System.Threading;
using System.Threading.Tasks;
using Code.Extensions.ValueTuple.ServiceProvider;
using Magneto.Configuration;
using Magneto.Core;

namespace Magneto;

/// <summary>
/// <para>
/// The main entry point for consumers to execute queries and commands using context obtained from the given <see cref="IServiceProvider"/>. If using
/// cached queries, make sure to register their respective cache stores in the given <see cref="IServiceProvider"/>. If the given <see cref="IServiceProvider"/>
/// is not able to provide the required <see cref="ISyncCacheStore{TCacheEntryOptions}"/> or <see cref="IAsyncCacheStore{TCacheEntryOptions}"/> for a
/// given cached query, caching functionality is disabled for that cached query. A special wrapper is used for the <see cref="IServiceProvider"/>
/// which can resolve instances of <see cref="ValueTuple"/> by resolving each item from the inner <see cref="IServiceProvider"/>. This enables queries
/// and commands to express the required context as being a composite of several items. For example:
/// </para>
/// <example>
/// class MyQuery : ISyncQuery&lt;(DbContext, IFileProvider), string&gt; { ... }
/// </example>
/// </summary>
public class Conductor : IMagneto
{
	/// <summary>
	/// Creates a new instance of <see cref="Conductor"/>. The contained <see cref="IMediary"/> is initialized from either the instance of
	/// <see cref="IMediary"/> obtained from the given <see cref="IServiceProvider"/>, or a new instance of <see cref="Mediary"/> if the given
	/// <see cref="IServiceProvider"/> couldn't provide it. The contained <see cref="IServiceProvider"/> wraps <paramref name="serviceProvider"/>,
	/// adding the capability to resolve instances of <see cref="ValueTuple"/>.
	/// </summary>
	/// <param name="serviceProvider">Used for obtaining instances of the context objects with which queries and commands are invoked.</param>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="serviceProvider"/> is null.</exception>
	public Conductor(IServiceProvider serviceProvider)
	{
		ArgumentNullException.ThrowIfNull(serviceProvider);
		
		Mediary = serviceProvider.GetService<IMediary>() ?? new Mediary(serviceProvider);
		ServiceProvider = new ValueTupleServiceProvider(serviceProvider);
	}

	/// <summary>
	/// Exposes an <see cref="IServiceProvider"/> which wraps the one provided in the constructor. The wrapper can resolve instances of
	/// <see cref="ValueTuple"/> by resolving each item from the inner <see cref="IServiceProvider"/>. This enables queries and commands
	/// to express the required context as being a composite of several items.
	/// </summary>
	protected IServiceProvider ServiceProvider { get; }

	/// <summary>
	/// Exposes the <see cref="IMediary"/> obtained from the <see cref="IServiceProvider"/> provided in the constructor, or an instance of
	/// <see cref="Mediary"/> if that <see cref="IServiceProvider"/> couldn't provide it.
	/// </summary>
	protected IMediary Mediary { get; }

	/// <summary>
	/// Gets the <typeparamref name="TContext"/> from the <see cref="ServiceProvider"/>.
	/// </summary>
	/// <typeparam name="TContext">The type of context object required.</typeparam>
	/// <returns>The result from calling <see cref="IServiceProvider.GetService"/>.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the <see cref="ServiceProvider"/> couldn't provide it.</exception>
	protected virtual TContext GetContext<TContext>() => ServiceProvider.GetRequiredService<TContext>();

	/// <inheritdoc cref="ISyncQueryMagneto.Query{TContext,TResult}"/>
	public virtual TResult Query<TContext, TResult>(ISyncQuery<TContext, TResult> query) =>
		Mediary.Query(query, GetContext<TContext>());

	/// <inheritdoc cref="IAsyncQueryMagneto.QueryAsync{TContext,TResult}"/>
	public virtual Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query, CancellationToken cancellationToken = default) =>
		Mediary.QueryAsync(query, GetContext<TContext>(), cancellationToken);

	/// <inheritdoc cref="ISyncQueryMagneto.Query{TContext,TCacheEntryOptions,TResult}"/>
	public virtual TResult Query<TContext, TCacheEntryOptions, TResult>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption) =>
		Mediary.Query(query, GetContext<TContext>(), cacheOption);

	/// <inheritdoc cref="IAsyncQueryMagneto.QueryAsync{TContext,TCacheEntryOptions,TResult}"/>
	public virtual Task<TResult> QueryAsync<TContext, TCacheEntryOptions, TResult>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption, CancellationToken cancellationToken = default) =>
		Mediary.QueryAsync(query, GetContext<TContext>(), cacheOption, cancellationToken);

	/// <inheritdoc cref="ISyncCacheManager.EvictCachedResult{TCacheEntryOptions}"/>
	public virtual void EvictCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> query) =>
		Mediary.EvictCachedResult(query);

	/// <inheritdoc cref="IAsyncCacheManager.EvictCachedResultAsync{TCacheEntryOptions}"/>
	public virtual Task EvictCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> query, CancellationToken cancellationToken = default) =>
		Mediary.EvictCachedResultAsync(query, cancellationToken);

	/// <inheritdoc cref="ISyncCacheManager.UpdateCachedResult{TCacheEntryOptions}"/>
	public virtual void UpdateCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> executedQuery) =>
		Mediary.UpdateCachedResult(executedQuery);

	/// <inheritdoc cref="IAsyncCacheManager.UpdateCachedResultAsync{TCacheEntryOptions}"/>
	public virtual Task UpdateCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> executedQuery, CancellationToken cancellationToken = default) =>
		Mediary.UpdateCachedResultAsync(executedQuery, cancellationToken);

	/// <inheritdoc cref="ISyncCommandMagneto.Command{TContext}"/>
	public virtual void Command<TContext>(ISyncCommand<TContext> command) =>
		Mediary.Command(command, GetContext<TContext>());

	/// <inheritdoc cref="IAsyncCommandMagneto.CommandAsync{TContext}"/>
	public virtual Task CommandAsync<TContext>(IAsyncCommand<TContext> command, CancellationToken cancellationToken = default) =>
		Mediary.CommandAsync(command, GetContext<TContext>(), cancellationToken);

	/// <inheritdoc cref="ISyncCommandMagneto.Command{TContext,TResult}"/>
	public virtual TResult Command<TContext, TResult>(ISyncCommand<TContext, TResult> command) =>
		Mediary.Command(command, GetContext<TContext>());

	/// <inheritdoc cref="IAsyncCommandMagneto.CommandAsync{TContext,TResult}"/>
	public virtual Task<TResult> CommandAsync<TContext, TResult>(IAsyncCommand<TContext, TResult> command, CancellationToken cancellationToken = default) =>
		Mediary.CommandAsync(command, GetContext<TContext>(), cancellationToken);
}
