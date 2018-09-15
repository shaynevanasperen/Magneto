﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Magneto.Configuration;
using Magneto.Core;

namespace Magneto
{
	/// <summary>
	/// If using an IoC container, it's highly recommended that this be registered as a scoped service
	/// so that the injected <see cref="IServiceProvider"/> is scoped appropriately.
	/// </summary>
	public class Magneto : IMagneto
	{
		/// <summary>
		/// Creates a new instance of <see cref="Magneto"/>.
		/// </summary>
		/// <param name="serviceProvider">Used for obtaining instances of the context objects with which queries and commands are invoked.</param>
		public Magneto(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			Mediary = ServiceProvider.GetService<IMediary>() ?? new Mediary(ServiceProvider, ServiceProvider.GetService<IDecorator>());
		}

		protected IServiceProvider ServiceProvider { get; }

		protected IMediary Mediary { get; }

		protected virtual TContext GetContext<TContext>() => ServiceProvider.GetService<TContext>();

		/// <inheritdoc cref="ISyncQueryMagneto.Query{TContext,TResult}"/>
		public virtual TResult Query<TContext, TResult>(ISyncQuery<TContext, TResult> query) =>
			Mediary.Query(query, GetContext<TContext>());

		/// <inheritdoc cref="IAsyncQueryMagneto.QueryAsync{TContext,TResult}"/>
		public virtual Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query, CancellationToken cancellationToken = default) =>
			Mediary.QueryAsync(query, GetContext<TContext>(), cancellationToken);

		/// <inheritdoc cref="ISyncQueryMagneto.Query{TContext,TCacheEntryOptions,TResult}"/>
		public virtual TResult Query<TContext, TCacheEntryOptions, TResult>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption = CacheOption.Default) =>
			Mediary.Query(query, GetContext<TContext>(), cacheOption);

		/// <inheritdoc cref="IAsyncQueryMagneto.QueryAsync{TContext,TCacheEntryOptions,TResult}"/>
		public virtual Task<TResult> QueryAsync<TContext, TCacheEntryOptions, TResult>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption = CacheOption.Default, CancellationToken cancellationToken = default) =>
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
}