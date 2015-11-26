using System.Threading.Tasks;

namespace Data.Operations
{
	public interface IAsyncDataQuery<in TContext, TResult>
	{
		Task<TResult> ExecuteAsync(TContext context);
	}
}
