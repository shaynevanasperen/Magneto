using FluentAssertions;
using Magneto.Configuration;
using Magneto.Core;
using NSubstitute;
using System;

namespace Magneto.Tests.Core.SyncCachedQueryTests;

public abstract class Executing : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
{
	protected readonly QueryContext QueryContext = new();
	protected readonly CacheEntryOptions CacheEntryOptions = new();
	protected readonly QueryResult QueryResult = new();

	protected string ExpectedCacheKey = null!;
	protected CacheOption CacheOption;
	protected QueryResult Result = null!;

	public void Setup()
	{
		SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());
		The<ISyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKey>())).Do(x => x.ArgAt<IKey>(0).VaryBy = nameof(IKey.VaryBy));
		ExpectedCacheKey = $"{SUT.GetType().FullName}_{nameof(IKey.VaryBy)}";
		The<ISyncCachedQueryStub>().CacheEntryOptions(QueryContext).Returns(CacheEntryOptions);
		The<ISyncCachedQueryStub>().Query(QueryContext).Returns(QueryResult);
	}

	protected void WhenExecutingTheQuery() => Result = SUT.Execute(QueryContext, The<ISyncCacheStore<CacheEntryOptions>>(), CacheOption);
	protected void ThenTheCacheKeyIsRequestedOnlyOnce() => The<ISyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKey>());
	protected void AndThenTheCacheKeyContainsTheFullClassName() => SUT.PeekCacheKey().Should().Contain(SUT.GetType().FullName);
	protected void AndThenTheCacheKeyContainsTheVaryByValue() => SUT.PeekCacheKey().Should().Contain(nameof(IKey.VaryBy));
	protected void AndThenNothingIsRemovedFromTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().DidNotReceive().RemoveEntry(Arg.Any<string>());
	protected void AndThenTheCachedResultIsSet() => SUT.PeekCachedResult().Should().BeSameAs(Result);

	public abstract class CacheOptionIsDefault : Executing
	{
		protected void GivenTheCacheOptionIsDefault() => CacheOption = CacheOption.Default;
		protected void AndThenTheCacheStoreIsQueried() => The<ISyncCacheStore<CacheEntryOptions>>().Received().GetEntry<QueryResult>(ExpectedCacheKey);

		public class CacheMiss : CacheOptionIsDefault
		{
			void GivenTheQueryResultIsNotCached() { }
			void AndThenTheQueryIsExecuted() => The<ISyncCachedQueryStub>().Received(1).Query(QueryContext);
			void AndThenTheCacheEntryOptionsIsRequested() => The<ISyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
			void AndThenTheResultIsSetInTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received(1).SetEntry(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions);
			void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
		}

		public class CacheHit : CacheOptionIsDefault
		{
			readonly QueryResult _cachedResult = new();

			void GivenTheQueryResultIsCached() => The<ISyncCacheStore<CacheEntryOptions>>().GetEntry<QueryResult>(ExpectedCacheKey).Returns(_cachedResult.ToCacheEntry());
			void AndThenTheQueryIsNotExecuted() => The<ISyncCachedQueryStub>().DidNotReceive().Query(QueryContext);
			void AndThenTheCacheEntryOptionsIsNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().CacheEntryOptions(Arg.Any<QueryContext>());
			void AndThenTheResultIsTheCachedResult() => Result.Should().BeSameAs(_cachedResult);
		}
	}

	public class CacheOptionIsRefresh : Executing
	{
		void GivenTheCacheOptionIsRefresh() => CacheOption = CacheOption.Refresh;
		void AndThenTheCacheStoreIsNotQueried() => The<ISyncCacheStore<CacheEntryOptions>>().DidNotReceive().GetEntry<QueryResult>(Arg.Any<string>());
		void AndThenTheQueryIsExecuted() => The<ISyncCachedQueryStub>().Received(1).Query(QueryContext);
		void AndThenTheCacheEntryOptionsIsRequested() => The<ISyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
		void AndThenTheResultIsSetInTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received(1).SetEntry(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions);
		void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
	}
}

public class ExecutingMultipleTimes : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
{
	readonly QueryContext _queryContext = new();

	QueryResult _result1 = null!;
	QueryResult _result2 = null!;
	string _cacheKey1 = null!;
	string _cacheKey2 = null!;
	CacheEntryOptions _cacheEntryOptions1 = null!;
	CacheEntryOptions _cacheEntryOptions2 = null!;
	QueryResult _cachedResult1 = null!;
	QueryResult _cachedResult2 = null!;

