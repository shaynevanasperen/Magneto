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

		protected string ExpectedCacheKey;
		protected CacheOption CacheOption;
		protected QueryResult Result;
		protected CancellationToken CancellationToken = new CancellationTokenSource().Token;

		public override void Setup()
		{
			SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());
			The<IAsyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKeyConfig>())).Do(x => x.ArgAt<IKeyConfig>(0).VaryBy = nameof(IKeyConfig.VaryBy));
			ExpectedCacheKey = $"{SUT.GetType().FullName}_{nameof(IKeyConfig.VaryBy)}";
			The<IAsyncCachedQueryStub>().CacheEntryOptions(QueryContext).Returns(CacheEntryOptions);
			The<IAsyncCachedQueryStub>().Query(QueryContext, CancellationToken).Returns(QueryResult);
		}

		protected async Task WhenExecutingTheQuery() => Result = await SUT.Execute(QueryContext, The<IAsyncCacheStore<CacheEntryOptions>>(), CacheOption, CancellationToken);
		protected void ThenTheCacheKeyIsRequestedOnlyOnce() => The<IAsyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKeyConfig>());
		protected void AndThenTheCacheKeyContainsTheFullClassName() => SUT.PeekCacheKey().Should().Contain(SUT.GetType().FullName);
		protected void AndThenTheCacheKeyContainsTheVaryByValue() => SUT.PeekCacheKey().Should().Contain(nameof(IKeyConfig.VaryBy));
		protected void AndThenNothingIsRemovedFromTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().DidNotReceive().Remove(Arg.Any<string>());
		protected void AndThenTheCachedResultIsSet() => SUT.PeekCachedResult().Should().BeSameAs(Result);

		public abstract class CacheOptionIsDefault : Executing
		{
			protected void GivenTheCacheOptionIsDefault() => CacheOption = CacheOption.Default;
			protected void AndThenTheCacheStoreIsQueried() => The<IAsyncCacheStore<CacheEntryOptions>>().Received().GetAsync<QueryResult>(ExpectedCacheKey, CancellationToken);

			public class CacheMiss : CacheOptionIsDefault
			{
				void GivenTheQueryResultIsNotCached() { }
				void AndThenTheQueryIsExecuted() => The<IAsyncCachedQueryStub>().Received(1).Query(QueryContext, CancellationToken);
				void AndThenTheCacheEntryOptionsIsRequested() => The<IAsyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
				void AndThenTheResultIsSetInTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().Received(1).SetAsync(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions, CancellationToken);
				void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
			}

			public class CacheHit : CacheOptionIsDefault
			{
				readonly QueryResult _cachedResult = new QueryResult();

				void GivenTheQueryResultIsCached() => The<IAsyncCacheStore<CacheEntryOptions>>().GetAsync<QueryResult>($"{SUT.GetType().FullName}_{nameof(IKeyConfig.VaryBy)}", CancellationToken).Returns(x => _cachedResult.ToCacheEntry());
				void AndThenTheQueryIsNotExecuted() => The<IAsyncCachedQueryStub>().DidNotReceive().Query(QueryContext);
				void AndThenTheCacheEntryOptionsIsNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().CacheEntryOptions(Arg.Any<QueryContext>());
				void AndThenTheResultIsNotSetInTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>(), Arg.Any<CancellationToken>());
				void AndThenTheResultIsTheCachedResult() => Result.Should().BeSameAs(_cachedResult);
			}
		}

		public class CacheOptionIsRefresh : Executing
		{
			void GivenTheCacheOptionIsRefresh() => CacheOption = CacheOption.Refresh;
			void AndThenTheCacheStoreIsNotQueried() => The<IAsyncCacheStore<CacheEntryOptions>>().DidNotReceive().GetAsync<QueryResult>(Arg.Any<string>(), Arg.Any<CancellationToken>());
			void AndThenTheQueryIsExecuted() => The<IAsyncCachedQueryStub>().Received(1).Query(QueryContext, CancellationToken);
			void AndThenTheCacheEntryOptionsIsRequested() => The<IAsyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
			void AndThenTheResultIsSetInTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().Received(1).SetAsync(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions, CancellationToken);
			void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
		}
	}

	public class ExecutingMultipleTimes : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
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
			SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());
			The<IAsyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKeyConfig>())).Do(x => x.ArgAt<IKeyConfig>(0).VaryBy = Guid.NewGuid().ToString());
			The<IAsyncCachedQueryStub>().CacheEntryOptions(_queryContext).Returns(x => new CacheEntryOptions());
			The<IAsyncCachedQueryStub>().Query(_queryContext).Returns(x => new QueryResult());
		}

		async Task WhenExecutingTheQuery()
		{
			_result1 = await SUT.Execute(_queryContext, The<IAsyncCacheStore<CacheEntryOptions>>(), CacheOption.Default, CancellationToken.None);
			_cacheKey1 = SUT.PeekCacheKey();
			_cacheEntryOptions1 = SUT.PeekCacheEntryOptions();
			_cachedResult1 = SUT.PeekCachedResult();
		}

		async Task AndWhenExecutingTheQueryAgain()
		{
			_result2 = await SUT.Execute(_queryContext, The<IAsyncCacheStore<CacheEntryOptions>>(), CacheOption.Default, CancellationToken.None);
			_cacheKey2 = SUT.PeekCacheKey();
			_cacheEntryOptions2 = SUT.PeekCacheEntryOptions();
			_cachedResult2 = SUT.PeekCachedResult();
		}

		void ThenTheQueryIsExecutedTwice() => The<IAsyncCachedQueryStub>().Received(2).Query(_queryContext);
		void AndThenTwoResultsAreDifferent() => _result1.Should().NotBeSameAs(_result2);
		void AndThenTwoStateCacheKeysAreDifferent() => _cacheKey1.Should().NotBeSameAs(_cacheKey2);
		void AndThenTwoStateCacheEntryOptionsAreDifferent() => _cacheEntryOptions1.Should().NotBeSameAs(_cacheEntryOptions2);
		void AndThenTwoStateCachedResultsAreDifferent() => _cachedResult1.Should().NotBeSameAs(_cachedResult2);
	}

	public class EvictingCachedResult : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;

		public override void Setup() => SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());

		void WhenEvictingCachedResult() => SUT.EvictCachedResult(The<IAsyncCacheStore<object>>(), _cancellationToken);
		void ThenItDelegatesToTheCacheStore() => The<IAsyncCacheStore<object>>().Received(1).RemoveAsync(SUT.PeekCacheKey(), _cancellationToken);
		void AndThenTheCacheKeyIsRequestedOnlyOnce() => The<IAsyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKeyConfig>());
	}

	public abstract class UpdatingCachedResult : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
	{
		protected readonly QueryContext QueryContext = new QueryContext();
		protected readonly CacheEntryOptions CacheEntryOptions = new CacheEntryOptions();
		protected readonly CancellationToken CancellationToken = new CancellationTokenSource().Token;

		protected string ExpectedCacheKey;

		public override void Setup()
		{
			SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());
			The<IAsyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKeyConfig>())).Do(x => x.ArgAt<IKeyConfig>(0).VaryBy = nameof(IKeyConfig.VaryBy));
			ExpectedCacheKey = $"{SUT.GetType().FullName}_{nameof(IKeyConfig.VaryBy)}";
			The<IAsyncCachedQueryStub>().CacheEntryOptions(QueryContext).Returns(CacheEntryOptions);
		}

		public class QueryWasNotExecuted : UpdatingCachedResult
		{
			Func<Task> _invocation;

			void GivenTheQueryWasNotExecuted() { }
			void WhenUpdatingCachedResult() => _invocation = SUT.Invoking(x => x.UpdateCachedResult(The<IAsyncCacheStore<CacheEntryOptions>>(), CancellationToken));
			void ThenTheCacheKeyIsNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().CacheKey(Arg.Any<IKeyConfig>());
			void AndThenItThrowsAnExceptionStatingThatTheCachedResultIsNotAvailable() => _invocation.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Cached result is not available");
			void AndThenTheCacheEntryOptionsIsNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().CacheEntryOptions(Arg.Any<QueryContext>());
		}

		public abstract class QueryWasExecuted : UpdatingCachedResult
		{
			protected readonly QueryResult QueryResult = new QueryResult();

			protected int ExpectedCacheStoreSetCount;

			protected void AndGivenTheQueryWasExecuted() => SUT.Execute(QueryContext, The<IAsyncCacheStore<CacheEntryOptions>>(), CacheOption.Default, CancellationToken);
			protected void WhenUpdatingCachedResult() => SUT.UpdateCachedResult(The<IAsyncCacheStore<CacheEntryOptions>>(), CancellationToken);
			protected void ThenTheCacheKeyIsRequestedOnlyOnce() => The<IAsyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKeyConfig>());
			protected void AndThenTheCacheEntryOptionsIsNotRequestedAgain() => The<IAsyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
			protected void AndThenItDelegatesToTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().Received(ExpectedCacheStoreSetCount).SetAsync(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions, CancellationToken);

			public class CacheMiss : QueryWasExecuted
			{
				void GivenTheQueryResultIsNotCached()
				{
					The<IAsyncCachedQueryStub>().Query(QueryContext, CancellationToken).Returns(QueryResult);
					ExpectedCacheStoreSetCount = 2;
				}
			}

			public class CacheHit : QueryWasExecuted
			{
				void GivenTheQueryResultIsCached()
				{
					The<IAsyncCacheStore<CacheEntryOptions>>().GetAsync<QueryResult>(ExpectedCacheKey, CancellationToken).Returns(QueryResult.ToCacheEntry());
					ExpectedCacheStoreSetCount = 1;
				}
			}
		}
	}

	public interface IAsyncCachedQueryStub
	{
		void CacheKey(IKeyConfig keyConfig);
		CacheEntryOptions CacheEntryOptions(QueryContext context);
		Task<QueryResult> Query(QueryContext context, CancellationToken cancellationToken = default);
	}

	public class ConcreteAsyncCachedQuery : AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>
	{
		readonly IAsyncCachedQueryStub _asyncCachedQueryStub;

		public ConcreteAsyncCachedQuery(IAsyncCachedQueryStub asyncCachedQueryStub) => _asyncCachedQueryStub = asyncCachedQueryStub;

		protected override void CacheKey(IKeyConfig keyConfig) => _asyncCachedQueryStub.CacheKey(keyConfig);

		protected override CacheEntryOptions CacheEntryOptions(QueryContext context) => _asyncCachedQueryStub.CacheEntryOptions(context);

		protected override Task<QueryResult> Query(QueryContext context, CancellationToken cancellationToken = default) => _asyncCachedQueryStub.Query(context, cancellationToken);
	}
}
