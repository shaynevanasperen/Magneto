using System;
using System.Threading.Tasks;
using Machine.Fakes;
using Machine.Fakes.ReSharperAnnotations;
using Machine.Specifications;

namespace Data.Operations.Tests
{
	[Subject(typeof(AbstractAsyncCachedDataQuery<,>))]
	class When_getting_cached_result_async_and_data_query_cache_is_null : WithSubject<TestAbstractAsyncCachedDataQuery>
	{
		It should_not_configure_cache = () =>
			The<ITestAbstractAsyncCachedDataQuery>().WasNotToldTo(x => x.ConfigureCache(Param.IsAny<ICacheInfo>()));

		It should_execute_the_query = () =>
			result.ShouldBeTheSameAs(queryResult);

		It should_not_set_cached_result = () =>
			Subject.CachedResult.ShouldBeNull();

		Because of = () =>
			result = Subject.ExecuteAsync(queryContext).Result;

		Establish context = () =>
			The<ITestAbstractAsyncCachedDataQuery>().WhenToldTo(x => x.QueryAsync(queryContext)).Return(Task.FromResult(queryResult));
		
		static object queryContext = new object();
		static object queryResult = new object();
		static object result;
	}

	[Subject(typeof(AbstractAsyncCachedDataQuery<,>))]
	class When_getting_cached_result_async_and_cache_info_is_disabled : WithSubject<TestAbstractAsyncCachedDataQuery>
	{
		It should_configure_cache_with_the_query_class_full_name_as_a_cache_key_prefix = () =>
			The<ITestAbstractAsyncCachedDataQuery>().WasToldTo(x => x.ConfigureCache(Param<ICacheInfo>.Matches(c => c.CacheKeyPrefix == typeof(TestAbstractAsyncCachedDataQuery).FullName)));

		It should_execute_the_query = () =>
			result.ShouldBeTheSameAs(queryResult);

		It should_not_get_from_the_data_query_cache = () =>
			The<IAsyncDataQueryCache>().WasNotToldTo(x => x.GetAsync(Param.IsAny<Func<Task<object>>>(), Param.IsAny<ICacheInfo>()));

		It should_not_refresh_the_data_query_cache = () =>
			The<IAsyncDataQueryCache>().WasNotToldTo(x => x.RefreshAsync(Param.IsAny<object>(), Param.IsAny<ICacheInfo>()));

		It should_not_set_cached_result = () =>
			Subject.CachedResult.ShouldBeNull();

		Because of = () =>
			result = Subject.ExecuteAsync(queryContext, The<IAsyncDataQueryCache>()).Result;

		Establish context = () =>
			The<ITestAbstractAsyncCachedDataQuery>().WhenToldTo(x => x.QueryAsync(queryContext)).Return(Task.FromResult(queryResult));

		static object queryContext = new object();
		static object queryResult = new object();
		static object result;
	}

	[Subject(typeof(AbstractAsyncCachedDataQuery<,>))]
	class When_getting_cached_result_async_and_result_is_not_in_cache : WithSubject<TestAbstractAsyncCachedDataQuery>
	{
		It should_configure_cache_with_the_query_class_full_name_as_a_cache_key_prefix = () =>
			The<ITestAbstractAsyncCachedDataQuery>().WasToldTo(x => x.ConfigureCache(Param<ICacheInfo>.Matches(c => c.CacheKeyPrefix == typeof(TestAbstractAsyncCachedDataQuery).FullName)));

		It should_execute_the_query = () =>
			result.ShouldBeTheSameAs(queryResult);

		It should_get_from_the_data_query_cache = () =>
			The<IAsyncDataQueryCache>().WasToldTo(x => x.GetAsync(Param.IsAny<Func<Task<object>>>(), cacheInfo));

		It should_not_refresh_the_data_query_cache = () =>
			The<IAsyncDataQueryCache>().WasNotToldTo(x => x.RefreshAsync(Param.IsAny<object>(), Param.IsAny<ICacheInfo>()));

		It should_set_cached_result = () =>
			Subject.CachedResult.ShouldBeTheSameAs(result);

		Because of = () =>
			result = Subject.ExecuteAsync(queryContext, The<IAsyncDataQueryCache>()).Result;

		Establish context = () =>
		{
			The<ITestAbstractAsyncCachedDataQuery>().WhenToldTo(x => x.ConfigureCache(Param.IsAny<ICacheInfo>())).Callback((ICacheInfo c) => configureCache(c));
			The<ITestAbstractAsyncCachedDataQuery>().WhenToldTo(x => x.QueryAsync(queryContext)).Return(Task.FromResult(queryResult));
			The<IAsyncDataQueryCache>().WhenToldTo(x => x.GetAsync(Param.IsAny<Func<Task<object>>>(), Param.IsAny<ICacheInfo>())).Return((Func<Task<object>> queryAsync, ICacheInfo c) => queryAsync());
		};

		static void configureCache(ICacheInfo c)
		{
			c.CacheItemPolicy.SlidingExpiration = TimeSpan.FromMinutes(1);
			cacheInfo = c;
		}

