using System;
using System.Threading.Tasks;
using FluentAssertions;
using Magneto.Configuration;
using Magneto.Core;
using NSubstitute;

namespace Magneto.Tests.Core.AsyncCachedQueryTests
{
	public abstract class Executing : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		protected readonly QueryContext QueryContext = new QueryContext();
		protected readonly QueryResult QueryResult = new QueryResult();

		protected CacheOption CacheOption;
		protected object Result;

		protected virtual void Setup()
		{
			SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());
			The<IDecorator>().Decorate(Arg.Any<object>(), Arg.Any<QueryContext>(), Arg.Any<Func<QueryContext, Task<QueryResult>>>()).Returns(x => x.ArgAt<Func<QueryContext, Task<QueryResult>>>(2).Invoke(QueryContext));
			The<IAsyncCachedQueryStub>().QueryAsync(QueryContext).Returns(QueryResult);
		}

		protected async Task WhenExecutingTheQuery() => Result = await SUT.ExecuteAsync(QueryContext, The<IDecorator>(), The<IAsyncQueryCache<CacheEntryOptions>>(), CacheOption);
		protected void ThenTheCacheKeyContainsTheFullClassName() => SUT.State.CacheInfo.Key.Contains(SUT.GetType().FullName).Should().BeTrue();
		protected void AndThenTheCachedResultIsSet() => SUT.State.CachedResult.Should().BeSameAs(Result);

		public abstract class CacheOptionIsDefault : Executing
		{
			protected void GivenCacheOptionIsDefault() => CacheOption = CacheOption.Default;
			protected void AndThenTheQueryCacheIsQueried() => The<IAsyncQueryCache<CacheEntryOptions>>().Received().GetAsync(Arg.Any<Func<Task<QueryResult>>>(), SUT.State.CacheInfo, Arg.Any<Func<CacheEntryOptions>>());
			protected void AndThenNothingIsSetInTheQueryCache() => The<IAsyncQueryCache<CacheEntryOptions>>().DidNotReceive().SetAsync(Arg.Any<QueryResult>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>());

			public abstract class ResultIsNotInCache : CacheOptionIsDefault
			{
				protected Func<CacheEntryOptions> GetCacheEntryOptions;

				protected void AndGivenResultIsNotInTheQueryCache() => The<IAsyncQueryCache<CacheEntryOptions>>()
					.GetAsync(Arg.Any<Func<Task<QueryResult>>>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>())
					.Returns(x =>
					{
						GetCacheEntryOptions = x.ArgAt<Func<CacheEntryOptions>>(2);
						return x.ArgAt<Func<Task<QueryResult>>>(0).Invoke();
					});
				protected void AndThenTheDecoratorIsInvoked() => The<IDecorator>().Received().Decorate(SUT, QueryContext, Arg.Any<Func<QueryContext, Task<QueryResult>>>());
				protected void AndThenTheQueryIsExecuted() => The<IAsyncCachedQueryStub>().Received().QueryAsync(QueryContext);
				protected void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);

				public class QueryCacheDoesNotRequestCacheEntryOptions : ResultIsNotInCache
				{
					void AndWhenTheQueryCacheDoesNotRequestCacheEntryOptions() { }
					void AndThenTheCacheEntryOptionsAreNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
				}

				public class QueryCacheRequestsCacheEntryOptions : ResultIsNotInCache
				{
					void AndWhenTheQueryCacheRequestsCacheEntryOptions() => GetCacheEntryOptions();
					void AndThenTheCacheEntryOptionsAreRequested() => The<IAsyncCachedQueryStub>().Received().GetCacheEntryOptions(QueryContext);
				}
			}

			public class ResultIsInCache : CacheOptionIsDefault
			{
				readonly QueryResult _cachedResult = new QueryResult();

				void AndGivenResultIsInTheQueryCache() => The<IAsyncQueryCache<CacheEntryOptions>>().GetAsync(Arg.Any<Func<Task<QueryResult>>>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>()).Returns(x => _cachedResult);
				void AndThenTheDecoratorIsNotInvoked() => The<IDecorator>().DidNotReceive().Decorate(Arg.Any<object>(), Arg.Any<QueryContext>(), Arg.Any<Func<QueryContext, Task<QueryResult>>>());
				void AndThenTheQueryIsNotExecuted() => The<IAsyncCachedQueryStub>().DidNotReceive().QueryAsync(QueryContext);
				void AndThenTheCacheEntryOptionsAreNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
				void AndThenTheResultIsTheCachedResult() => Result.Should().BeSameAs(_cachedResult);
			}
		}

		public abstract class CacheOptionIsRefresh : Executing
		{
			protected Func<CacheEntryOptions> GetCacheEntryOptions;

			protected override void Setup()
			{
				base.Setup();
				The<IAsyncQueryCache<CacheEntryOptions>>()
					.When(x => x.SetAsync(Arg.Any<QueryResult>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>()))
					.Do(x => GetCacheEntryOptions = x.ArgAt<Func<CacheEntryOptions>>(2));
			}

			protected void GivenCacheOptionIsRefresh() => CacheOption = CacheOption.Refresh;
			protected void AndThenTheQueryCacheIsNotQueried() => The<IAsyncQueryCache<CacheEntryOptions>>().DidNotReceive().GetAsync(Arg.Any<Func<Task<QueryResult>>>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>());
			protected void AndThenTheDecoratorIsInvoked() => The<IDecorator>().Received().Decorate(SUT, QueryContext, Arg.Any<Func<QueryContext, Task<QueryResult>>>());
			protected void AndThenTheQueryIsExecuted() => The<IAsyncCachedQueryStub>().Received().QueryAsync(QueryContext);
			protected void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
			protected void AndThenTheResultIsSetInTheQueryCache() => The<IAsyncQueryCache<CacheEntryOptions>>().Received().SetAsync(QueryResult, SUT.State.CacheInfo, Arg.Any<Func<CacheEntryOptions>>());

			public class QueryCacheDoesNotRequestCacheEntryOptions : CacheOptionIsRefresh
			{
				void AndWhenTheQueryCacheDoesNotRequestCacheEntryOptions() { }
				void AndThenTheCacheEntryOptionsAreNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
			}

			public class QueryCacheRequestsCacheEntryOptions : CacheOptionIsRefresh
			{
				void AndWhenTheQueryCacheRequestsCacheEntryOptions() => GetCacheEntryOptions();
				void AndThenTheCacheEntryOptionsAreRequested() => The<IAsyncCachedQueryStub>().Received().GetCacheEntryOptions(QueryContext);
			}
		}
	}

	public class EvictingCachedResult : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		void Setup() => SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());
		
		void WhenEvictingCachedResult() => SUT.EvictCachedResultAsync(The<IAsyncQueryCache<object>>());
		void ThenItDelegatesToTheQueryCache() => The<IAsyncQueryCache<object>>().Received().EvictAsync(SUT.State.CacheInfo.Key);
	}

	public abstract class UpdatingCachedResult : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		protected void Setup() => SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());

		public class QueryWasNotExecuted : UpdatingCachedResult
		{
			Action _invocation;

			void GivenTheQueryWasNotExecuted() { }
			void WhenUpdatingCachedResult() => _invocation = SUT.Invoking(x => x.UpdateCachedResultAsync(The<IAsyncQueryCache<CacheEntryOptions>>()));
			void ThenTheCacheEntryOptionsAreNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
			void AndThenItThrowsAnExceptionStatingThatTheCachedResultIsNotAvailable() => _invocation.ShouldThrow<InvalidOperationException>().Which.Message.Should().Be("Cached result is not available");
		}

		public abstract class QueryWasExecuted : UpdatingCachedResult
		{
			protected readonly QueryContext QueryContext = new QueryContext();
			protected Func<CacheEntryOptions> GetCacheEntryOptions;

			protected void WhenUpdatingCachedResult() => SUT.UpdateCachedResultAsync(The<IAsyncQueryCache<CacheEntryOptions>>());
			protected void ThenItDelegatesToTheQueryCache() => The<IAsyncQueryCache<CacheEntryOptions>>().Received(2).SetAsync(SUT.State.CachedResult, SUT.State.CacheInfo, Arg.Any<Func<CacheEntryOptions>>());
			protected void AndThenTheCacheEntryOptionsAreNotRequestedAgain() => The<IAsyncCachedQueryStub>().Received(1).GetCacheEntryOptions(QueryContext);

			public class CacheOptionWasDefault : QueryWasExecuted
			{
				void GivenTheQueryCacheRequestsCacheEntryOptions()
				{
					The<IAsyncQueryCache<CacheEntryOptions>>().GetAsync(Arg.Any<Func<Task<QueryResult>>>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>()).Returns(async x =>
					{
						var result = new QueryResult();
						await The<IAsyncQueryCache<CacheEntryOptions>>().SetAsync(result, x.ArgAt<ICacheInfo>(1), x.ArgAt<Func<CacheEntryOptions>>(2));
						return result;
					});
					The<IAsyncQueryCache<CacheEntryOptions>>().When(x => x.SetAsync(Arg.Any<QueryResult>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>())).Do(x => x.ArgAt<Func<CacheEntryOptions>>(2).Invoke());
				}
				void AndGivenTheQueryWasExecutedWithCacheOptionOfDefault() => SUT.ExecuteAsync(QueryContext, The<IAsyncDecorator>(), The<IAsyncQueryCache<CacheEntryOptions>>());
			}

			public class CacheOptionWasRefresh : QueryWasExecuted
			{
				void GivenTheQueryCacheRequestsCacheEntryOptions() => The<IAsyncQueryCache<CacheEntryOptions>>()
					.When(x => x.SetAsync(Arg.Any<QueryResult>(), Arg.Any<ICacheInfo>(), Arg.Any<Func<CacheEntryOptions>>()))
					.Do(x => x.ArgAt<Func<CacheEntryOptions>>(2).Invoke());
				void AndGivenTheQueryWasExecutedWithCacheOptionOfRefresh() => SUT.ExecuteAsync(QueryContext, The<IAsyncDecorator>(), The<IAsyncQueryCache<CacheEntryOptions>>(), CacheOption.Refresh);
			}
		}
	}

	public interface IAsyncCachedQueryStub
	{
		void ConfigureCache(ICacheConfig cacheConfig);
		CacheEntryOptions GetCacheEntryOptions(QueryContext context);
		Task<QueryResult> QueryAsync(QueryContext context);
	}

	public class ConcreteAsyncCachedQuery : AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>
	{
		readonly IAsyncCachedQueryStub _asyncCachedQueryStub;

		public ConcreteAsyncCachedQuery(IAsyncCachedQueryStub asyncCachedQueryStub) => _asyncCachedQueryStub = asyncCachedQueryStub;

		protected override void ConfigureCache(ICacheConfig cacheConfig)
		{
			_asyncCachedQueryStub.ConfigureCache(cacheConfig);
		}

		protected override CacheEntryOptions GetCacheEntryOptions(QueryContext context) => _asyncCachedQueryStub.GetCacheEntryOptions(context);

		protected override Task<QueryResult> QueryAsync(QueryContext context) => _asyncCachedQueryStub.QueryAsync(context);
	}
}
