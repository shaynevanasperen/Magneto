namespace Data.Operations
{
	public interface IDataQuery<in TContext, out TResult>
	{
		TResult Execute(TContext context);
	}
}