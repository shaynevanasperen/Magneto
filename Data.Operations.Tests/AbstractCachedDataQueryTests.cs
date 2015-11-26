using System;
using Machine.Fakes;
using Machine.Fakes.ReSharperAnnotations;
using Machine.Specifications;

namespace Data.Operations.Tests
{
	[Subject(typeof(AbstractCachedDataQuery<,>))]
	class When_getting_cached_result_and_data_query_cache_is_null : WithSubject<TestAbstractCachedDataQuery>
	{
		It should_not_configure_cache = () =>
			The<ITestAbstractCachedDataQuery>().WasNotToldTo(x => x.ConfigureCache(Param.IsAny<ICacheInfo>()));

		It should_execute_the_query = () =>
			result.ShouldBeTheSameAs(queryResult);

		It should_not_set_cached_result = () =>
			Subject.CachedResult.ShouldBeNull();

		Because of = () =>
			result = Subject.Execute(queryContext);

		Establish context = () =>
			The<ITestAbstractCachedDataQuery>().WhenToldTo(x => x.Query(queryContext)).Return(queryResult);

		static object queryContext = new object();
		static object queryResult = new object();
		static object result;
	}

	[Subject(typeof(AbstractCachedDataQuery<,>))]
	class When_getting_cached_result_and_cache_info_is_disabled : WithSubject<TestAbstractCachedDataQuery>
	{
		It should_configure_cache_with_the_query_class_full_name_as_a_cache_key_prefix = () =>
			The<ITestAbstractCachedDataQuery>().WasToldTo(x => x.ConfigureCache(Param<ICacheInfo>.Matches(c => c.CacheKeyPrefix == typeof(TestAbstractCachedDataQuery).FullName)));

		It should_execute_the_query = () =>
			result.ShouldBeTheSameAs(queryResult);

		It should_not_get_from_the_data_query_cache = () =>
			The<IDataQueryCache>().WasNotToldTo(x => x.Get(Param.IsAny<Func<object>>(), Param.IsAny<ICacheInfo>()));

		It should_not_refresh_the_data_query_cache = () =>
			The<IDataQueryCache>().WasNotToldTo(x => x.Refresh(Param.IsAny<object>(), Param.IsAny<ICacheInfo>()));

		It should_not_set_cached_result = () =>
			Subject.CachedResult.ShouldBeNull();

		Because of = () =>
			result = Subject.Execute(queryContext, The<IDataQueryCache>());

		Establish context = () =>
			The<ITestAbstractCachedDataQuery>().WhenToldTo(x => x.Query(queryContext)).Return(queryResult);

		static object queryContext = new object();
		static object queryResult = new object();
		static object result;
	}

	[Subject(typeof(AbstractCachedDataQuery<,>))]
	class When_getting_cached_result_and_result_is_not_in_cache : WithSubject<TestAbstractCachedDataQuery>
	{
		It should_configure_cache_with_the_query_class_full_name_as_a_cache_key_prefix = () =>
			The<ITestAbstractCachedDataQuery>().WasToldTo(x => x.ConfigureCache(Param<ICacheInfo>.Matches(c => c.CacheKeyPrefix == typeof(TestAbstractCachedDataQuery).FullName)));

		It should_execute_the_query = () =>
			result.ShouldBeTheSameAs(queryResult);

		It should_get_from_the_data_query_cache = () =>
			The<IDataQueryCache>().WasToldTo(x => x.Get(Param.IsAny<Func<object>>(), cacheInfo));

		It should_not_refresh_the_data_query_cache = () =>
			The<IDataQueryCache>().WasNotToldTo(x => x.Refresh(Param.IsAny<object>(), Param.IsAny<ICacheInfo>()));

		It should_set_cached_result = () =>
			Subject.CachedResult.ShouldBeTheSameAs(result);

		Because of = () =>
			result = Subject.Execute(queryContext, The<IDataQueryCache>());

		Establish context = () =>
		{
			The<ITestAbstractCachedDataQuery>().WhenToldTo(x => x.ConfigureCache(Param.IsAny<ICacheInfo>())).Callback((ICacheInfo c) => configureCache(c));
			The<ITestAbstractCachedDataQuery>().WhenToldTo(x => x.Query(queryContext)).Return(queryResult);
			The<IDataQueryCache>().WhenToldTo(x => x.Get(Param.IsAny<Func<object>>(), Param.IsAny<ICacheInfo>())).Return((Func<object> query, ICacheInfo cacheInfo) => query());
		};

		static void configureCache(ICacheInfo c)
		{
			c.AbsoluteDuration = TimeSpan.FromMinutes(1);
			cacheInfo = c;
		}