		static ICacheInfo cacheInfo;
		static object queryContext = new object();
		static object queryResult = new object();
		static object result;
	}

	[Subject(typeof(AbstractAsyncCachedDataQuery<,>))]
	class When_getting_cached_result_async_and_result_is_in_cache : WithSubject<TestAbstractAsyncCachedDataQuery>
	{
		It should_configure_cache_with_the_query_class_full_name_as_a_cache_key_prefix = () =>
			The<ITestAbstractAsyncCachedDataQuery>().WasToldTo(x => x.ConfigureCache(Param<ICacheInfo>.Matches(c => c.CacheKeyPrefix == typeof(TestAbstractAsyncCachedDataQuery).FullName)));

		It should_not_execute_the_query = () =>
			The<ITestAbstractAsyncCachedDataQuery>().WasNotToldTo(x => x.QueryAsync(Param.IsAny<object>()));

		It should_get_from_the_data_query_cache = () =>
			The<IAsyncDataQueryCache>().WasToldTo(x => x.GetAsync(Param.IsAny<Func<Task<object>>>(), cacheInfo));

		It should_not_refresh_the_data_query_cache = () =>
			The<IAsyncDataQueryCache>().WasNotToldTo(x => x.RefreshAsync(Param.IsAny<object>(), Param.IsAny<ICacheInfo>()));

		It should_set_cached_result = () =>
			Subject.CachedResult.ShouldBeTheSameAs(result);

		Because of = () =>
			result = Subject.ExecuteAsync(queryContext, The<IAsyncDataQueryCache>()).Result;

		Establish context = () =>
		{
			The<ITestAbstractAsyncCachedDataQuery>().WhenToldTo(x => x.ConfigureCache(Param.IsAny<ICacheInfo>())).Callback((ICacheInfo c) => configureCache(c));
			The<IAsyncDataQueryCache>().WhenToldTo(x => x.GetAsync(Param.IsAny<Func<Task<object>>>(), cacheInfo)).Return(Task.FromResult(queryResult));
		};

		static void configureCache(ICacheInfo c)
		{
			c.CacheItemPolicy.SlidingExpiration = TimeSpan.FromMinutes(1);
			cacheInfo = c;
		}

		static ICacheInfo cacheInfo;
		static object queryContext = new object();
		static object queryResult = new object();
		static object result;
	}

	[Subject(typeof(AbstractAsyncCachedDataQuery<,>))]
	class When_getting_cached_result_async_and_cache_option_is_refresh : WithSubject<TestAbstractAsyncCachedDataQuery>
	{
		It should_configure_cache_with_the_query_class_full_name_as_a_cache_key_prefix = () =>
			The<ITestAbstractAsyncCachedDataQuery>().WasToldTo(x => x.ConfigureCache(Param<ICacheInfo>.Matches(c => c.CacheKeyPrefix == typeof(TestAbstractAsyncCachedDataQuery).FullName)));

		It should_execute_the_query = () =>
			result.ShouldBeTheSameAs(queryResult);

		It should_not_get_from_the_data_query_cache = () =>
			The<IAsyncDataQueryCache>().WasNotToldTo(x => x.GetAsync(Param.IsAny<Func<Task<object>>>(), Param.IsAny<ICacheInfo>()));

		It should_refresh_the_data_query_cache = () =>
			The<IAsyncDataQueryCache>().WasToldTo(x => x.RefreshAsync(result, cacheInfo));

		It should_set_cached_result = () =>
			Subject.CachedResult.ShouldBeTheSameAs(result);

		Because of = () =>
			result = Subject.ExecuteAsync(queryContext, The<IAsyncDataQueryCache>(), CacheOption.Refresh).Result;

		Establish context = () =>
		{
			The<ITestAbstractAsyncCachedDataQuery>().WhenToldTo(x => x.ConfigureCache(Param.IsAny<ICacheInfo>())).Callback((ICacheInfo c) => configureCache(c));
			The<ITestAbstractAsyncCachedDataQuery>().WhenToldTo(x => x.QueryAsync(queryContext)).Return(Task.FromResult(queryResult));
		};

		static void configureCache(ICacheInfo c)
		{
			c.CacheItemPolicy.SlidingExpiration = TimeSpan.FromMinutes(1);
			cacheInfo = c;
		}

		static ICacheInfo cacheInfo;
		static object queryContext = new object();
		static object queryResult = new object();
		static object result;
	}

	public interface ITestAbstractAsyncCachedDataQuery
	{
		void ConfigureCache(ICacheInfo cacheInfo);
		Task<object> QueryAsync(object context);
	}

	[UsedImplicitly]
	class TestAbstractAsyncCachedDataQuery : AsyncCachedDataQuery<object, object>
	{
		readonly ITestAbstractAsyncCachedDataQuery _testQuery;

		public TestAbstractAsyncCachedDataQuery(ITestAbstractAsyncCachedDataQuery testQuery)
		{
			_testQuery = testQuery;
		}

		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			_testQuery.ConfigureCache(cacheInfo);
		}

		protected override Task<object> QueryAsync(object context)
		{
			return _testQuery.QueryAsync(context);
		}
	}
}
