using System.Threading;
using System.Threading.Tasks;

namespace Magneto;

/// <summary>
/// A command operation that executes synchronously and doesn't return a result.
/// </summary>
/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
public interface ISyncCommand<in TContext>
{
	/// <summary>
	/// Executes the command.
	/// </summary>
	/// <param name="context">The context with which the command is executed.</param>
	void Execute(TContext context);
}

/// <summary>
/// A command operation that executes synchronously and returns a result.
/// </summary>
/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
/// <typeparam name="TResult">The type of the command result.</typeparam>
public interface ISyncCommand<in TContext, out TResult>
{
	/// <summary>
	/// Executes the command and returns the result.
	/// </summary>
	/// <param name="context">The context with which the command is executed.</param>
	/// <returns>The result of command execution.</returns>
	TResult Execute(TContext context);
}

/// <summary>
/// A command operation that executes asynchronously and doesn't return a result.
/// </summary>
/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
public interface IAsyncCommand<in TContext>
{
	/// <summary>
	/// Executes the command.
	/// </summary>
	/// <param name="context">The context with which the command is executed.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the operation.</param>
	/// <returns>A task representing the execution of the command.</returns>
	Task Execute(TContext context, CancellationToken cancellationToken);
}

/// <summary>
/// A command operation that executes asynchronously and returns a result.
/// </summary>
/// <typeparam name="TContext">The type of context with which the command is executed.</typeparam>
/// <typeparam name="TResult">The type of the command result.</typeparam>
public interface IAsyncCommand<in TContext, TResult>
{
	/// <summary>
	/// Executes the command and returns the result.
	/// </summary>
	/// <param name="context">The context with which the command is executed.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the operation.</param>
	/// <returns>The result of command execution.</returns>
	Task<TResult> Execute(TContext context, CancellationToken cancellationToken);
}
