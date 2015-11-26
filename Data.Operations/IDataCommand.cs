namespace Data.Operations
{
	public interface IDataCommand<in TContext>
	{
		void Execute(TContext context);
	}

	public interface IDataCommand<in TContext, out TResult>
	{
		TResult Execute(TContext context);
	}
}