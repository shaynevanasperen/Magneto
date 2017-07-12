using System.Threading.Tasks;
using FluentAssertions;
using Magneto.Configuration;
using Magneto.Core;
using NSubstitute;

namespace Magneto.Tests.Configuration.QueryCacheTests
{
	public abstract class Getting : ScenarioFor<QueryCache<CacheEntryOptions>>
	{
		protected CacheEntryOptions CacheEntryOptions = new CacheEntryOptions();
		protected QueryResult CachedValue = new QueryResult();
		protected QueryResult QueryResult = new QueryResult();
		protected QueryResult Result;

		protected void Setup()
		{
			The<ICacheInfo>().Key.Returns("Key");
			The<IQuery>().GetCacheEntryOptions().Returns(CacheEntryOptions);
		}

		public abstract class Synchronously : Getting
		{
			protected void WhenGettingItem() => Result = SUT.Get(The<IQuery>().Execute, The<ICacheInfo>(), The<IQuery>().GetCacheEntryOptions);
			protected void ThenTheCacheStoreIsQueried() => The<ICacheStore<CacheEntryOptions>>().Received().Get<QueryResult>(The<ICacheInfo>().Key);

			public abstract class CacheEntryIsNotInCacheStore : Synchronously
			{
				protected void GivenNothingIsInTheCacheStore() { }
				protected void AndThenTheQueryIsExecuted() => The<IQuery>().Received().Execute();
				protected void AndThenNothingIsRemovedFromTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().Remove(Arg.Any<string>());

				public class QueryReturnsNonNull : CacheEntryIsNotInCacheStore
				{
					void AndGivenTheQueryReturnsNonNull() => The<IQuery>().Execute().Returns(QueryResult);
					void AndThenTheQueryResultIsCached() => The<ICacheStore<CacheEntryOptions>>().Received().Set(The<ICacheInfo>().Key, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == QueryResult), CacheEntryOptions);
					void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
				}

				public abstract class QueryReturnsNull : CacheEntryIsNotInCacheStore
				{
					protected void AndGivenTheQueryReturnsNull() => The<IQuery>().Execute().Returns((QueryResult)null);
					protected void AndThenTheResultIsNull() => Result.Should().BeNull();

					public class CacheNullsIsFalse : QueryReturnsNull
					{
						void AndGivenCacheNullsIsFalse() => The<ICacheInfo>().CacheNulls.Returns(false);
						void AndThenNothingIsCached() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().Set(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>());
					}

					public class CacheNullsIsTrue : QueryReturnsNull
					{
						void AndGivenCacheNullsIsTrue() => The<ICacheInfo>().CacheNulls.Returns(true);
						void AndThenANullIsCached() => The<ICacheStore<CacheEntryOptions>>().Received().Set(The<ICacheInfo>().Key, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == null), CacheEntryOptions);
					}
				}
			}

			public abstract class CacheEntryIsInCacheStore : Synchronously
			{
				public abstract class CacheEntryHasNullValue : CacheEntryIsInCacheStore
				{
					protected void GivenANullIsInTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().Get<QueryResult>(The<ICacheInfo>().Key).Returns(new CacheEntry<QueryResult>());

					public abstract class CacheNullsIsFalse : CacheEntryHasNullValue
					{
						protected void AndGivenCacheNullsIsFalse() => The<ICacheInfo>().CacheNulls.Returns(false);
						protected void AndThenTheNullIsRemovedFromTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().Received().Remove(The<ICacheInfo>().Key);
						protected void AndThenTheQueryIsExecuted() => The<IQuery>().Received().Execute();

						public class QueryReturnsNonNull : CacheNullsIsFalse
						{
							void AndGivenTheQueryReturnsNonNull() => The<IQuery>().Execute().Returns(QueryResult);
							void AndThenTheQueryResultIsCached() => The<ICacheStore<CacheEntryOptions>>().Received().Set(The<ICacheInfo>().Key, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == QueryResult), CacheEntryOptions);
							void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
						}

						public class QueryReturnsNull : CacheNullsIsFalse
						{
							void AndGivenTheQueryReturnsNull() => The<IQuery>().Execute().Returns((QueryResult)null);
							void AndThenNothingIsCached() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().Set(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>());
							void AndThenTheResultIsNull() => Result.Should().BeNull();
						}
					}

					public class CacheNullsIsTrue : CacheEntryHasNullValue
					{
						void AndGivenCacheNullsIsTrue() => The<ICacheInfo>().CacheNulls.Returns(true);
						void AndThenNothingIsRemovedFromTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().Remove(Arg.Any<string>());
						void AndThenNothingIsCached() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().Set(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>());
						void AndThenTheQueryIsNotExecuted() => The<IQuery>().DidNotReceive().Execute();
						void AndThenTheResultIsNull() => Result.Should().BeNull();
					}
				}

