using System;
using FluentAssertions;
using Magneto.Configuration;
using Magneto.Core;
using NSubstitute;

namespace Magneto.Tests.Core.SyncCachedQueryTests
{
	public abstract class Executing : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		protected readonly QueryContext QueryContext = new QueryContext();
		protected readonly CacheEntryOptions CacheEntryOptions = new CacheEntryOptions();
		protected readonly QueryResult QueryResult = new QueryResult();

		protected string ExpectedCacheKey;
		protected CacheOption CacheOption;
		protected QueryResult Result;

		public override void Setup()
		{
			SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());
			The<ISyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKeyConfig>())).Do(x => x.ArgAt<IKeyConfig>(0).VaryBy = nameof(IKeyConfig.VaryBy));
			ExpectedCacheKey = $"{SUT.GetType().FullName}_{nameof(IKeyConfig.VaryBy)}";
			The<ISyncCachedQueryStub>().CacheEntryOptions(QueryContext).Returns(CacheEntryOptions);
			The<ISyncCachedQueryStub>().Query(QueryContext).Returns(QueryResult);
		}

		protected void WhenExecutingTheQuery() => Result = SUT.Execute(QueryContext, The<ISyncCacheStore<CacheEntryOptions>>(), CacheOption);
		protected void ThenTheCacheKeyIsRequestedOnlyOnce() => The<ISyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKeyConfig>());
		protected void AndThenTheCacheKeyContainsTheFullClassName() => SUT.PeekCacheKey().Should().Contain(SUT.GetType().FullName);
		protected void AndThenTheCacheKeyContainsTheVaryByValue() => SUT.PeekCacheKey().Should().Contain(nameof(IKeyConfig.VaryBy));
		protected void AndThenNothingIsRemovedFromTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().DidNotReceive().Remove(Arg.Any<string>());
		protected void AndThenTheCachedResultIsSet() => SUT.PeekCachedResult().Should().BeSameAs(Result);

		public abstract class CacheOptionIsDefault : Executing
		{
			protected void GivenTheCacheOptionIsDefault() => CacheOption = CacheOption.Default;
			protected void AndThenTheCacheStoreIsQueried() => The<ISyncCacheStore<CacheEntryOptions>>().Received().Get<QueryResult>(ExpectedCacheKey);

			public class CacheMiss : CacheOptionIsDefault
			{
				void GivenTheQueryResultIsNotCached() { }
				void AndThenTheQueryIsExecuted() => The<ISyncCachedQueryStub>().Received(1).Query(QueryContext);
				void AndThenTheCacheEntryOptionsIsRequested() => The<ISyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
				void AndThenTheResultIsSetInTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received(1).Set(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions);
				void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
			}

			public class CacheHit : CacheOptionIsDefault
			{
				readonly QueryResult _cachedResult = new QueryResult();

				void GivenTheQueryResultIsCached() => The<ISyncCacheStore<CacheEntryOptions>>().Get<QueryResult>(ExpectedCacheKey).Returns(_cachedResult.ToCacheEntry());
				void AndThenTheQueryIsNotExecuted() => The<ISyncCachedQueryStub>().DidNotReceive().Query(QueryContext);
				void AndThenTheCacheEntryOptionsIsNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().CacheEntryOptions(Arg.Any<QueryContext>());
				void AndThenTheResultIsTheCachedResult() => Result.Should().BeSameAs(_cachedResult);
			}
		}

		public class CacheOptionIsRefresh : Executing
		{
			void GivenTheCacheOptionIsRefresh() => CacheOption = CacheOption.Refresh;
			void AndThenTheCacheStoreIsNotQueried() => The<ISyncCacheStore<CacheEntryOptions>>().DidNotReceive().Get<QueryResult>(Arg.Any<string>());
			void AndThenTheQueryIsExecuted() => The<ISyncCachedQueryStub>().Received(1).Query(QueryContext);
			void AndThenTheCacheEntryOptionsIsRequested() => The<ISyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
			void AndThenTheResultIsSetInTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received(1).Set(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions);
			void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
		}
	}

	public class ExecutingMultipleTimes : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		readonly QueryContext _queryContext = new QueryContext();

		QueryResult _result1;
		QueryResult _result2;
		string _cacheKey1;
		string _cacheKey2;
		CacheEntryOptions _cacheEntryOptions1;
		CacheEntryOptions _cacheEntryOptions2;
		QueryResult _cachedResult1;
		QueryResult _cachedResult2;

		public override void Setup()
		{
			SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());
			The<ISyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKeyConfig>())).Do(x => x.ArgAt<IKeyConfig>(0).VaryBy = Guid.NewGuid().ToString());
			The<ISyncCachedQueryStub>().CacheEntryOptions(_queryContext).Returns(x => new CacheEntryOptions());
			The<ISyncCachedQueryStub>().Query(_queryContext).Returns(x => new QueryResult());
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
		void AndThenTwoResultsAreDifferent() => _result1.Should().NotBeSameAs(_result2);
		void AndThenTwoStateCacheKeysAreDifferent() => _cacheKey1.Should().NotBeSameAs(_cacheKey2);
		void AndThenTwoStateCacheEntryOptionsAreDifferent() => _cacheEntryOptions1.Should().NotBeSameAs(_cacheEntryOptions2);
		void AndThenTwoStateCachedResultsAreDifferent() => _cachedResult1.Should().NotBeSameAs(_cachedResult2);
	}

	public class EvictingCachedResult : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		public override void Setup() => SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());

		void WhenEvictingCachedResult() => SUT.EvictCachedResult(The<ISyncCacheStore<CacheEntryOptions>>());
		void ThenItDelegatesToTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received(1).Remove(SUT.PeekCacheKey());
		void AndThenTheCacheKeyIsRequestedOnlyOnce() => The<ISyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKeyConfig>());
	}

	public abstract class UpdatingCachedResult : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		protected readonly QueryContext QueryContext = new QueryContext();
		protected readonly CacheEntryOptions CacheEntryOptions = new CacheEntryOptions();

		protected string ExpectedCacheKey;

		public override void Setup()
		{
			SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());
			The<ISyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKeyConfig>())).Do(x => x.ArgAt<IKeyConfig>(0).VaryBy = nameof(IKeyConfig.VaryBy));
			ExpectedCacheKey = $"{SUT.GetType().FullName}_{nameof(IKeyConfig.VaryBy)}";
			The<ISyncCachedQueryStub>().CacheEntryOptions(QueryContext).Returns(CacheEntryOptions);
		}

		public class QueryWasNotExecuted : UpdatingCachedResult
		{
			Action _invocation;

			void GivenTheQueryWasNotExecuted() { }
			void WhenUpdatingCachedResult() => _invocation = SUT.Invoking(x => x.UpdateCachedResult(The<ISyncCacheStore<CacheEntryOptions>>()));
			void ThenTheCacheKeyIsNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().CacheKey(Arg.Any<IKeyConfig>());
			void AndThenItThrowsAnExceptionStatingThatTheCachedResultIsNotAvailable() => _invocation.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Cached result is not available");
			void AndThenTheCacheEntryOptionsIsNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().CacheEntryOptions(Arg.Any<QueryContext>());
		}

		public abstract class QueryWasExecuted : UpdatingCachedResult
		{
			protected readonly QueryResult QueryResult = new QueryResult();

			protected int ExpectedCacheStoreSetCount;

			protected void AndGivenTheQueryWasExecuted() => SUT.Execute(QueryContext, The<ISyncCacheStore<CacheEntryOptions>>(), CacheOption.Default);
			protected void WhenUpdatingCachedResult() => SUT.UpdateCachedResult(The<ISyncCacheStore<CacheEntryOptions>>());
			protected void ThenTheCacheKeyIsRequestedOnlyOnce() => The<ISyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKeyConfig>());
			protected void AndThenTheCacheEntryOptionsIsNotRequestedAgain() => The<ISyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
			protected void AndThenItDelegatesToTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received(ExpectedCacheStoreSetCount).Set(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions);

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
					The<ISyncCacheStore<CacheEntryOptions>>().Get<QueryResult>(ExpectedCacheKey).Returns(QueryResult.ToCacheEntry());
					ExpectedCacheStoreSetCount = 1;
				}
			}
		}
	}

	public interface ISyncCachedQueryStub
	{
		void CacheKey(IKeyConfig keyConfig);
		CacheEntryOptions CacheEntryOptions(QueryContext context);
		QueryResult Query(QueryContext context);
	}

	public class ConcreteSyncCachedQuery : SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>
	{
		readonly ISyncCachedQueryStub _syncCachedQueryStub;

		public ConcreteSyncCachedQuery(ISyncCachedQueryStub syncCachedQueryStub) => _syncCachedQueryStub = syncCachedQueryStub;

		protected override void CacheKey(IKeyConfig keyConfig) => _syncCachedQueryStub.CacheKey(keyConfig);

		protected override CacheEntryOptions CacheEntryOptions(QueryContext context) => _syncCachedQueryStub.CacheEntryOptions(context);

		protected override QueryResult Query(QueryContext context) => _syncCachedQueryStub.Query(context);
	}
}
