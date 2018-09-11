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
		protected readonly QueryResult QueryResult = new QueryResult();

		protected CacheOption CacheOption;
		protected QueryResult Result;

		public override void Setup()
		{
			SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());
			The<IDecorator>().Decorate(Arg.Any<object>(), Arg.Any<QueryContext>(), Arg.Any<Func<QueryContext, QueryResult>>()).Returns(x => x.ArgAt<Func<QueryContext, QueryResult>>(2).Invoke(QueryContext));
			The<ISyncCachedQueryStub>().Query(QueryContext).Returns(QueryResult);
		}

		protected void WhenExecutingTheQuery() => Result = SUT.Execute(QueryContext, The<IDecorator>(), The<ISyncQueryCache<CacheEntryOptions>>(), CacheOption);
		protected void ThenTheCacheKeyContainsTheFullClassName() => SUT.State.CacheInfo.Key.Contains(SUT.GetType().FullName).Should().BeTrue();
		protected void AndThenTheCachedResultIsSet() => SUT.State.CachedResult.Should().BeSameAs(Result);

		public abstract class CacheOptionIsDefault : Executing
		{
			protected void GivenCacheOptionIsDefault() => CacheOption = CacheOption.Default;
			protected void AndThenTheQueryCacheIsQueried() => The<ISyncQueryCache<CacheEntryOptions>>().Received().Get(Arg.Any<Func<QueryResult>>(), SUT.State.CacheInfo, Arg.Any<Func<CacheEntryOptions>>());
			protected void AndThenNothingIsSetInTheQueryCache() => The<ISyncQueryCache<CacheEntryOptions>>().DidNotReceive().Set(Arg.Any<QueryResult>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>());

			public abstract class ResultIsNotInCache : CacheOptionIsDefault
			{
				protected Func<CacheEntryOptions> GetCacheEntryOptions;

				protected void AndGivenResultIsNotInTheQueryCache() => The<ISyncQueryCache<CacheEntryOptions>>()
					.Get(Arg.Any<Func<QueryResult>>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>())
					.Returns(x =>
					{
						GetCacheEntryOptions = x.ArgAt<Func<CacheEntryOptions>>(2);
						return x.ArgAt<Func<QueryResult>>(0).Invoke();
					});
				protected void AndThenTheDecoratorIsInvoked() => The<IDecorator>().Received().Decorate(SUT, QueryContext, Arg.Any<Func<QueryContext, QueryResult>>());
				protected void AndThenTheQueryIsExecuted() => The<ISyncCachedQueryStub>().Received().Query(QueryContext);
				protected void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);

				public class QueryCacheDoesNotRequestCacheEntryOptions : ResultIsNotInCache
				{
					void AndWhenTheQueryCacheDoesNotRequestCacheEntryOptions() { }
					void AndThenTheCacheEntryOptionsAreNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
				}

				public class QueryCacheRequestsCacheEntryOptions : ResultIsNotInCache
				{
					void AndWhenTheQueryCacheRequestsCacheEntryOptions() => GetCacheEntryOptions();
					void AndThenTheCacheEntryOptionsAreRequested() => The<ISyncCachedQueryStub>().Received().GetCacheEntryOptions(QueryContext);
				}
			}

			public class ResultIsInCache : CacheOptionIsDefault
			{
				readonly QueryResult _cachedResult = new QueryResult();

				void AndGivenResultIsInTheQueryCache() => The<ISyncQueryCache<CacheEntryOptions>>().Get(Arg.Any<Func<QueryResult>>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>()).Returns(_cachedResult);
				void AndThenTheDecoratorIsNotInvoked() => The<IDecorator>().DidNotReceive().Decorate(Arg.Any<object>(), Arg.Any<QueryContext>(), Arg.Any<Func<QueryContext, QueryResult>>());
				void AndThenTheQueryIsNotExecuted() => The<ISyncCachedQueryStub>().DidNotReceive().Query(QueryContext);
				void AndThenTheCacheEntryOptionsAreNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
				void AndThenTheResultIsTheCachedResult() => Result.Should().BeSameAs(_cachedResult);
			}
		}

		public abstract class CacheOptionIsRefresh : Executing
		{
			protected Func<CacheEntryOptions> GetCacheEntryOptions;

			public override void Setup()
			{
				base.Setup();
				The<ISyncQueryCache<CacheEntryOptions>>()
					.When(x => x.Set(Arg.Any<QueryResult>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>()))
					.Do(x => GetCacheEntryOptions = x.ArgAt<Func<CacheEntryOptions>>(2));
			}

			protected void GivenCacheOptionIsRefresh() => CacheOption = CacheOption.Refresh;
			protected void AndThenTheQueryCacheIsNotQueried() => The<ISyncQueryCache<CacheEntryOptions>>().DidNotReceive().Get(Arg.Any<Func<QueryResult>>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>());
			protected void AndThenTheDecoratorIsInvoked() => The<IDecorator>().Received().Decorate(SUT, QueryContext, Arg.Any<Func<QueryContext, QueryResult>>());
			protected void AndThenTheQueryIsExecuted() => The<ISyncCachedQueryStub>().Received().Query(QueryContext);
			protected void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
			protected void AndThenTheResultIsSetInTheQueryCache() => The<ISyncQueryCache<CacheEntryOptions>>().Received().Set(QueryResult, SUT.State.CacheInfo, Arg.Any<Func<CacheEntryOptions>>());

			public class QueryCacheDoesNotRequestCacheEntryOptions : CacheOptionIsRefresh
			{
				void AndWhenTheQueryCacheDoesNotRequestCacheEntryOptions() { }
				void AndThenTheCacheEntryOptionsAreNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
			}

			public class QueryCacheRequestsCacheEntryOptions : CacheOptionIsRefresh
			{
				void AndWhenTheQueryCacheRequestsCacheEntryOptions() => GetCacheEntryOptions();
				void AndThenTheCacheEntryOptionsAreRequested() => The<ISyncCachedQueryStub>().Received().GetCacheEntryOptions(QueryContext);
			}
		}
	}

	public class EvictingCachedResult : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		public override void Setup() => SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());

		void WhenEvictingCachedResult() => SUT.EvictCachedResult(The<ISyncQueryCache<CacheEntryOptions>>());
		void ThenItDelegatesToTheQueryCache() => The<ISyncQueryCache<CacheEntryOptions>>().Received().Evict(SUT.State.CacheInfo.Key);
	}

	public abstract class UpdatingCachedResult : ScenarioFor<SyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		public override void Setup() => SUT = new ConcreteSyncCachedQuery(The<ISyncCachedQueryStub>());

		public class QueryWasNotExecuted : UpdatingCachedResult
		{
			Action _invocation;
			
			void GivenTheQueryWasNotExecuted() { }
			void WhenUpdatingCachedResult() => _invocation = SUT.Invoking(x => x.UpdateCachedResult(The<ISyncQueryCache<CacheEntryOptions>>()));
			void ThenTheCacheEntryOptionsAreNotRequested() => The<ISyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
			void AndThenItThrowsAnExceptionStatingThatTheCachedResultIsNotAvailable() => _invocation.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Cached result is not available");
		}

		public abstract class QueryWasExecuted : UpdatingCachedResult
		{
			protected readonly QueryContext QueryContext = new QueryContext();
			protected Func<CacheEntryOptions> GetCacheEntryOptions;

			protected void WhenUpdatingCachedResult() => SUT.UpdateCachedResult(The<ISyncQueryCache<CacheEntryOptions>>());
			protected void ThenItDelegatesToTheQueryCache() => The<ISyncQueryCache<CacheEntryOptions>>().Received(2).Set(SUT.State.CachedResult, SUT.State.CacheInfo, Arg.Any<Func<CacheEntryOptions>>());
			protected void AndThenTheCacheEntryOptionsAreNotRequestedAgain() => The<ISyncCachedQueryStub>().Received(1).GetCacheEntryOptions(QueryContext);

			public class CacheOptionWasDefault : QueryWasExecuted
			{
				void GivenTheQueryCacheRequestsCacheEntryOptions()
				{
					The<ISyncQueryCache<CacheEntryOptions>>().Get(Arg.Any<Func<QueryResult>>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>()).Returns(x =>
					{
						var result = new QueryResult();
						The<ISyncQueryCache<CacheEntryOptions>>().Set(result, x.ArgAt<ICacheInfo>(1), x.ArgAt<Func<CacheEntryOptions>>(2));
						return result;
					});
					The<ISyncQueryCache<CacheEntryOptions>>().When(x => x.Set(Arg.Any<QueryResult>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>())).Do(x => x.ArgAt<Func<CacheEntryOptions>>(2).Invoke());
				}
				void AndGivenTheQueryWasExecutedWithCacheOptionOfDefault() => SUT.Execute(QueryContext, The<ISyncDecorator>(), The<ISyncQueryCache<CacheEntryOptions>>());
			}

			public class CacheOptionWasRefresh : QueryWasExecuted
			{
				void GivenTheQueryCacheRequestsCacheEntryOptions() => The<ISyncQueryCache<CacheEntryOptions>>()
					.When(x => x.Set(Arg.Any<QueryResult>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>()))
					.Do(x => x.ArgAt<Func<CacheEntryOptions>>(2).Invoke());
				void AndGivenTheQueryWasExecutedWithCacheOptionOfRefresh() => SUT.Execute(QueryContext, The<ISyncDecorator>(), The<ISyncQueryCache<CacheEntryOptions>>(), CacheOption.Refresh);
			}
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

		protected override void ConfigureCache(ICacheConfig cacheConfig)
		{
			_syncCachedQueryStub.ConfigureCache(cacheConfig);
		}

		protected override CacheEntryOptions GetCacheEntryOptions(QueryContext context) => _syncCachedQueryStub.GetCacheEntryOptions(context);

		protected override QueryResult Query(QueryContext context) => _syncCachedQueryStub.Query(context);
	}
}
