using System.Threading.Tasks;

namespace Magneto
{
	public interface ISyncQuery<in TContext, out TResult>
	{
		TResult Execute(TContext context);
	}

	public interface IAsyncQuery<in TContext, TResult>
	{
		Task<TResult> ExecuteAsync(TContext context);
	}
}