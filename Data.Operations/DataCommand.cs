namespace Data.Operations
{
	public abstract class DataCommand<TContext> : DataOperation, IDataCommand<TContext>
	{
		public abstract void Execute(TContext context);
	}

	public abstract class DataCommand<TContext, TResult> : DataOperation, IDataCommand<TContext, TResult>
	{
		public abstract TResult Execute(TContext context);
	}
}
