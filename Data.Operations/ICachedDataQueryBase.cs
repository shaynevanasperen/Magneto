namespace Data.Operations
{
	public interface ICachedDataQueryBase<out TCachedResult>
	{
		ICacheInfo GetCacheInfo();
		TCachedResult CachedResult { get; }
	}
}
