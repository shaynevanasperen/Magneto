using System;
using System.Threading;
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
		protected readonly CacheEntryOptions CacheEntryOptions = new CacheEntryOptions();
		protected readonly QueryResult QueryResult = new QueryResult();

		protected CacheOption CacheOption;
		protected CancellationToken CancellationToken = new CancellationTokenSource().Token;
		protected QueryResult Result;

		public override void Setup()
		{
			SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());
			The<IAsyncCachedQueryStub>().When(x => x.ConfigureCache(Arg.Any<ICacheConfig>())).Do(x => x.ArgAt<ICacheConfig>(0).VaryBy = nameof(ICacheConfig.VaryBy));
			The<IAsyncCachedQueryStub>().GetCacheEntryOptions(QueryContext).Returns(CacheEntryOptions);
			The<IAsyncCachedQueryStub>().QueryAsync(QueryContext, CancellationToken).Returns(QueryResult);
		}

		protected async Task WhenExecutingTheQuery() => Result = await SUT.ExecuteAsync(QueryContext, The<IAsyncCacheStore<CacheEntryOptions>>(), CacheOption, CancellationToken);
		protected void ThenTheCacheKeyContainsTheFullClassName() => SUT.State.CacheKey.Contains(SUT.GetType().FullName).Should().BeTrue();
		protected void AndTheCacheKeyContainsTheVaryByValue() => SUT.State.CacheKey.Contains(nameof(ICacheConfig.VaryBy)).Should().BeTrue();
		protected void AndThenNothingIsRemovedFromTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().DidNotReceive().Remove(Arg.Any<string>());
		protected void AndThenTheCachedResultIsSet() => SUT.State.CachedResult.Should().BeSameAs(Result);

		public abstract class CacheOptionIsDefault : Executing
		{
			protected void GivenCacheOptionIsDefault() => CacheOption = CacheOption.Default;
			protected void AndThenTheCacheStoreIsQueried() => The<IAsyncCacheStore<CacheEntryOptions>>().Received().GetAsync<QueryResult>(SUT.State.CacheKey, CancellationToken);

			public class CacheMiss : CacheOptionIsDefault
			{
				void AndGivenThereIsACacheMiss() { }
				void AndThenTheQueryIsExecuted() => The<IAsyncCachedQueryStub>().Received().QueryAsync(QueryContext, CancellationToken);
				void AndThenTheCacheEntryOptionsAreRequested() => The<IAsyncCachedQueryStub>().Received().GetCacheEntryOptions(QueryContext);
				void AndThenTheResultIsSetInTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().Received().SetAsync(SUT.State.CacheKey, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == QueryResult), CacheEntryOptions, CancellationToken);
				void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
			}

			public class CacheHit : CacheOptionIsDefault
			{
				readonly QueryResult _cachedResult = new QueryResult();

				void AndGivenThereIsACacheHit() => The<IAsyncCacheStore<CacheEntryOptions>>().GetAsync<QueryResult>(SUT.State.CacheKey, CancellationToken).Returns(x => _cachedResult.ToCacheEntry());
				void AndThenTheQueryIsNotExecuted() => The<IAsyncCachedQueryStub>().DidNotReceive().QueryAsync(QueryContext);
				void AndThenTheCacheEntryOptionsAreNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
				void AndThenTheResultIsNotSetInTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>(), Arg.Any<CancellationToken>());
				void AndThenTheResultIsTheCachedResult() => Result.Should().BeSameAs(_cachedResult);
			}
		}

		public class CacheOptionIsRefresh : Executing
		{
			void GivenCacheOptionIsRefresh() => CacheOption = CacheOption.Refresh;
			void AndThenTheCacheStoreIsNotQueried() => The<IAsyncCacheStore<CacheEntryOptions>>().DidNotReceive().GetAsync<QueryResult>(Arg.Any<string>(), Arg.Any<CancellationToken>());
			void AndThenTheQueryIsExecuted() => The<IAsyncCachedQueryStub>().Received().QueryAsync(QueryContext, CancellationToken);
			void AndThenTheCacheEntryOptionsAreRequested() => The<IAsyncCachedQueryStub>().Received().GetCacheEntryOptions(QueryContext);
			void AndThenTheResultIsSetInTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().Received().SetAsync(SUT.State.CacheKey, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == QueryResult), CacheEntryOptions, CancellationToken);
			void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
		}
	}

	public class EvictingCachedResult : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;

		public override void Setup() => SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());

		void WhenEvictingCachedResult() => SUT.EvictCachedResultAsync(The<IAsyncCacheStore<object>>(), _cancellationToken);
		void ThenItDelegatesToTheCacheStore() => The<IAsyncCacheStore<object>>().Received().RemoveAsync(SUT.State.CacheKey, _cancellationToken);
	}

	public abstract class UpdatingCachedResult : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		protected readonly CancellationToken CancellationToken = new CancellationTokenSource().Token;
		protected readonly QueryContext QueryContext = new QueryContext();
		protected readonly CacheEntryOptions CacheEntryOptions = new CacheEntryOptions();

		public override void Setup()
		{
			SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());
			The<IAsyncCachedQueryStub>().GetCacheEntryOptions(QueryContext).Returns(CacheEntryOptions);
		}

		public class QueryWasNotExecuted : UpdatingCachedResult
		{
			Action _invocation;

			void GivenTheQueryWasNotExecuted() { }
			void WhenUpdatingCachedResult() => _invocation = SUT.Invoking(x => x.UpdateCachedResultAsync(The<IAsyncCacheStore<CacheEntryOptions>>(), CancellationToken));
			void ThenItThrowsAnExceptionStatingThatTheCachedResultIsNotAvailable() => _invocation.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Cached result is not available");
			void AndThenTheCacheEntryOptionsAreNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().GetCacheEntryOptions(Arg.Any<QueryContext>());
		}

		public class QueryWasExecuted : UpdatingCachedResult
		{
			readonly QueryResult _queryResult = new QueryResult();

			public override void Setup()
			{
				base.Setup();
				The<IAsyncCachedQueryStub>().QueryAsync(QueryContext, CancellationToken).Returns(_queryResult);
			}

			void GivenTheQueryWasExecuted() => SUT.ExecuteAsync(QueryContext, The<IAsyncCacheStore<CacheEntryOptions>>(), cancellationToken:CancellationToken);
			void WhenUpdatingCachedResult() => SUT.UpdateCachedResultAsync(The<IAsyncCacheStore<CacheEntryOptions>>(), CancellationToken);
			void ThenTheCacheEntryOptionsAreNotRequestedAgain() => The<IAsyncCachedQueryStub>().Received(1).GetCacheEntryOptions(QueryContext);
			void AndThenItDelegatesToTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().Received(2).SetAsync(SUT.State.CacheKey, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == _queryResult), CacheEntryOptions, CancellationToken);
		}
	}

	public interface IAsyncCachedQueryStub
	{
		void ConfigureCache(ICacheConfig cacheConfig);
		CacheEntryOptions GetCacheEntryOptions(QueryContext context);
		Task<QueryResult> QueryAsync(QueryContext context, CancellationToken cancellationToken = default);
	}

	public class ConcreteAsyncCachedQuery : AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>
	{
		readonly IAsyncCachedQueryStub _asyncCachedQueryStub;

		public ConcreteAsyncCachedQuery(IAsyncCachedQueryStub asyncCachedQueryStub) => _asyncCachedQueryStub = asyncCachedQueryStub;

		protected override void ConfigureCache(ICacheConfig cacheConfig) => _asyncCachedQueryStub.ConfigureCache(cacheConfig);

		protected override CacheEntryOptions GetCacheEntryOptions(QueryContext context) => _asyncCachedQueryStub.GetCacheEntryOptions(context);

		protected override Task<QueryResult> QueryAsync(QueryContext context, CancellationToken cancellationToken = default) => _asyncCachedQueryStub.QueryAsync(context, cancellationToken);
	}
}