				public class CacheEntryHasNonNullValue : CacheEntryIsInCacheStore
				{
					void GivenANonNullIsInTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().Get<QueryResult>(The<ICacheInfo>().Key).Returns(new CacheEntry<QueryResult> { Value = CachedValue });
					void AndThenNothingIsRemovedFromTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().Remove(Arg.Any<string>());
					void AndThenNothingIsCached() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().Set(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>());
					void AndThenTheQueryIsNotExecuted() => The<IQuery>().DidNotReceive().Execute();
					void AndThenTheResultIsTheCachedValue() => Result.Should().BeSameAs(CachedValue);
				}
			}
		}

		public abstract class Asynchronously : Getting
		{
			protected async Task WhenGettingItem() => Result = await SUT.GetAsync(The<IQuery>().ExecuteAsync, The<ICacheInfo>(), The<IQuery>().GetCacheEntryOptions);
			protected void ThenTheCacheStoreIsQueried() => The<ICacheStore<CacheEntryOptions>>().Received().GetAsync<QueryResult>(The<ICacheInfo>().Key);

			public abstract class CacheEntryIsNotInCacheStore : Asynchronously
			{
				protected void GivenNothingIsInTheCacheStore() { }
				protected void AndThenTheQueryIsExecuted() => The<IQuery>().Received().ExecuteAsync();
				protected void AndThenNothingIsRemovedFromTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().RemoveAsync(Arg.Any<string>());

				public class QueryReturnsNonNull : CacheEntryIsNotInCacheStore
				{
					void AndGivenTheQueryReturnsNonNull() => The<IQuery>().ExecuteAsync().Returns(QueryResult);
					void AndThenTheQueryResultIsCached() => The<ICacheStore<CacheEntryOptions>>().Received().SetAsync(The<ICacheInfo>().Key, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == QueryResult), CacheEntryOptions);
					void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
				}

				public abstract class QueryReturnsNull : CacheEntryIsNotInCacheStore
				{
					protected void AndGivenTheQueryReturnsNull() => The<IQuery>().ExecuteAsync().Returns((QueryResult)null);
					protected void AndThenTheResultIsNull() => Result.Should().BeNull();

					public class CacheNullsIsFalse : QueryReturnsNull
					{
						void AndGivenCacheNullsIsFalse() => The<ICacheInfo>().CacheNulls.Returns(false);
						void AndThenNothingIsCached() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>());
					}

					public class CacheNullsIsTrue : QueryReturnsNull
					{
						void AndGivenCacheNullsIsTrue() => The<ICacheInfo>().CacheNulls.Returns(true);
						void AndThenANullIsCached() => The<ICacheStore<CacheEntryOptions>>().Received().SetAsync(The<ICacheInfo>().Key, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == null), CacheEntryOptions);
					}
				}
			}

			public abstract class CacheEntryIsInCacheStore : Asynchronously
			{
				public abstract class CacheEntryHasNullValue : CacheEntryIsInCacheStore
				{
					protected void GivenANullIsInTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().GetAsync<QueryResult>(The<ICacheInfo>().Key).Returns(new CacheEntry<QueryResult>());

					public abstract class CacheNullsIsFalse : CacheEntryHasNullValue
					{
						protected void AndGivenCacheNullsIsFalse() => The<ICacheInfo>().CacheNulls.Returns(false);
						protected void AndThenTheNullIsRemovedFromTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().Received().RemoveAsync(The<ICacheInfo>().Key);
						protected void AndThenTheQueryIsExecuted() => The<IQuery>().Received().ExecuteAsync();

						public class QueryReturnsNonNull : CacheNullsIsFalse
						{
							void AndGivenTheQueryReturnsNonNull() => The<IQuery>().ExecuteAsync().Returns(QueryResult);
							void AndThenTheQueryResultIsCached() => The<ICacheStore<CacheEntryOptions>>().Received().SetAsync(The<ICacheInfo>().Key, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == QueryResult), CacheEntryOptions);
							void AndThenTheResultIsTheQueryResult() => Result.Should().BeSameAs(QueryResult);
						}

						public class QueryReturnsNull : CacheNullsIsFalse
						{
							void AndGivenTheQueryReturnsNull() => The<IQuery>().ExecuteAsync().Returns((QueryResult)null);
							void AndThenNothingIsCached() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>());
							void AndThenTheResultIsNull() => Result.Should().BeNull();
						}
					}

					public class CacheNullsIsTrue : CacheEntryHasNullValue
					{
						void AndGivenCacheNullsIsTrue() => The<ICacheInfo>().CacheNulls.Returns(true);
						void AndThenNothingIsRemovedFromTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().RemoveAsync(Arg.Any<string>());
						void AndThenNothingIsCached() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>());
						void AndThenTheQueryIsNotExecuted() => The<IQuery>().DidNotReceive().ExecuteAsync();
						void AndThenTheResultIsNull() => Result.Should().BeNull();
					}
				}

				public class CacheEntryHasNonNullValue : CacheEntryIsInCacheStore
				{
					void GivenANonNullIsInTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().GetAsync<QueryResult>(The<ICacheInfo>().Key).Returns(new CacheEntry<QueryResult> { Value = CachedValue });
					void AndThenNothingIsRemovedFromTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().RemoveAsync(Arg.Any<string>());
					void AndThenNothingIsCached() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>());
					void AndThenTheQueryIsNotExecuted() => The<IQuery>().DidNotReceive().ExecuteAsync();
					void AndThenTheResultIsTheCachedValue() => Result.Should().BeSameAs(CachedValue);
				}
			}
		}
	}

	public abstract class Setting : ScenarioFor<QueryCache<CacheEntryOptions>>
	{
		protected CacheEntryOptions CacheEntryOptions = new CacheEntryOptions();
		protected QueryResult QueryResult;

		protected void Setup()
		{
			The<ICacheInfo>().Key.Returns("Key");
			The<IQuery>().GetCacheEntryOptions().Returns(CacheEntryOptions);
		}

		public abstract class Synchronously : Setting
		{
			protected void WhenSettingItem() => SUT.Set(QueryResult, The<ICacheInfo>(), The<IQuery>().GetCacheEntryOptions);

			public abstract class QueryResultIsNull : Synchronously
			{
				protected void GivenTheQueryResultIsNull() => QueryResult = null;

				public class CacheNullsIsFalse : QueryResultIsNull
				{
					void AndGivenCacheNullsIsFalse() => The<ICacheInfo>().CacheNulls.Returns(false);
					void ThenNothingIsCached() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().Set(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>());
				}

				public class CacheNullsIsTrue : QueryResultIsNull
				{
					void AndGivenCacheNullsIsTrue() => The<ICacheInfo>().CacheNulls.Returns(true);
					void ThenANullIsCached() => The<ICacheStore<CacheEntryOptions>>().Received().Set(The<ICacheInfo>().Key, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == null), CacheEntryOptions);
				}
			}

			public class QueryResultIsNotNull : Synchronously
			{
				void GivenTheQueryResultIsNotNull() => QueryResult = new QueryResult();
				void ThenTheQueryResultIsCached() => The<ICacheStore<CacheEntryOptions>>().Received().Set(The<ICacheInfo>().Key, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == QueryResult), CacheEntryOptions);
			}
		}

		public abstract class Asynchronously : Setting
		{
			protected async Task WhenSettingItem() => await SUT.SetAsync(QueryResult, The<ICacheInfo>(), The<IQuery>().GetCacheEntryOptions);

			public abstract class QueryResultIsNull : Asynchronously
			{
				protected void GivenTheQueryResultIsNull() => QueryResult = null;

				public class CacheNullsIsFalse : QueryResultIsNull
				{
					void AndGivenCacheNullsIsFalse() => The<ICacheInfo>().CacheNulls.Returns(false);
					void ThenNothingIsCached() => The<ICacheStore<CacheEntryOptions>>().DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<CacheEntry<QueryResult>>(), Arg.Any<CacheEntryOptions>());
				}

				public class CacheNullsIsTrue : QueryResultIsNull
				{
					void AndGivenCacheNullsIsTrue() => The<ICacheInfo>().CacheNulls.Returns(true);
					void ThenANullIsCached() => The<ICacheStore<CacheEntryOptions>>().Received().SetAsync(The<ICacheInfo>().Key, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == null), CacheEntryOptions);
				}
			}

			public class QueryResultIsNotNull : Asynchronously
			{
				void GivenTheQueryResultIsNotNull() => QueryResult = new QueryResult();
				void ThenTheQueryResultIsCached() => The<ICacheStore<CacheEntryOptions>>().Received().SetAsync(The<ICacheInfo>().Key, Arg.Is<CacheEntry<QueryResult>>(x => x.Value == QueryResult), CacheEntryOptions);
			}
		}
	}

	public abstract class Evicting : ScenarioFor<QueryCache<CacheEntryOptions>>
	{
		protected string Key = "Key";

		public class Synchronously : Evicting
		{
			void WhenEvictingItem() => SUT.Evict(Key);
			void ThenItRemovesTheEntryFromTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().Received().Remove(Key);
		}

		public class Asynchronously : Evicting
		{
			void WhenEvictingItemAsync() => SUT.EvictAsync(Key);
			void ThenItRemovesTheEntryFromTheCacheStore() => The<ICacheStore<CacheEntryOptions>>().Received().RemoveAsync(Key);
		}
	}

	public interface IQuery
	{
		CacheEntryOptions GetCacheEntryOptions();
		QueryResult Execute();
		Task<QueryResult> ExecuteAsync();
	}
}
