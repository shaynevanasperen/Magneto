using System.Threading;
using System.Threading.Tasks;

namespace Magneto
{
	/// <summary>
	/// A query operation that executes synchronously.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TResult">The type of the query result.</typeparam>
	public interface ISyncQuery<in TContext, out TResult>
	{
		/// <summary>
		/// Executes the query and returns the result.
		/// </summary>
		/// <param name="context">The context with which the query is executed.</param>
		/// <returns>The result of query execution.</returns>
		TResult Execute(TContext context);
	}

	/// <summary>
	/// A query operation that executes asynchronously.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TResult">The type of the query result.</typeparam>
	public interface IAsyncQuery<in TContext, TResult>
	{
		/// <summary>
		/// Executes the query and returns the result.
		/// </summary>
		/// <param name="context">The context with which the query is executed.</param>
		/// <param name="cancellationToken">A <see cref="CancellationToken" /> to cancel the operation.</param>
		/// <returns>The result of query execution.</returns>
		Task<TResult> Execute(TContext context, CancellationToken cancellationToken);
	}
}
