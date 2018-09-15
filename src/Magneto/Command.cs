using System.Threading;
using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto
{
	/// <summary>
	/// <para>A base class for synchronous commands which don't return results.</para>
	/// <para>Implementors must override <see cref="Execute"/> in order to define how the command is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
	public abstract class SyncCommand<TContext> : Operation, ISyncCommand<TContext>
	{
		/// <inheritdoc cref="ISyncCommand{TContext}.Execute"/>
		public abstract void Execute(TContext context);
	}

	/// <summary>
	/// <para>A base class for synchronous commands which return results.</para>
	/// <para>Implementors must override <see cref="Execute"/> in order to define how the command is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
	/// <typeparam name="TResult">The type of the command result.</typeparam>
	public abstract class SyncCommand<TContext, TResult> : Operation, ISyncCommand<TContext, TResult>
	{
		/// <inheritdoc cref="ISyncCommand{TContext,TResult}.Execute"/>
		public abstract TResult Execute(TContext context);
	}

	/// <summary>
	/// <para>A base class for asynchronous commands which don't return results.</para>
	/// <para>Implementors must override <see cref="ExecuteAsync"/> in order to define how the command is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
	public abstract class AsyncCommand<TContext> : Operation, IAsyncCommand<TContext>
	{
		/// <inheritdoc cref="IAsyncCommand{TContext}.ExecuteAsync"/>
		public abstract Task ExecuteAsync(TContext context, CancellationToken cancellationToken = default);
	}

	/// <summary>
	/// <para>A base class for asynchronous commands which return results.</para>
	/// <para>Implementors must override <see cref="ExecuteAsync"/> in order to define how the command is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
	/// <typeparam name="TResult">The type of the command result.</typeparam>
	public abstract class AsyncCommand<TContext, TResult> : Operation, IAsyncCommand<TContext, TResult>
	{
		/// <inheritdoc cref="IAsyncCommand{TContext,TResult}.ExecuteAsync"/>
		public abstract Task<TResult> ExecuteAsync(TContext context, CancellationToken cancellationToken = default);
	}
}
