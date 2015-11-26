using System.Threading.Tasks;

namespace Data.Operations
{
	public abstract class AsyncDataQuery<TContext, TResult> : DataOperation, IAsyncDataQuery<TContext, TResult>
	{
		public abstract Task<TResult> ExecuteAsync(TContext context);
	}
}