		static ICacheInfo cacheInfo;
		static object queryContext = new object();
		static object queryResult = new object();
		static object result;
	}

	[Subject(typeof(AbstractCachedDataQuery<,>))]
	class When_getting_cached_result_and_result_is_in_cache : WithSubject<TestAbstractCachedDataQuery>
	{
		It should_configure_cache_with_the_query_class_full_name_as_a_cache_key_prefix = () =>
			The<ITestAbstractCachedDataQuery>().WasToldTo(x => x.ConfigureCache(Param<ICacheInfo>.Matches(c => c.CacheKeyPrefix == typeof(TestAbstractCachedDataQuery).FullName)));

		It should_not_execute_the_query = () =>
			The<ITestAbstractCachedDataQuery>().WasNotToldTo(x => x.Query(Param.IsAny<object>()));

		It should_get_from_the_data_query_cache = () =>
			The<IDataQueryCache>().WasToldTo(x => x.Get(Param.IsAny<Func<object>>(), cacheInfo));

		It should_not_refresh_the_data_query_cache = () =>
			The<IDataQueryCache>().WasNotToldTo(x => x.Refresh(Param.IsAny<object>(), Param.IsAny<ICacheInfo>()));

		It should_set_cached_result = () =>
			Subject.CachedResult.ShouldBeTheSameAs(result);

		Because of = () =>
			result = Subject.Execute(queryContext, The<IDataQueryCache>());

		Establish context = () =>
		{
			The<ITestAbstractCachedDataQuery>().WhenToldTo(x => x.ConfigureCache(Param.IsAny<ICacheInfo>())).Callback((ICacheInfo c) => configureCache(c));
			The<IDataQueryCache>().WhenToldTo(x => x.Get(Param.IsAny<Func<object>>(), Param.IsAny<ICacheInfo>())).Return(queryResult);
		};

		static void configureCache(ICacheInfo c)
		{
			c.AbsoluteDuration = TimeSpan.FromMinutes(1);
			cacheInfo = c;
		}

		static ICacheInfo cacheInfo;

		static object queryContext = new object();
		static object queryResult = new object();
		static object result;
	}

	[Subject(typeof(AbstractCachedDataQuery<,>))]
	class When_getting_cached_result_and_cache_option_is_refresh : WithSubject<TestAbstractCachedDataQuery>
	{
		It should_configure_cache_with_the_query_class_full_name_as_a_cache_key_prefix = () =>
			The<ITestAbstractCachedDataQuery>().WasToldTo(x => x.ConfigureCache(Param<ICacheInfo>.Matches(c => c.CacheKeyPrefix == typeof(TestAbstractCachedDataQuery).FullName)));

		It should_execute_the_query = () =>
			result.ShouldBeTheSameAs(queryResult);

		It should_not_get_from_the_data_query_cache = () =>
			The<IDataQueryCache>().WasNotToldTo(x => x.Get(Param.IsAny<Func<object>>(), Param.IsAny<ICacheInfo>()));

		It should_refresh_the_data_query_cache = () =>
			The<IDataQueryCache>().WasToldTo(x => x.Refresh(result, cacheInfo));

		It should_set_cached_result = () =>
			Subject.CachedResult.ShouldBeTheSameAs(result);

		Because of = () =>
			result = Subject.Execute(queryContext, The<IDataQueryCache>(), CacheOption.Refresh);

		Establish context = () =>
		{
			The<ITestAbstractCachedDataQuery>().WhenToldTo(x => x.ConfigureCache(Param.IsAny<ICacheInfo>())).Callback((ICacheInfo c) => configureCache(c));
			The<ITestAbstractCachedDataQuery>().WhenToldTo(x => x.Query(queryContext)).Return(queryResult);
		};

		static void configureCache(ICacheInfo c)
		{
			c.AbsoluteDuration = TimeSpan.FromMinutes(1);
			cacheInfo = c;
		}

		static ICacheInfo cacheInfo;
		static object queryContext = new object();
		static object queryResult = new object();
		static object result;
	}

	public interface ITestAbstractCachedDataQuery
	{
		void ConfigureCache(ICacheInfo cacheInfo);
		object Query(object context);
	}

	[UsedImplicitly]
	class TestAbstractCachedDataQuery : CachedDataQuery<object, object>
	{
		readonly ITestAbstractCachedDataQuery _testQuery;

		public TestAbstractCachedDataQuery(ITestAbstractCachedDataQuery testQuery)
		{
			_testQuery = testQuery;
		}

		protected override void ConfigureCache(ICacheInfo cacheInfo)
		{
			_testQuery.ConfigureCache(cacheInfo);
		}

		protected override object Query(object context)
		{
			return _testQuery.Query(context);
		}
	}
}
