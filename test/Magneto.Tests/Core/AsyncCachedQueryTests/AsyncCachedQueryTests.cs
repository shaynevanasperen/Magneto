using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magneto.Configuration;
using Magneto.Core;
using NSubstitute;

namespace Magneto.Tests.Core.AsyncCachedQueryTests;

public abstract class Executing : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
{
	protected readonly QueryContext QueryContext = new();
	protected readonly CacheEntryOptions CacheEntryOptions = new();
	protected readonly QueryResult QueryResult = new();

	protected string ExpectedCacheKey = null!;
	protected CacheOption CacheOption;
	protected QueryResult Result = null!;
	protected CancellationToken CancellationToken = new CancellationTokenSource().Token;

	public override void Setup()
	{
		SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());
		The<IAsyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKey>())).Do(x => x.ArgAt<IKey>(0).VaryBy = nameof(IKey.VaryBy));
		ExpectedCacheKey = $"{SUT.GetType().FullName}_{nameof(IKey.VaryBy)}";
		The<IAsyncCachedQueryStub>().CacheEntryOptions(QueryContext).Returns(CacheEntryOptions);
		The<IAsyncCachedQueryStub>().Query(QueryContext, CancellationToken).Returns(QueryResult);
	}

	protected async Task WhenExecutingTheQuery() => Result = await SUT.Execute(QueryContext, The<IAsyncCacheStore<CacheEntryOptions>>(), CacheOption, CancellationToken);
	protected void ThenTheCacheKeyIsRequestedOnlyOnce() => The<IAsyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKey>());
	protected void AndThenTheCacheKeyContainsTheFullClassName() => SUT.PeekCacheKey().Should().Contain(SUT.GetType().FullName);
	protected void AndThenTheCacheKeyContainsTheVaryByValue() => SUT.PeekCacheKey().Should().Contain(nameof(IKey.VaryBy));
	protected void AndThenNothingIsRemovedFromTheCacheStore() => The<ISyncCacheStore<CacheEntryOptions>>().DidNotReceive().RemoveEntry(Arg.Any<string>());
	protected void AndThenTheCachedResultIsSet() => SUT.PeekCachedResult().Should().BeSameAs(Result);

	public abstract class CacheOptionIsDefault : Executing
	{
		protected void GivenTheCacheOptionIsDefault() => CacheOption = CacheOption.Default;
		protected void AndThenTheCacheStoreIsQueried() => The<IAsyncCacheStore<CacheEntryOptions>>().Received().GetEntryAsync<QueryResult>(ExpectedCacheKey, CancellationToken);

		public class CacheMiss : CacheOptionIsDefault
		{
			void GivenTheQueryResultIsNotCached() { }
			void AndThenTheQueryIsExecuted() => The<IAsyncCachedQueryStub>().Received(1).Query(QueryContext, CancellationToken);
			void AndThenTheCacheEntryOptionsIsRequested() => The<IAsyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
			void AndThenTheResultIsSetInTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().Received(1).SetEntryAsync(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions, CancellationToken);
			void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
		}

		public class CacheHit : CacheOptionIsDefault
		{
			readonly QueryResult _cachedResult = new();

			void GivenTheQueryResultIsCached() => The<IAsyncCacheStore<CacheEntryOptions>>().GetEntryAsync<QueryResult>($"{SUT.GetType().FullName}_{nameof(IKey.VaryBy)}", CancellationToken).Returns(x => _cachedResult.ToCacheEntry());
			void AndThenTheQueryIsNotExecuted() => The<IAsyncCachedQueryStub>().DidNotReceive().Query(QueryContext, Arg.Any<CancellationToken>());
			void AndThenTheCacheEntryOptionsIsNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().CacheEntryOptions(Arg.Any<QueryContext>());
			void AndThenTheResultIsNotSetInTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().DidNotReceive().SetEntryAsync(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>(), Arg.Any<CancellationToken>());
			void AndThenTheResultIsTheCachedResult() => Result.Should().BeSameAs(_cachedResult);
		}
	}

	public class CacheOptionIsRefresh : Executing
	{
		void GivenTheCacheOptionIsRefresh() => CacheOption = CacheOption.Refresh;
		void AndThenTheCacheStoreIsNotQueried() => The<IAsyncCacheStore<CacheEntryOptions>>().DidNotReceive().GetEntryAsync<QueryResult>(Arg.Any<string>(), Arg.Any<CancellationToken>());
		void AndThenTheQueryIsExecuted() => The<IAsyncCachedQueryStub>().Received(1).Query(QueryContext, CancellationToken);
		void AndThenTheCacheEntryOptionsIsRequested() => The<IAsyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
		void AndThenTheResultIsSetInTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().Received(1).SetEntryAsync(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions, CancellationToken);
		void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
	}
}

