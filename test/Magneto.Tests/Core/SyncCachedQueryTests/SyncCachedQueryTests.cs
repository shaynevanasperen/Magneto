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

		protected CacheOption CacheOption;
		protected QueryResult Result;

		public override void Setup()
		{
			SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());
			The<ISyncCachedQueryStub>().GetCacheEntryOptions(QueryContext).Returns(CacheEntryOptions);
			The<ISyncCachedQueryStub>().Query(QueryContext).Returns(QueryResult);
		}

		protected void WhenExecutingTheQuery() => Result = SUT.Execute(QueryContext, The<ISyncCacheStore<CacheEntryOptions>>(), CacheOption);
		protected void ThenTheCacheKeyContainsTheFullClassName() => SUT.State.CacheKey.Contains(SUT.GetType().FullName).Should().BeTrue();
		protected void AndThenNothingIsRemovedFromTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().DidNotReceive().Remove(Arg.Any<string>());
		protected void AndThenTheCachedResultIsSet() => SUT.State.CachedResult.Should().BeSameAs(Result);

		public abstract class CacheOptionIsDefault : Executing
		{
			protected void GivenCacheOptionIsDefault() => CacheOption = CacheOption.Default;
			protected void AndThenTheCacheStoreIsQueried() => The<ISyncCacheStore<CacheEntryOptions>>().Received().Get<QueryResult>(SUT.State.CacheKey);

			public class CacheMiss : CacheOptionIsDefault
			{
				void AndGivenThereIsACacheMiss() { }
				void AndThenTheQueryIsExecuted() => The<ISyncCachedQueryStub>().Received().Query(QueryContext);
				void AndThenTheCacheEntryOptionsAreRequested() => The<ISyncCachedQueryStub>().Received().GetCacheEntryOptions(QueryContext);
				void AndThenTheResultIsSetInTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received().Set(SUT.State.CacheKey, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == QueryResult), CacheEntryOptions);
				void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
			}

			public class CacheHit : CacheOptionIsDefault
			{
				readonly QueryResult _cachedResult = new QueryResult();

				void AndGivenThereIsACacheHit() => The<ISyncCacheStore<CacheEntryOptions>>().Get<QueryResult>(SUT.State.CacheKey).Returns(_cachedResult.ToCacheEntry());
				void AndThenTheQueryIsNotExecuted() => The<ISyncCachedQueryStub>().DidNotReceive().Query(QueryContext);
				void AndThenTheCacheEntryOptionsAreNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
				void AndThenTheResultIsNotSetInTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().DidNotReceive().Set(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>());
				void AndThenTheResultIsTheCachedResult() => Result.Should().BeSameAs(_cachedResult);
			}
		}

		public class CacheOptionIsRefresh : Executing
		{
			void GivenCacheOptionIsRefresh() => CacheOption = CacheOption.Refresh;
			void AndThenTheCacheStoreIsNotQueried() => The<ISyncCacheStore<CacheEntryOptions>>().DidNotReceive().Get<QueryResult>(Arg.Any<string>());
			void AndThenTheQueryIsExecuted() => The<ISyncCachedQueryStub>().Received().Query(QueryContext);
			void AndThenTheCacheEntryOptionsAreRequested() => The<ISyncCachedQueryStub>().Received().GetCacheEntryOptions(QueryContext);
			void AndThenTheResultIsSetInTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received().Set(SUT.State.CacheKey, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == QueryResult), CacheEntryOptions);
			void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
		}
	}

	public class EvictingCachedResult : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		public override void Setup() => SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());

		void WhenEvictingCachedResult() => SUT.EvictCachedResult(The<ISyncCacheStore<CacheEntryOptions>>());
		void ThenItDelegatesToTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received().Remove(SUT.State.CacheKey);
	}

	public abstract class UpdatingCachedResult : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		public override void Setup() => SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());

		public class QueryWasNotExecuted : UpdatingCachedResult
		{
			Action _invocation;
			
			void GivenTheQueryWasNotExecuted() { }
			void WhenUpdatingCachedResult() => _invocation = SUT.Invoking(x => x.UpdateCachedResult(The<ISyncCacheStore<CacheEntryOptions>>()));
			void ThenTheCacheEntryOptionsAreNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
			void AndThenItThrowsAnExceptionStatingThatTheCachedResultIsNotAvailable() => _invocation.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Cached result is not available");
		}

		public class QueryWasExecuted : UpdatingCachedResult
		{
			readonly QueryContext _queryContext = new QueryContext();
			readonly CacheEntryOptions _cacheEntryOptions = new CacheEntryOptions();
			readonly QueryResult _queryResult = new QueryResult();

			public override void Setup()
			{
				base.Setup();
				The<ISyncCachedQueryStub>().GetCacheEntryOptions(_queryContext).Returns(_cacheEntryOptions);
				The<ISyncCachedQueryStub>().Query(_queryContext).Returns(_queryResult);
			}

			void GivenTheQueryWasExecuted() => SUT.Execute(_queryContext, The<ISyncCacheStore<CacheEntryOptions>>());
			void WhenUpdatingCachedResult() => SUT.UpdateCachedResult(The<ISyncCacheStore<CacheEntryOptions>>());
			void ThenTheCacheEntryOptionsAreNotRequestedAgain() => The<ISyncCachedQueryStub>().Received(1).GetCacheEntryOptions(_queryContext);
			void AndThenItDelegatesToTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().Received(2).Set(SUT.State.CacheKey, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == _queryResult), _cacheEntryOptions);
		}
	}

	public interface ISyncCachedQueryStub
	{
		void ConfigureCache(ICacheConfig cacheConfig);
		CacheEntryOptions GetCacheEntryOptions(QueryContext context);
		QueryResult Query(QueryContext context);
	}

	public class ConcreteSyncCachedQuery : SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>
	{
		readonly ISyncCachedQueryStub _syncCachedQueryStub;

		public ConcreteSyncCachedQuery(ISyncCachedQueryStub syncCachedQueryStub) => _syncCachedQueryStub = syncCachedQueryStub;

		protected override void ConfigureCache(ICacheConfig cacheConfig) => _syncCachedQueryStub.ConfigureCache(cacheConfig);

		protected override CacheEntryOptions GetCacheEntryOptions(QueryContext context) => _syncCachedQueryStub.GetCacheEntryOptions(context);

		protected override QueryResult Query(QueryContext context) => _syncCachedQueryStub.Query(context);
	}
}
