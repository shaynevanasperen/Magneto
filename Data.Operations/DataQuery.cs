namespace Data.Operations
{
	public abstract class DataQuery<TContext, TResult> : DataOperation, IDataQuery<TContext, TResult>
	{
		public abstract TResult Execute(TContext context);
	}
}
