using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto
{
	/// <summary>
	/// A base class for synchronous commands which don't return results.
	/// <para>Implementors must override <see cref="Execute"/> in order to define how the command is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
	public abstract class SyncCommand<TContext> : Operation, ISyncCommand<TContext>
	{
		public abstract void Execute(TContext context);
	}

	/// <summary>
	/// A base class for synchronous commands which return results.
	/// <para>Implementors must override <see cref="Execute"/> in order to define how the command is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
	/// <typeparam name="TResult">The type of the command result.</typeparam>
	public abstract class SyncCommand<TContext, TResult> : Operation, ISyncCommand<TContext, TResult>
	{
		public abstract TResult Execute(TContext context);
	}

	/// <summary>
	/// A base class for asynchronous commands which don't return results.
	/// <para>Implementors must override <see cref="ExecuteAsync"/> in order to define how the command is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
	public abstract class AsyncCommand<TContext> : Operation, IAsyncCommand<TContext>
	{
		public abstract Task ExecuteAsync(TContext context);
	}

	/// <summary>
	/// A base class for asynchronous commands which return results.
	/// <para>Implementors must override <see cref="ExecuteAsync"/> in order to define how the command is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
	/// <typeparam name="TResult">The type of the command result.</typeparam>
	public abstract class AsyncCommand<TContext, TResult> : Operation, IAsyncCommand<TContext, TResult>
	{
		public abstract Task<TResult> ExecuteAsync(TContext context);
	}
}
