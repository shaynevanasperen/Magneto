namespace Data.Operations
{
	public abstract class CachedDataQueryBase<TCachedResult> : DataOperation, ICachedDataQueryBase<TCachedResult>
	{
		public virtual ICacheInfo GetCacheInfo()
		{
			var cacheInfo = new CacheInfo(GetType().FullName);
			ConfigureCache(cacheInfo);
			return cacheInfo;
		}

		public virtual TCachedResult CachedResult { get; protected set; }

		protected abstract void ConfigureCache(ICacheInfo cacheInfo);
	}
}