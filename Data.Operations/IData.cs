using System;
using System.Threading.Tasks;

namespace Data.Operations
{
	public interface IData : ISyncData, IAsyncData, IQueryInvoker, ICommandInvoker { }

	public interface IQueryInvoker : ISyncQueryInvoker, IAsyncQueryInvoker { }

	public interface ICommandInvoker : ISyncCommandInvoker, IAsyncCommandInvoker { }

	public interface ISyncData : ISyncQueryInvoker, ISyncCommandInvoker { }

	public interface IAsyncData : IAsyncQueryInvoker, IAsyncCommandInvoker { }

	public interface ISyncQueryInvoker
	{
		TResult Query<TContext, TResult>(IDataQuery<TContext, TResult> query, TContext context);
		TResult Query<TContext, TCachedResult, TResult>(
			ICachedDataQuery<TContext, TCachedResult, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default);
		void EvictCachedResult<TCachedResult>(ICachedDataQueryBase<TCachedResult> query);
		void UpdateCachedResult<TCachedResult>(ICachedDataQueryBase<TCachedResult> executedQuery);
	}

	public interface IAsyncQueryInvoker
	{
		Task<TResult> QueryAsync<TContext, TResult>(IAsyncDataQuery<TContext, TResult> query, TContext context);
		Task<TResult> QueryAsync<TContext, TCachedResult, TResult>(
			IAsyncCachedDataQuery<TContext, TCachedResult, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default);
		Task EvictCachedResultAsync<TCachedResult>(ICachedDataQueryBase<TCachedResult> query);
		Task UpdateCachedResultAsync<TCachedResult>(ICachedDataQueryBase<TCachedResult> executedQuery);
	}

	public interface ISyncCommandInvoker
	{
		void Command<TContext>(IDataCommand<TContext> command, TContext context);
		TResult Command<TContext, TResult>(IDataCommand<TContext, TResult> command, TContext context);
	}

	public interface IAsyncCommandInvoker
	{
		Task CommandAsync<TContext>(IAsyncDataCommand<TContext> command, TContext context);
		Task<TResult> CommandAsync<TContext, TResult>(IAsyncDataCommand<TContext, TResult> command, TContext context);
	}

	public interface IData<out TContext> : ISyncData<TContext>, IAsyncData<TContext>, IQueryInvoker<TContext>, ICommandInvoker<TContext> { }

	public interface IQueryInvoker<out TContext> : ISyncQueryInvoker<TContext>, IAsyncQueryInvoker<TContext> { }

	public interface ICommandInvoker<out TContext> : ISyncCommandInvoker<TContext>, IAsyncCommandInvoker<TContext> { }

	public interface ISyncData<out TContext> : ISyncQueryInvoker<TContext>, ISyncCommandInvoker<TContext> { }

	public interface IAsyncData<out TContext> : IAsyncQueryInvoker<TContext>, IAsyncCommandInvoker<TContext> { }

	public interface ISyncQueryInvoker<out TContext> : IContextData<TContext>
	{
		TResult Query<TResult>(IDataQuery<TContext, TResult> query);
		TResult Query<TCachedResult, TResult>(
			ICachedDataQuery<TContext, TCachedResult, TResult> query, CacheOption cacheOption = CacheOption.Default);
		void EvictCachedResult<TCachedResult>(ICachedDataQueryBase<TCachedResult> query);
		void UpdateCachedResult<TCachedResult>(ICachedDataQueryBase<TCachedResult> executedQuery);
	}

	public interface IAsyncQueryInvoker<out TContext> : IContextData<TContext>
	{
		Task<TResult> QueryAsync<TResult>(IAsyncDataQuery<TContext, TResult> query);
		Task<TResult> QueryAsync<TCachedResult, TResult>(
			IAsyncCachedDataQuery<TContext, TCachedResult, TResult> query, CacheOption cacheOption = CacheOption.Default);
		Task EvictCachedResultAsync<TCachedResult>(ICachedDataQueryBase<TCachedResult> query);
		Task UpdateCachedResultAsync<TCachedResult>(ICachedDataQueryBase<TCachedResult> executedQuery);
	}

	public interface ISyncCommandInvoker<out TContext> : IContextData<TContext>
	{
		void Command(IDataCommand<TContext> command);
		TResult Command<TResult>(IDataCommand<TContext, TResult> command);
	}

	public interface IAsyncCommandInvoker<out TContext> : IContextData<TContext>
	{
		Task CommandAsync(IAsyncDataCommand<TContext> command);
		Task<TResult> CommandAsync<TResult>(IAsyncDataCommand<TContext, TResult> command);
	}

	public interface IContextData<out TContext> : IDisposable
	{
		TContext Context { get; }
	}
}
