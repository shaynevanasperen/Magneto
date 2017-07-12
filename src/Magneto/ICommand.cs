using System.Threading.Tasks;

namespace Magneto
{
	public interface ISyncCommand<in TContext>
	{
		void Execute(TContext context);
	}

	public interface ISyncCommand<in TContext, out TResult>
	{
		TResult Execute(TContext context);
	}

	public interface IAsyncCommand<in TContext>
	{
		Task ExecuteAsync(TContext context);
	}

	public interface IAsyncCommand<in TContext, TResult>
	{
		Task<TResult> ExecuteAsync(TContext context);
	}
}