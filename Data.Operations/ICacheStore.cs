using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Data.Operations
{
	public interface ICacheStore : ISyncCacheStore, IAsyncCacheStore { }

	public interface ISyncCacheStore
	{
		object GetItem(string cacheKey);
		void SetItem(string cacheKey, object item, CacheItemPolicy cacheItemPolicy);
		void RemoveItem(string cacheKey);
	}

	public interface IAsyncCacheStore
	{
		Task<object> GetItemAsync(string cacheKey);
		Task SetItemAsync(string cacheKey, object item, CacheItemPolicy cacheItemPolicy);
		Task RemoveItemAsync(string cacheKey);
	}
}