public class ExecutingMultipleTimes : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
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

	public override void Setup()
	{
		SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());
		The<IAsyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKey>())).Do(x => x.ArgAt<IKey>(0).VaryBy = Guid.NewGuid().ToString());
		The<IAsyncCachedQueryStub>().CacheEntryOptions(_queryContext).Returns(x => new());
		The<IAsyncCachedQueryStub>().Query(_queryContext, CancellationToken.None).Returns(x => new());
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

	void ThenTheQueryIsExecutedTwice() => The<IAsyncCachedQueryStub>().Received(2).Query(_queryContext, CancellationToken.None);
	void AndThenTheTwoResultsAreDifferent() => _result1.Should().NotBeSameAs(_result2);
	void AndThenTheTwoStateCacheKeysAreDifferent() => _cacheKey1.Should().NotBeSameAs(_cacheKey2);
	void AndThenTheTwoStateCacheEntryOptionsAreDifferent() => _cacheEntryOptions1.Should().NotBeSameAs(_cacheEntryOptions2);
	void AndThenTheTwoStateCachedResultsAreDifferent() => _cachedResult1.Should().NotBeSameAs(_cachedResult2);
}

public class EvictingCachedResult : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
{
	readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;

	public override void Setup()
	{
		SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());
		SUT.Execute(new(), The<IAsyncCacheStore<CacheEntryOptions>>(), CacheOption.Default, CancellationToken.None);
	}

	void WhenEvictingCachedResult() => SUT.EvictCachedResult(The<IAsyncCacheStore<CacheEntryOptions>>(), _cancellationToken);
	void ThenItDelegatesToTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().Received(1).RemoveEntryAsync(SUT.PeekCacheKey(), _cancellationToken);
	void AndThenTheCacheKeyIsRequestedOnlyOnce() => The<IAsyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKey>());
}

public abstract class UpdatingCachedResult : ScenarioFor<AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>>
{
	protected readonly QueryContext QueryContext = new();
	protected readonly CacheEntryOptions CacheEntryOptions = new();
	protected readonly CancellationToken CancellationToken = new CancellationTokenSource().Token;

	protected string ExpectedCacheKey = null!;

	public override void Setup()
	{
		SUT = new ConcreteAsyncCachedQuery(The<IAsyncCachedQueryStub>());
		The<IAsyncCachedQueryStub>().When(x => x.CacheKey(Arg.Any<IKey>())).Do(x => x.ArgAt<IKey>(0).VaryBy = nameof(IKey.VaryBy));
		ExpectedCacheKey = $"{SUT.GetType().FullName}_{nameof(IKey.VaryBy)}";
		The<IAsyncCachedQueryStub>().CacheEntryOptions(QueryContext).Returns(CacheEntryOptions);
	}

	public class QueryWasNotExecuted : UpdatingCachedResult
	{
		Func<Task> _invocation = null!;

		void GivenTheQueryWasNotExecuted() { }
		void WhenUpdatingCachedResult() => _invocation = SUT.Invoking(x => x.UpdateCachedResult(The<IAsyncCacheStore<CacheEntryOptions>>(), CancellationToken));
		void ThenTheCacheKeyIsNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().CacheKey(Arg.Any<IKey>());
		void AndThenItThrowsAnExceptionStatingThatTheCachedResultIsNotAvailable() => _invocation.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Cached result is not available");
		void AndThenTheCacheEntryOptionsIsNotRequested() => The<IAsyncCachedQueryStub>().DidNotReceive().CacheEntryOptions(Arg.Any<QueryContext>());
	}

	public abstract class QueryWasExecuted : UpdatingCachedResult
	{
		protected readonly QueryResult QueryResult = new();

		protected int ExpectedCacheStoreSetCount;

		protected void AndGivenTheQueryWasExecuted() => SUT.Execute(QueryContext, The<IAsyncCacheStore<CacheEntryOptions>>(), CacheOption.Default, CancellationToken);
		protected void WhenUpdatingCachedResult() => SUT.UpdateCachedResult(The<IAsyncCacheStore<CacheEntryOptions>>(), CancellationToken);
		protected void ThenTheCacheKeyIsRequestedOnlyOnce() => The<IAsyncCachedQueryStub>().Received(1).CacheKey(Arg.Any<IKey>());
		protected void AndThenTheCacheEntryOptionsIsNotRequestedAgain() => The<IAsyncCachedQueryStub>().Received(1).CacheEntryOptions(QueryContext);
		protected void AndThenItDelegatesToTheCacheStore() => The<IAsyncCacheStore<CacheEntryOptions>>().Received(ExpectedCacheStoreSetCount).SetEntryAsync(ExpectedCacheKey, QueryResult.ToCacheEntry(), CacheEntryOptions, CancellationToken);

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
				The<IAsyncCacheStore<CacheEntryOptions>>().GetEntryAsync<QueryResult>(ExpectedCacheKey, CancellationToken).Returns(QueryResult.ToCacheEntry());
				ExpectedCacheStoreSetCount = 1;
			}
		}
	}
}

public interface IAsyncCachedQueryStub
{
	void CacheKey(IKey key);
	CacheEntryOptions CacheEntryOptions(QueryContext context);
	Task<QueryResult> Query(QueryContext context, CancellationToken cancellationToken);
}

public class ConcreteAsyncCachedQuery(IAsyncCachedQueryStub asyncCachedQueryStub) : AsyncCachedQuery<QueryContext, CacheEntryOptions, QueryResult>
{
	protected override void CacheKey(IKey key) => asyncCachedQueryStub.CacheKey(key);

	protected override CacheEntryOptions CacheEntryOptions(QueryContext context) => asyncCachedQueryStub.CacheEntryOptions(context);

	protected override Task<QueryResult> Query(QueryContext context, CancellationToken cancellationToken) => asyncCachedQueryStub.Query(context, cancellationToken);
}
