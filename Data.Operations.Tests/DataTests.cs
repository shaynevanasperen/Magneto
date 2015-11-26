using System;
using Machine.Fakes;
using Machine.Specifications;

namespace Data.Operations.Tests
{
	[Subject(typeof(Data<>))]
	class When_disposing_context_data_and_the_context_implements_idisposable : WithSubject<Data<IDisposable>>
	{
		It should_dispose_the_context_only_once = () =>
			Subject.Context.WasToldTo(x => x.Dispose()).OnlyOnce();

		Because of = () =>
		{
			Subject.Dispose();
			Subject.Dispose();
		};
	}

	[Subject(typeof(Data))]
	class When_evicting_cached_result_and_data_query_cache_is_null : WithSubject<Data>
	{
		It should_not_throw_an_exception = () =>
			exception.ShouldBeNull();

		Because of = () =>
			exception = Catch.Exception(() => Subject.EvictCachedResult(The<ICachedDataQueryBase<object>>()));

		Establish context = () =>
			Configure<IDataQueryCache>(null);

		static Exception exception;
	}

	[Subject(typeof(Data))]
	class When_evicting_cached_result_async_and_data_query_cache_is_null : WithSubject<Data>
	{
		It should_not_throw_an_exception = () =>
			exception.ShouldBeNull();

		Because of = () =>
			exception = Catch.Exception(() => Subject.EvictCachedResultAsync(The<ICachedDataQueryBase<object>>()).Wait());

		Establish context = () =>
		{
			Configure<IDataQueryCache>(null);
			Configure<IDataQueryCache>(null);
		};

		static Exception exception;
	}

	[Subject(typeof(Data))]
	class When_evicting_cached_result_and_cache_is_disabled : WithSubject<Data>
	{
		It should_not_evict_anything = () =>
			The<IDataQueryCache>().WasNotToldTo(x => x.Evict(Param.IsAny<ICacheInfo>()));

		Because of = () =>
			Subject.EvictCachedResult(The<ICachedDataQueryBase<object>>());

		Establish context = () =>
		{
			The<ICachedDataQueryBase<object>>().WhenToldTo(x => x.GetCacheInfo()).Return(The<ICacheInfo>());
			The<ICacheInfo>().WhenToldTo(x => x.Disabled).Return(true);
		};
	}

	[Subject(typeof(Data))]
	class When_evicting_cached_result_async_and_cache_is_disabled : WithSubject<Data>
	{
		It should_not_evict_anything = () =>
			The<IDataQueryCache>().WasNotToldTo(x => x.Evict(Param.IsAny<ICacheInfo>()));

		Because of = () =>
			Subject.EvictCachedResultAsync(The<ICachedDataQueryBase<object>>()).Wait();

		Establish context = () =>
		{
			The<ICachedDataQueryBase<object>>().WhenToldTo(x => x.GetCacheInfo()).Return(The<ICacheInfo>());
			The<ICacheInfo>().WhenToldTo(x => x.Disabled).Return(true);
		};
	}

	[Subject(typeof(Data))]
	class When_evicting_cached_result : WithSubject<Data>
	{
		It should_evict_the_cached_result = () =>
			The<IDataQueryCache>().WasToldTo(x => x.Evict(The<ICacheInfo>()));

		Because of = () =>
			Subject.EvictCachedResult(The<ICachedDataQueryBase<object>>());

		Establish context = () =>
			The<ICachedDataQueryBase<object>>().WhenToldTo(x => x.GetCacheInfo()).Return(The<ICacheInfo>());
	}

	[Subject(typeof(Data))]
	class When_evicting_cached_result_async : WithSubject<Data>
	{
		It should_evict_the_cached_result_using_the_async_data_query_cache = () =>
			The<IDataQueryCache>().WasToldTo(x => x.EvictAsync(The<ICacheInfo>()));

		Because of = () =>
			Subject.EvictCachedResultAsync(The<ICachedDataQueryBase<object>>()).Wait();

