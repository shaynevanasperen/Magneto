using System;
using System.Collections.Generic;
using Machine.Specifications;

namespace Data.Operations.Tests
{
	[Subject(typeof(CacheInfo))]
	class When_setting_cache_key_prefix_to_a_null_or_whitespace_value
	{
		It should_throw_an_exception = () =>
		{
			Catch.Exception(() => subject.CacheKeyPrefix = null).ShouldNotBeNull();
			Catch.Exception(() => subject.CacheKeyPrefix = "").ShouldNotBeNull();
			Catch.Exception(() => subject.CacheKeyPrefix = " ").ShouldNotBeNull();
		};

		Establish context = () =>
			subject = new CacheInfo(TestCacheKeyPrefix.Value);

		static CacheInfo subject;
	}

	[Subject(typeof(CacheInfo))]
	class When_getting_cache_key_with_no_vary_by
	{
		It should_return_the_cache_key_prefix = () =>
			subject.CacheKey.ShouldEqual(TestCacheKeyPrefix.Value);

		Establish context = () =>
			subject = new CacheInfo(TestCacheKeyPrefix.Value);

		static CacheInfo subject;
	}

	[Subject(typeof(CacheInfo))]
	class When_getting_cache_key_with_vary_by_having_only_nulls
	{
		It should_return_the_cache_key_prefix_appended_with_an_underscore_for_each_null_property = () =>
			subject.CacheKey.ShouldEqual(TestCacheKeyPrefix.Value + "__");

		Establish context = () =>
			subject = new CacheInfo(TestCacheKeyPrefix.Value) { VaryBy = new { queryParameters.Null, NullToo = queryParameters.Null } };

		static CacheInfo subject;
		static QueryParameters queryParameters = new QueryParameters { Null = null };
	}

	[Subject(typeof(CacheInfo))]
	class When_getting_cache_key_with_vary_by_as_an_object
	{
		It should_return_the_cache_key_prefix_and_the_flattened_vary_by_property_values_ordered_by_property_name_joined_by_underscores = () =>
			subject.CacheKey.ShouldEqual(TestCacheKeyPrefix.Value + "_a_1__Friday_one_two_three_x_y_z_i_ii_iii_£_$_#_first_second");

		Establish context = () =>
			subject = new CacheInfo(TestCacheKeyPrefix.Value)
			{
				VaryBy = new
				{
					d = queryParameters.Enum,
					e = queryParameters.Enumerable,
					f = queryParameters.NestedEnumerable,
					a = queryParameters.String,
					b = queryParameters.Integer,
					c = queryParameters.Null,
					g = new { b = "second", a = "first" }
				}
			};

		static CacheInfo subject;
		static QueryParameters queryParameters = new QueryParameters
		{
			String = "a",
			Integer = 1,
			Null = null,
			Enum = DayOfWeek.Friday,
			Enumerable = new[] { "one", "two", "three" },
			NestedEnumerable = new[] { new[] { "x", "y", "z" }, new[] { "i", "ii", "iii" }, new[] { "£", "$", "#" } }
		};
	}

	[Subject(typeof(CacheInfo))]
	class When_getting_cache_key_with_vary_by_as_an_enumerable
	{
		It should_return_the_cache_key_prefix_and_the_flattened_vary_by_values_joined_by_underscores = () =>
			subject.CacheKey.ShouldEqual(TestCacheKeyPrefix.Value + "_a_1__Friday_one_two_three_x_y_z_i_ii_iii_£_$_#_first_second");

		Establish context = () =>
			subject = new CacheInfo(TestCacheKeyPrefix.Value)
			{
				VaryBy = new[]
				{
					queryParameters.String,
					queryParameters.Integer,
					queryParameters.Null,
					queryParameters.Enum,
					queryParameters.Enumerable,
					queryParameters.NestedEnumerable,
					new { b = "second", a = "first" }
				}
			};

		static CacheInfo subject;
		static QueryParameters queryParameters = new QueryParameters
		{
			String = "a",
			Integer = 1,
			Null = null,
			Enum = DayOfWeek.Friday,
			Enumerable = new[] { "one", "two", "three" },
			NestedEnumerable = new[] { new[] { "x", "y", "z" }, new[] { "i", "ii", "iii" }, new[] { "£", "$", "#" } }
		};
	}

	[Subject(typeof(CacheInfo))]
	class When_getting_cache_key_with_vary_by_as_a_string_only
	{
		It should_return_the_cache_key_prefix_and_the_vary_by_string_joined_with_an_underscore = () =>
			subject.CacheKey.ShouldEqual(TestCacheKeyPrefix.Value + "_varyby");

		Establish context = () =>
			subject = new CacheInfo(TestCacheKeyPrefix.Value) { VaryBy = "varyby" };

		static CacheInfo subject;
	}

	[Subject(typeof(CacheInfo))]
	class When_getting_cache_key_when_vary_by_is_a_primitive_type
	{
		It should_return_the_cache_key_prefix_and_the_vary_by_value_joined_with_an_underscore = () =>
			subject.CacheKey.ShouldEqual(TestCacheKeyPrefix.Value + "_1");

		Establish context = () =>
			subject = new CacheInfo(TestCacheKeyPrefix.Value) { VaryBy = 1 };

		static CacheInfo subject;
	}

	[Subject(typeof(CacheInfo))]
	class When_getting_cache_key_when_vary_by_is_an_enum_value
	{
		It should_return_the_cache_key_prefix_and_the_vary_by_value_joined_with_an_underscore = () =>
			subject.CacheKey.ShouldEqual(TestCacheKeyPrefix.Value + "_" + TestEnum.EnumValue);

		Establish context = () =>
			subject = new CacheInfo(TestCacheKeyPrefix.Value) { VaryBy = TestEnum.EnumValue };

		static CacheInfo subject;

		enum TestEnum
		{
			EnumValue
		}
	}

	[Subject(typeof(CacheInfo))]
	class When_getting_cache_key_when_vary_by_is_a_datetime
	{
		It should_return_the_cache_key_prefix_and_the_vary_by_string_representation_joined_with_an_underscore = () =>
			subject.CacheKey.ShouldEqual(TestCacheKeyPrefix.Value + "_" + DateTime.Today);

		Establish context = () =>
			subject = new CacheInfo(TestCacheKeyPrefix.Value) { VaryBy = DateTime.Today };

		static CacheInfo subject;
	}

	[Subject(typeof(CacheInfo))]
	class When_getting_cache_key_when_vary_by_is_an_object_without_public_properties
	{
		It should_return_the_cache_key_prefix_and_the_vary_by_tostring_value_joined_with_an_underscore = () =>
			subject.CacheKey.ShouldEqual(TestCacheKeyPrefix.Value + "_" + guid);

		Establish context = () =>
			subject = new CacheInfo(TestCacheKeyPrefix.Value) { VaryBy = guid };

		static CacheInfo subject;
		static Guid guid = Guid.NewGuid();
	}

	class QueryParameters
	{
		public string String { get; set; }
		public int Integer { get; set; }
		public object Null { get; set; }
		public DayOfWeek Enum { get; set; }
		public IEnumerable<string> Enumerable { get; set; }
		public IEnumerable<IEnumerable<string>> NestedEnumerable { get; set; }
	}

	static class TestCacheKeyPrefix
	{
		public const string Value = "cacheKeyPrefix";
	}
}
