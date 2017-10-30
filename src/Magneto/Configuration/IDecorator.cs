using System;
using System.Threading.Tasks;

namespace Magneto.Configuration
{
	public interface IDecorator : ISyncDecorator, IAsyncDecorator { }

	public interface ISyncDecorator
	{
		/// <summary>
		/// Decorates the given <paramref name="operation"/> with predefined behaviours (such as logging, tracing, or error handling).
		/// </summary>
		/// <typeparam name="TContext">The type of context required by the operation.</typeparam>
		/// <typeparam name="TResult">The type of the result returned by the operation.</typeparam>
		/// <param name="operation">The operation which will be decorated with behaviours.</param>
		/// <param name="context">The context with which the given operation will execute.</param>
		/// <param name="invoke">A delegate representing the invocation of the operation.</param>
		/// <returns>The result returned by the operation invocation.</returns>
		TResult Decorate<TContext, TResult>(object operation, TContext context, Func<TContext, TResult> invoke);

		/// <summary>
		/// Decorates the given <paramref name="operation"/> with predefined behaviours (such as logging, tracing, or error handling).
		/// </summary>
		/// <typeparam name="TContext">The type of context required by the operation.</typeparam>
		/// <param name="operation">The operation which will be decorated with behaviours.</param>
		/// <param name="context">The context with which the given operation will execute.</param>
		/// <param name="invoke">A delegate representing the invocation of the operation.</param>
		void Decorate<TContext>(object operation, TContext context, Action<TContext> invoke);
	}

	public interface IAsyncDecorator
	{
		/// <summary>
		/// Decorates the given <paramref name="operation"/> with predefined behaviours (such as logging, tracing, or error handling).
		/// </summary>
		/// <typeparam name="TContext">The type of context required by the operation.</typeparam>
		/// <typeparam name="TResult">The type of the result returned by the operation.</typeparam>
		/// <param name="operation">The operation which will be decorated with behaviours.</param>
		/// <param name="context">The context with which the given operation will execute.</param>
		/// <param name="invoke">A delegate representing the invocation of the operation.</param>
		/// <returns>The result returned by the operation invocation.</returns>
		Task<TResult> Decorate<TContext, TResult>(object operation, TContext context, Func<TContext, Task<TResult>> invoke);

		/// <summary>
		/// Decorates the given <paramref name="operation"/> with predefined behaviours (such as logging, tracing, or error handling).
		/// </summary>
		/// <typeparam name="TContext">The type of context required by the operation.</typeparam>
		/// <param name="operation">The operation which will be decorated with behaviours.</param>
		/// <param name="context">The context with which the given operation will execute.</param>
		/// <param name="invoke">A delegate representing the invocation of the operation.</param>
		/// <returns>A task representing the operation invocation.</returns>
		Task Decorate<TContext>(object operation, TContext context, Func<TContext, Task> invoke);
	}
}
