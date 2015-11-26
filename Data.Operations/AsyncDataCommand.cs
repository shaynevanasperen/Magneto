using System.Threading.Tasks;

namespace Data.Operations
{
	public abstract class AsyncDataCommand<TContext> : DataOperation, IAsyncDataCommand<TContext>
	{
		public abstract Task ExecuteAsync(TContext context);
	}

	public abstract class AsyncDataCommand<TContext, TResult> : DataOperation, IAsyncDataCommand<TContext, TResult>
	{
		public abstract Task<TResult> ExecuteAsync(TContext context);
	}
}