		Establish context = () =>
			The<ICachedDataQueryBase<object>>().WhenToldTo(x => x.GetCacheInfo()).Return(The<ICacheInfo>());
	}

	[Subject(typeof(Data))]
	class When_updating_cached_result_and_data_query_cache_is_null : WithSubject<Data>
	{
		It should_not_throw_an_exception = () =>
			exception.ShouldBeNull();

		Because of = () =>
			exception = Catch.Exception(() => Subject.UpdateCachedResult(The<ICachedDataQueryBase<object>>()));

		Establish context = () =>
			Configure<IDataQueryCache>(null);

		static Exception exception;
	}

	[Subject(typeof(Data))]
	class When_updating_cached_result_async_and_data_query_cache_is_null : WithSubject<Data>
	{
		It should_not_throw_an_exception = () =>
			exception.ShouldBeNull();

		Because of = () =>
			exception = Catch.Exception(() => Subject.UpdateCachedResultAsync(The<ICachedDataQueryBase<object>>()).Wait());

		Establish context = () =>
		{
			Configure<IDataQueryCache>(null);
			Configure<IDataQueryCache>(null);
		};

		static Exception exception;
	}

	[Subject(typeof(Data))]
	class When_updating_cached_result_and_cache_is_disabled : WithSubject<Data>
	{
		It should_not_refresh_anything = () =>
			The<IDataQueryCache>().WasNotToldTo(x => x.Evict(Param.IsAny<ICacheInfo>()));

		Because of = () =>
			Subject.UpdateCachedResult(The<ICachedDataQueryBase<object>>());

		Establish context = () =>
		{
			The<ICachedDataQueryBase<object>>().WhenToldTo(x => x.GetCacheInfo()).Return(The<ICacheInfo>());
			The<ICacheInfo>().WhenToldTo(x => x.Disabled).Return(true);
		};
	}

	[Subject(typeof(Data))]
	class When_updating_cached_result_async_and_cache_is_disabled : WithSubject<Data>
	{
		It should_not_refresh_anything = () =>
			The<IDataQueryCache>().WasNotToldTo(x => x.Evict(Param.IsAny<ICacheInfo>()));

		Because of = () =>
			Subject.UpdateCachedResultAsync(The<ICachedDataQueryBase<object>>()).Wait();

		Establish context = () =>
		{
			The<ICachedDataQueryBase<object>>().WhenToldTo(x => x.GetCacheInfo()).Return(The<ICacheInfo>());
			The<ICacheInfo>().WhenToldTo(x => x.Disabled).Return(true);
		};
	}

	[Subject(typeof(Data))]
	class When_updating_cached_result : WithSubject<Data>
	{
		It should_refresh_the_cached_result = () =>
			The<IDataQueryCache>().WasToldTo(x => x.Refresh(The<ICachedDataQueryBase<object>>().CachedResult, The<ICacheInfo>()));

		Because of = () =>
			Subject.UpdateCachedResult(The<ICachedDataQueryBase<object>>());

		Establish context = () =>
		{
			The<ICachedDataQueryBase<object>>().WhenToldTo(x => x.GetCacheInfo()).Return(The<ICacheInfo>());
			The<ICachedDataQueryBase<object>>().WhenToldTo(x => x.CachedResult).Return(new object());
		};
	}

	[Subject(typeof(Data))]
	class When_updating_cached_result_async : WithSubject<Data>
	{
		It should_refresh_the_cached_result_using_the_async_data_query_cache = () =>
			The<IDataQueryCache>().WasToldTo(x => x.RefreshAsync(The<ICachedDataQueryBase<object>>().CachedResult, The<ICacheInfo>()));

		Because of = () =>
			Subject.UpdateCachedResultAsync(The<ICachedDataQueryBase<object>>()).Wait();

		Establish context = () =>
		{
			The<ICachedDataQueryBase<object>>().WhenToldTo(x => x.GetCacheInfo()).Return(The<ICacheInfo>());
			The<ICachedDataQueryBase<object>>().WhenToldTo(x => x.CachedResult).Return(new object());
		};
	}
}
