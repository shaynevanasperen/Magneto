using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto
{
	/// <summary>
	/// A base class for synchronous queries.
	/// <para>Implementors must override <see cref="Execute"/> in order to define how the query is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TResult">The type of the query result.</typeparam>
	public abstract class SyncQuery<TContext, TResult> : Operation, ISyncQuery<TContext, TResult>
	{
		public abstract TResult Execute(TContext context);
	}

	/// <summary>
	/// A base class for asynchronous queries.
	/// <para>Implementors must override <see cref="ExecuteAsync"/> in order to define how the query is executed.</para>
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TResult">The type of the query result.</typeparam>
	public abstract class AsyncQuery<TContext, TResult> : Operation, IAsyncQuery<TContext, TResult>
	{
		public abstract Task<TResult> ExecuteAsync(TContext context);
	}
}
