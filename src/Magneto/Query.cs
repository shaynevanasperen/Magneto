using System.Threading;
using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto
{
	/// <summary>
	/// <para>A base class for synchronous queries.</para>
	/// <para>Implementors must override <see cref="Query"/> in order to define how the query is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TResult">The type of the query result.</typeparam>
	public abstract class SyncQuery<TContext, TResult> : Operation, ISyncQuery<TContext, TResult>
	{
		/// <inheritdoc cref="ISyncQuery{TContext,TResult}.Execute"/>
		public TResult Execute(TContext context) => Query(context);

		/// <inheritdoc cref="ISyncQuery{TContext,TResult}.Execute"/>
		protected abstract TResult Query(TContext context);
	}

	/// <summary>
	/// <para>A base class for asynchronous queries.</para>
	/// <para>Implementors must override <see cref="QueryAsync"/> in order to define how the query is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TResult">The type of the query result.</typeparam>
	public abstract class AsyncQuery<TContext, TResult> : Operation, IAsyncQuery<TContext, TResult>
	{
		/// <inheritdoc cref="IAsyncQuery{TContext,TResult}.ExecuteAsync"/>
		public Task<TResult> ExecuteAsync(TContext context, CancellationToken cancellationToken = default) =>
			QueryAsync(context, cancellationToken);

		/// <inheritdoc cref="IAsyncQuery{TContext,TResult}.ExecuteAsync"/>
		protected abstract Task<TResult> QueryAsync(TContext context, CancellationToken cancellationToken = default);
	}
}
