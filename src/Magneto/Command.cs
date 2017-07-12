using System.Threading.Tasks;
using Magneto.Core;

namespace Magneto
{
	public abstract class SyncCommand<TContext> : Operation, ISyncCommand<TContext>
	{
		public abstract void Execute(TContext context);
	}

	public abstract class SyncCommand<TContext, TResult> : Operation, ISyncCommand<TContext, TResult>
	{
		public abstract TResult Execute(TContext context);
	}

	public abstract class AsyncCommand<TContext> : Operation, IAsyncCommand<TContext>
	{
		public abstract Task ExecuteAsync(TContext context);
	}

	public abstract class AsyncCommand<TContext, TResult> : Operation, IAsyncCommand<TContext, TResult>
	{
		public abstract Task<TResult> ExecuteAsync(TContext context);
	}
}
