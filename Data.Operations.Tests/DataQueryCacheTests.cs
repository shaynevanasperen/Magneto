using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Machine.Fakes;
using Machine.Specifications;

namespace Data.Operations.Tests
{
	class ConfigForACacheInfo
	{
		OnEstablish context = fakeAccessor =>
		{
			fakeAccessor.The<ICacheInfo>().WhenToldTo(x => x.CacheItemPolicy).Return(new CacheItemPolicy());
			fakeAccessor.The<ICacheInfo>().WhenToldTo(x => x.CacheKey).Return("cacheKey");
		};
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_and_it_is_not_in_the_underlying_store : WithSubject<DataQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(queryResult);

		It should_put_the_query_result_in_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.SetItem(The<ICacheInfo>().CacheKey, queryResult, The<ICacheInfo>().CacheItemPolicy));

		Because of = () =>
			result = Subject.Get(query, The<ICacheInfo>());

		Establish context = () =>
			With<ConfigForACacheInfo>();

		static int query()
		{
			queryExecuted = true;
			return queryResult;
		}

		static bool queryExecuted;
		const int queryResult = 1;
		static int result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_async_and_it_is_not_in_the_underlying_store : WithSubject<DataQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(queryResult);

		It should_put_the_query_result_in_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.SetItemAsync(The<ICacheInfo>().CacheKey, queryResult, The<ICacheInfo>().CacheItemPolicy));

		Because of = () =>
			result = Subject.GetAsync(query, The<ICacheInfo>()).Result;

		Establish context = () =>
			With<ConfigForACacheInfo>();

		static Task<int> query()
		{
			queryExecuted = true;
			return Task.FromResult(queryResult);
		}

		static bool queryExecuted;
		const int queryResult = 1;
		static int result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_and_it_is_in_the_underlying_store : WithSubject<DataQueryCache>
	{
		It should_not_execute_the_query = () =>
			queryExecuted.ShouldBeFalse();

		It should_return_the_cached_result = () =>
			result.ShouldEqual(cacheResult);

		It should_not_put_the_query_result_in_the_underlying_store = () =>
			The<ICacheStore>().WasNotToldTo(x => x.SetItem(Param.IsAny<string>(), Param.IsAny<object>(), Param.IsAny<CacheItemPolicy>()));

		Because of = () =>
			result = Subject.Get(query, The<ICacheInfo>());

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheStore>().WhenToldTo(x => x.GetItem(The<ICacheInfo>().CacheKey)).Return(cacheResult);
		};

		static int query()
		{
			queryExecuted = true;
			return 0;
		}

		static bool queryExecuted;
		const int cacheResult = 1;
		static int result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_async_and_it_is_in_the_underlying_store : WithSubject<DataQueryCache>
	{
		It should_not_execute_the_query = () =>
			queryExecuted.ShouldBeFalse();

		It should_return_the_cached_result = () =>
			result.ShouldEqual(cacheResult);

		It should_not_put_the_query_result_in_the_underlying_store = () =>
			The<ICacheStore>().WasNotToldTo(x => x.SetItemAsync(Param.IsAny<string>(), Param.IsAny<object>(), Param.IsAny<CacheItemPolicy>()));

		Because of = () =>
			result = Subject.GetAsync(query, The<ICacheInfo>()).Result;

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheStore>().WhenToldTo(x => x.GetItemAsync(The<ICacheInfo>().CacheKey)).Return(Task.FromResult((object)cacheResult));
		};

		static Task<int> query()
		{
			queryExecuted = true;
			return Task.FromResult(0);
		}

		static bool queryExecuted;
		const int cacheResult = 1;
		static int result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_and_it_is_in_the_underlying_store_but_with_wrong_type : WithSubject<DataQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(queryResult);

		It should_put_the_query_result_in_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.SetItem(The<ICacheInfo>().CacheKey, queryResult, The<ICacheInfo>().CacheItemPolicy));

		Because of = () =>
			result = Subject.Get(query, The<ICacheInfo>());

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheStore>().WhenToldTo(x => x.GetItem(The<ICacheInfo>().CacheKey)).Return("wrong type");
		};

		static int query()
		{
			queryExecuted = true;
			return queryResult;
		}

		static bool queryExecuted;
		const int queryResult = 1;
		static int result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_async_and_it_is_in_the_underlying_store_but_with_wrong_type : WithSubject<DataQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(queryResult);

		It should_put_the_query_result_in_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.SetItemAsync(The<ICacheInfo>().CacheKey, queryResult, The<ICacheInfo>().CacheItemPolicy));

		Because of = () =>
			result = Subject.GetAsync(query, The<ICacheInfo>()).Result;

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheStore>().WhenToldTo(x => x.GetItemAsync(The<ICacheInfo>().CacheKey)).Return(Task.FromResult((object)"wrong type"));
		};

		static Task<int> query()
		{
			queryExecuted = true;
			return Task.FromResult(queryResult);
		}

		static bool queryExecuted;
		const int queryResult = 1;
		static int result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_and_it_has_previously_been_cached_as_a_null_token : WithSubject<DataQueryCache>
	{
		It should_not_execute_the_query = () =>
			queryExecuted.ShouldBeFalse();

		It should_return_the_default_for_the_query_result_type = () =>
			result.ShouldEqual(0);

		It should_not_put_anything_in_the_underlying_store = () =>
			The<ICacheStore>().WasNotToldTo(x => x.SetItem(Param.IsAny<string>(), Param.IsAny<object>(), Param.IsAny<CacheItemPolicy>()));

		Because of = () =>
			result = Subject.Get(query, The<ICacheInfo>());

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheStore>().WhenToldTo(x => x.GetItem(The<ICacheInfo>().CacheKey)).Return(new DataQueryCache.NullToken());
		};

		static int query()
		{
			queryExecuted = true;
			return queryResult;
		}

		static bool queryExecuted;
		const int queryResult = 1;
		static int result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_async_and_it_has_previously_been_cached_as_a_null_token : WithSubject<DataQueryCache>
	{
		It should_not_execute_the_query = () =>
			queryExecuted.ShouldBeFalse();

		It should_return_the_default_for_the_query_result_type = () =>
			result.ShouldEqual(0);

		It should_not_put_anything_in_the_underlying_store = () =>
			The<ICacheStore>().WasNotToldTo(x => x.SetItemAsync(Param.IsAny<string>(), Param.IsAny<object>(), Param.IsAny<CacheItemPolicy>()));

		Because of = () =>
			result = Subject.GetAsync(query, The<ICacheInfo>()).Result;

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheStore>().WhenToldTo(x => x.GetItemAsync(The<ICacheInfo>().CacheKey)).Return(Task.FromResult((object)new DataQueryCache.NullToken()));
		};

		static Task<int> query()
		{
			queryExecuted = true;
			return Task.FromResult(queryResult);
		}

		static bool queryExecuted;
		const int queryResult = 1;
		static int result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_and_it_is_not_in_the_underlying_store_and_query_returns_null_and_cache_nulls_flag_is_false : WithSubject<DataQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(queryResult);

		It should_not_put_anything_in_the_underlying_store = () =>
			The<ICacheStore>().WasNotToldTo(x => x.SetItem(Param.IsAny<string>(), Param.IsAny<object>(), Param.IsAny<CacheItemPolicy>()));

		Because of = () =>
			result = Subject.Get(query, The<ICacheInfo>());

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheInfo>().WhenToldTo(x => x.CacheNulls).Return(false);
		};

		static object query()
		{
			queryExecuted = true;
			return queryResult;
		}

		static bool queryExecuted;
		const object queryResult = null;
		static object result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_async_and_it_is_not_in_the_underlying_store_and_query_returns_null_and_cache_nulls_flag_is_false : WithSubject<DataQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(queryResult);

		It should_not_put_anything_in_the_underlying_store = () =>
			The<ICacheStore>().WasNotToldTo(x => x.SetItemAsync(Param.IsAny<string>(), Param.IsAny<object>(), Param.IsAny<CacheItemPolicy>()));

		Because of = () =>
			result = Subject.GetAsync(query, The<ICacheInfo>()).Result;

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheInfo>().WhenToldTo(x => x.CacheNulls).Return(false);
		};

		static Task<object> query()
		{
			queryExecuted = true;
			return Task.FromResult(queryResult);
		}

		static bool queryExecuted;
		const object queryResult = null;
		static object result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_and_it_is_not_in_the_underlying_store_and_query_returns_null_and_cache_nulls_flag_is_true : WithSubject<DataQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(queryResult);

		It should_put_a_null_token_in_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.SetItem(The<ICacheInfo>().CacheKey, Param.IsAny<DataQueryCache.NullToken>(), The<ICacheInfo>().CacheItemPolicy));

		Because of = () =>
			result = Subject.Get(query, The<ICacheInfo>());

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheInfo>().WhenToldTo(x => x.CacheNulls).Return(true);
		};

		static object query()
		{
			queryExecuted = true;
			return queryResult;
		}

		static bool queryExecuted;
		const object queryResult = null;
		static object result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_getting_an_item_async_and_it_is_not_in_the_underlying_store_and_query_returns_null_and_cache_nulls_flag_is_true : WithSubject<DataQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(queryResult);

		It should_put_a_null_token_in_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.SetItemAsync(The<ICacheInfo>().CacheKey, Param.IsAny<DataQueryCache.NullToken>(), The<ICacheInfo>().CacheItemPolicy));

		Because of = () =>
			result = Subject.GetAsync(query, The<ICacheInfo>()).Result;

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheInfo>().WhenToldTo(x => x.CacheNulls).Return(true);
		};

		static Task<object> query()
		{
			queryExecuted = true;
			return Task.FromResult(queryResult);
		}

		static bool queryExecuted;
		const object queryResult = null;
		static object result;
	}

	[Subject(typeof(DataQueryCache))]
	class When_refreshing_a_query_result_and_it_is_null_and_cache_nulls_flag_is_false : WithSubject<DataQueryCache>
	{
		It should_remove_the_item_from_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.RemoveItem(The<ICacheInfo>().CacheKey));

		Because of = () =>
			Subject.Refresh<object>(null, The<ICacheInfo>());

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheInfo>().WhenToldTo(x => x.CacheNulls).Return(false);
		};
	}

	[Subject(typeof(DataQueryCache))]
	class When_refreshing_a_query_result_async_and_it_is_null_and_cache_nulls_flag_is_false : WithSubject<DataQueryCache>
	{
		It should_remove_the_item_from_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.RemoveItemAsync(The<ICacheInfo>().CacheKey));

		Because of = () =>
			Subject.RefreshAsync<object>(null, The<ICacheInfo>());

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheInfo>().WhenToldTo(x => x.CacheNulls).Return(false);
		};
	}

	[Subject(typeof(DataQueryCache))]
	class When_refreshing_a_query_result_and_it_is_null_and_cache_nulls_flag_is_true : WithSubject<DataQueryCache>
	{
		It should_put_a_null_token_in_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.SetItem(The<ICacheInfo>().CacheKey, Param.IsAny<DataQueryCache.NullToken>(), The<ICacheInfo>().CacheItemPolicy));

		Because of = () =>
			Subject.Refresh<object>(null, The<ICacheInfo>());

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheInfo>().WhenToldTo(x => x.CacheNulls).Return(true);
		};
	}

	[Subject(typeof(DataQueryCache))]
	class When_refreshing_a_query_result_async_and_it_is_null_and_cache_nulls_flag_is_true : WithSubject<DataQueryCache>
	{
		It should_put_a_null_token_in_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.SetItemAsync(The<ICacheInfo>().CacheKey, Param.IsAny<DataQueryCache.NullToken>(), The<ICacheInfo>().CacheItemPolicy));

		Because of = () =>
			Subject.RefreshAsync<object>(null, The<ICacheInfo>());

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheInfo>().WhenToldTo(x => x.CacheNulls).Return(true);
		};
	}

	[Subject(typeof(DataQueryCache))]
	class When_refreshing_a_query_result : WithSubject<DataQueryCache>
	{
		It should_put_the_query_result_in_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.SetItem(The<ICacheInfo>().CacheKey, queryResult, The<ICacheInfo>().CacheItemPolicy));

		Because of = () =>
			Subject.Refresh(queryResult, The<ICacheInfo>());

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheInfo>().WhenToldTo(x => x.CacheNulls).Return(true);
		};

		static object queryResult = new object();
	}

	[Subject(typeof(DataQueryCache))]
	class When_refreshing_a_query_result_async : WithSubject<DataQueryCache>
	{
		It should_put_the_query_result_in_the_underlying_store = () =>
			The<ICacheStore>().WasToldTo(x => x.SetItemAsync(The<ICacheInfo>().CacheKey, queryResult, The<ICacheInfo>().CacheItemPolicy));

		Because of = () =>
			Subject.RefreshAsync(queryResult, The<ICacheInfo>());

		Establish context = () =>
		{
			With<ConfigForACacheInfo>();
			The<ICacheInfo>().WhenToldTo(x => x.CacheNulls).Return(true);
		};

		static object queryResult = new object();
	}
}