	public void Setup()
	{
		SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());
		The<ISyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKey>())).Do(x => x.ArgAt<IKey>(0).VaryBy = Guid.NewGuid().ToString());
		The<ISyncCachedQueryStub>().CacheEntryOptions(_queryContext).Returns(x => new());
		The<ISyncCachedQueryStub>().Query(_queryContext).Returns(x => new());
	}

	void WhenExecutingTheQuery()
	{
		_result1 = SUT.Execute(_queryContext, The<ISyncCacheStore<CacheEntryOptions>>(), CacheOption.Default);
		_cacheKey1 = SUT.PeekCacheKey();
		_cacheEntryOptions1 = SUT.PeekCacheEntryOptions();
		_cachedResult1 = SUT.PeekCachedResult();
	}

	void AndWhenExecutingTheQueryAgain()
	{
		_result2 = SUT.Execute(_queryContext, The<ISyncCacheStore<CacheEntryOptions>>(), CacheOption.Default);
		_cacheKey2 = SUT.PeekCacheKey();
		_cacheEntryOptions2 = SUT.PeekCacheEntryOptions();
		_cachedResult2 = SUT.PeekCachedResult();
	}

	void ThenTheQueryIsExecutedTwice() => The<ISyncCachedQueryStub>().Received(2).Query(_queryContext);
	void AndThenTheTwoResultsAreDifferent() => _result1.Should().NotBeSameAs(_result2);
	void AndThenTheTwoStateCacheKeysAreDifferent() => _cacheKey1.Should().NotBeSameAs(_cacheKey2);
	void AndThenTheTwoStateCacheEntryOptionsAreDifferent() => _cacheEntryOptions1.Should().NotBeSameAs(_cacheEntryOptions2);
	void AndThenTheTwoStateCachedResultsAreDifferent() => _cachedResult1.Should().NotBeSameAs(_cachedResult2);
}

public class EvictingCachedResult : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
{
	public void Setup() => SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());

	void WhenEvictingCachedResult() => SUT.EvictCachedResult(The<ISyncCacheStore<CacheEntryOptions>>());
	void ThenItDelegatesToTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received(1).RemoveEntry(SUT.PeekCacheKey());
	void AndThenTheCacheKeyIsRequestedOnlyOnce() => The<ISyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKey>());
}

public abstract class UpdatingCachedResult : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
{
	protected readonly QueryContext QueryContext = new();
	protected readonly CacheEntryOptions CacheEntryOptions = new();

	protected string ExpectedCacheKey = null!;

	public void Setup()
	{
		SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());
		The<ISyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKey>())).Do(x => x.ArgAt<IKey>(0).VaryBy = nameof(IKey.VaryBy));
		ExpectedCacheKey = $"{SUT.GetType().FullName}_{nameof(IKey.VaryBy)}";
		The<ISyncCachedQueryStub>().CacheEntryOptions(QueryContext).Returns(CacheEntryOptions);
	}

	public class QueryWasNotExecuted : UpdatingCachedResult
	{
		Action _invocation = null!;

		void GivenTheQueryWasNotExecuted() { }
		void WhenUpdatingCachedResult() => _invocation = SUT.Invoking(x => x.UpdateCachedResult(The<ISyncCacheStore<CacheEntryOptions>>()));
		void ThenTheCacheKeyIsNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().CacheKey(Arg.Any<IKey>());
		void AndThenItThrowsAnExceptionStatingThatTheCachedResultIsNotAvailable() => _invocation.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Cached result is not available");
		void AndThenTheCacheEntryOptionsIsNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().CacheEntryOptions(Arg.Any<QueryContext>());
	}

	public abstract class QueryWasExecuted : UpdatingCachedResult
	{
		protected readonly QueryResult QueryResult = new();

		protected int ExpectedCacheStoreSetCount;

		protected void AndGivenTheQueryWasExecuted() => SUT.Execute(QueryContext, The<ISyncCacheStore<CacheEntryOptions>>(), CacheOption.Default);
		protected void WhenUpdatingCachedResult() => SUT.UpdateCachedResult(The<ISyncCacheStore<CacheEntryOptions>>());
		protected void ThenTheCacheKeyIsRequestedOnlyOnce() => The<ISyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKey>());
		protected void AndThenTheCacheEntryOptionsIsNotRequestedAgain() => The<ISyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
		protected void AndThenItDelegatesToTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received(ExpectedCacheStoreSetCount).SetEntry(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions);

		public class CacheMiss : QueryWasExecuted
		{
			void GivenTheQueryResultIsNotCached()
			{
				The<ISyncCachedQueryStub>().Query(QueryContext).Returns(QueryResult);
				ExpectedCacheStoreSetCount = 2;
			}
		}

		public class CacheHit : QueryWasExecuted
		{
			void GivenTheQueryResultIsCached()
			{
				The<ISyncCacheStore<CacheEntryOptions>>().GetEntry<QueryResult>(ExpectedCacheKey).Returns(QueryResult.ToCacheEntry());
				ExpectedCacheStoreSetCount = 1;
			}
		}
	}
}

public interface ISyncCachedQueryStub
{
	void CacheKey(IKey key);
	CacheEntryOptions CacheEntryOptions(QueryContext context);
	QueryResult Query(QueryContext context);
}

public class ConcreteSyncCachedQuery(ISyncCachedQueryStub syncCachedQueryStub) : SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>
{
	protected override void CacheKey(IKey key) => syncCachedQueryStub.CacheKey(key);

	protected override CacheEntryOptions CacheEntryOptions(QueryContext context) => syncCachedQueryStub.CacheEntryOptions(context);

	protected override QueryResult Query(QueryContext context) => syncCachedQueryStub.Query(context);
}
