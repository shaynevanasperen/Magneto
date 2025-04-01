using System;
using System.Threading.Tasks;

namespace Magneto.Configuration;

/// <summary>
/// Decorates the given operations with predefined behaviour (such as logging, tracing, or error handling).
/// </summary>
public interface IDecorator : ISyncDecorator, IAsyncDecorator { }

/// <summary>
/// Decorates the given synchronous operations with predefined behaviour (such as logging, tracing, or error handling).
/// </summary>
public interface ISyncDecorator
{
	/// <summary>
	/// Decorates the given operation with predefined behaviour (such as logging, tracing, or error handling).
	/// </summary>
	/// <typeparam name="TResult">The type of the result returned by the operation.</typeparam>
	/// <param name="operationName">The name of the operation which will be decorated with behaviour.</param>
	/// <param name="invoke">A delegate representing the invocation of the operation.</param>
	/// <returns>The result returned by the operation invocation.</returns>
	TResult Decorate<TResult>(string operationName, Func<TResult> invoke);

	/// <summary>
	/// Decorates the given operation with predefined behaviour (such as logging, tracing, or error handling).
	/// </summary>
	/// <param name="operationName">The name of the operation which will be decorated with behaviour.</param>
	/// <param name="invoke">A delegate representing the invocation of the operation.</param>
	void Decorate(string operationName, Action invoke);
}

/// <summary>
/// Decorates the given asynchronous operations with predefined behaviour (such as logging, tracing, or error handling).
/// </summary>
public interface IAsyncDecorator
{
	/// <summary>
	/// Decorates the given operation with predefined behaviour (such as logging, tracing, or error handling).
	/// </summary>
	/// <typeparam name="TResult">The type of the result returned by the operation.</typeparam>
	/// <param name="operationName">The name of the operation which will be decorated with behaviour.</param>
	/// <param name="invoke">A delegate representing the invocation of the operation.</param>
	/// <returns>The result returned by the operation invocation.</returns>
	Task<TResult> Decorate<TResult>(string operationName, Func<Task<TResult>> invoke);

	/// <summary>
	/// Decorates the given operation with predefined behaviour (such as logging, tracing, or error handling).
	/// </summary>
	/// <param name="operationName">The name of the operation which will be decorated with behaviour.</param>
	/// <param name="invoke">A delegate representing the invocation of the operation.</param>
	/// <returns>A task representing the operation invocation.</returns>
	Task Decorate(string operationName, Func<Task> invoke);
}
