using System;
using System.Threading.Tasks;

namespace Data.Operations
{
	public class Data : IData
	{
		readonly IDataQueryCache _dataQueryCache;

		public Data(IDataQueryCache dataQueryCache = null)
		{
			_dataQueryCache = dataQueryCache;
		}

		public virtual TResult Query<TContext, TResult>(IDataQuery<TContext, TResult> query, TContext context)
		{
			return query.Execute(context);
		}

		public virtual Task<TResult> QueryAsync<TContext, TResult>(IAsyncDataQuery<TContext, TResult> query, TContext context)
		{
			return query.ExecuteAsync(context);
		}

		public virtual TResult Query<TContext, TCachedResult, TResult>(
			ICachedDataQuery<TContext, TCachedResult, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default)
		{
			return query.Execute(context, _dataQueryCache, cacheOption);
		}

		public virtual Task<TResult> QueryAsync<TContext, TCachedResult, TResult>(
			IAsyncCachedDataQuery<TContext, TCachedResult, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default)
		{
			return query.ExecuteAsync(context, _dataQueryCache, cacheOption);
		}

		public virtual void EvictCachedResult<TCachedResult>(ICachedDataQueryBase<TCachedResult> query)
		{
			if (_dataQueryCache == null)
				return;

			var cacheInfo = query.GetCacheInfo();
			if (cacheInfo.Disabled)
				return;

			_dataQueryCache.Evict(cacheInfo);
		}

		public virtual Task EvictCachedResultAsync<TCachedResult>(ICachedDataQueryBase<TCachedResult> query)
		{
			if (_dataQueryCache == null)
				return Task.FromResult(0);

			var cacheInfo = query.GetCacheInfo();
			return cacheInfo.Disabled
				? Task.FromResult(0)
				: _dataQueryCache.EvictAsync(cacheInfo);
		}

		public virtual void UpdateCachedResult<TCachedResult>(ICachedDataQueryBase<TCachedResult> executedQuery)
		{
			if (_dataQueryCache == null)
				return;

			var cacheInfo = executedQuery.GetCacheInfo();
			if (cacheInfo.Disabled)
				return;

			_dataQueryCache.Refresh(executedQuery.CachedResult, cacheInfo);
		}

		public virtual Task UpdateCachedResultAsync<TCachedResult>(ICachedDataQueryBase<TCachedResult> executedQuery)
		{
			if (_dataQueryCache == null)
				return Task.FromResult(0);

			var cacheInfo = executedQuery.GetCacheInfo();
			return cacheInfo.Disabled
				? Task.FromResult(0)
				: _dataQueryCache.RefreshAsync(executedQuery.CachedResult, cacheInfo);
		}

		public virtual void Command<TContext>(IDataCommand<TContext> command, TContext context)
		{
			command.Execute(context);
		}

		public virtual Task CommandAsync<TContext>(IAsyncDataCommand<TContext> command, TContext context)
		{
			return command.ExecuteAsync(context);
		}

		public virtual TResult Command<TContext, TResult>(IDataCommand<TContext, TResult> command, TContext context)
		{
			return command.Execute(context);
		}

		public virtual Task<TResult> CommandAsync<TContext, TResult>(IAsyncDataCommand<TContext, TResult> command, TContext context)
		{
			return command.ExecuteAsync(context);
		}
	}

	public class Data<TContext> : IData<TContext>
	{
		readonly IData _data;

		public Data(IData data, TContext context)
		{
			_data = data;
			Context = context;
		}

		public virtual TContext Context { get; private set; }

		public virtual TResult Query<TResult>(IDataQuery<TContext, TResult> query)
		{
			return _data.Query(query, Context);
		}

		public virtual Task<TResult> QueryAsync<TResult>(IAsyncDataQuery<TContext, TResult> query)
		{
			return _data.QueryAsync(query, Context);
		}

		public virtual TResult Query<TCachedResult, TResult>(
			ICachedDataQuery<TContext, TCachedResult, TResult> query, CacheOption cacheOption = CacheOption.Default)
		{
			return _data.Query(query, Context, cacheOption);
		}

		public virtual Task<TResult> QueryAsync<TCachedResult, TResult>(
			IAsyncCachedDataQuery<TContext, TCachedResult, TResult> query, CacheOption cacheOption = CacheOption.Default)
		{
			return _data.QueryAsync(query, Context, cacheOption);
		}

		public virtual void EvictCachedResult<TCachedResult>(ICachedDataQueryBase<TCachedResult> query)
		{
			_data.EvictCachedResult(query);
		}

		public virtual Task EvictCachedResultAsync<TCachedResult>(ICachedDataQueryBase<TCachedResult> query)
		{
			return _data.EvictCachedResultAsync(query);
		}

		public virtual void UpdateCachedResult<TCachedResult>(ICachedDataQueryBase<TCachedResult> executedQuery)
		{
			_data.UpdateCachedResult(executedQuery);
		}

		public virtual Task UpdateCachedResultAsync<TCachedResult>(ICachedDataQueryBase<TCachedResult> executedQuery)
		{
			return _data.UpdateCachedResultAsync(executedQuery);
		}

		public virtual void Command(IDataCommand<TContext> command)
		{
			_data.Command(command, Context);
		}

		public virtual Task CommandAsync(IAsyncDataCommand<TContext> command)
		{
			return _data.CommandAsync(command, Context);
		}

		public virtual TResult Command<TResult>(IDataCommand<TContext, TResult> command)
		{
			return _data.Command(command, Context);
		}

		public virtual Task<TResult> CommandAsync<TResult>(IAsyncDataCommand<TContext, TResult> command)
		{
			return _data.CommandAsync(command, Context);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		bool _disposed;
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				var disposable = Context as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
			_disposed = true;
		}
	}
}