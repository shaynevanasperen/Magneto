using System;
using System.Threading.Tasks;

namespace Data.Operations
{
	public interface ICacheStore : ISyncCacheStore, IAsyncCacheStore { }

	public interface ISyncCacheStore
	{
		object GetItem(string cacheKey);
		void SetItem(string cacheKey, object item, TimeSpan absoluteDuration);
		void RemoveItem(string cacheKey);
	}

	public interface IAsyncCacheStore
	{
		Task<object> GetItemAsync(string cacheKey);
		Task SetItemAsync(string cacheKey, object item, TimeSpan absoluteDuration);
		Task RemoveItemAsync(string cacheKey);
	}
}