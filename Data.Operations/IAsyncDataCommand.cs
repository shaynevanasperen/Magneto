using System.Threading.Tasks;

namespace Data.Operations
{
	public interface IAsyncDataCommand<in TContext>
	{
		Task ExecuteAsync(TContext context);
	}

	public interface IAsyncDataCommand<in TContext, TResult>
	{
		Task<TResult> ExecuteAsync(TContext context);
	}
}