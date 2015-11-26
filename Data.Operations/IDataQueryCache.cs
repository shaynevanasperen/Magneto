using System;
using System.Threading.Tasks;

namespace Data.Operations
{
	public interface IDataQueryCache : ISyncDataQueryCache, IAsyncDataQueryCache { }

	public interface ISyncDataQueryCache
	{
		T Get<T>(Func<T> executeQuery, ICacheInfo cacheInfo);
		void Refresh<T>(T queryResult, ICacheInfo cacheInfo);
		void Evict(ICacheInfo cacheInfo);
	}

	public interface IAsyncDataQueryCache
	{
		Task<T> GetAsync<T>(Func<Task<T>> executeQueryAsync, ICacheInfo cacheInfo);
		Task RefreshAsync<T>(T queryResult, ICacheInfo cacheInfo);
		Task EvictAsync(ICacheInfo cacheInfo);
	}
}