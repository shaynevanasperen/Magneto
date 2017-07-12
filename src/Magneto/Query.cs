using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto
{
	public abstract class SyncQuery<TContext, TResult> : Operation, ISyncQuery<TContext, TResult>
	{
		public abstract TResult Execute(TContext context);
	}

	public abstract class AsyncQuery<TContext, TResult> : Operation, IAsyncQuery<TContext, TResult>
	{
		public abstract Task<TResult> ExecuteAsync(TContext context);
	}
}
