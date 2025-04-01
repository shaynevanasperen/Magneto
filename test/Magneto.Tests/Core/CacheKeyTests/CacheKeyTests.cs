using System;
using System.Linq;
using FluentAssertions;
using Magneto.Core;

namespace Magneto.Tests.Core.CacheKeyTests;

public abstract class SettingPrefix : ScenarioFor<CacheKey>
{
	public override void Setup()
	{
		SUT = new(TestPrefix.Value);
		SUT.Value.Should().Be(TestPrefix.Value);
	}

	public abstract class InvalidValue : SettingPrefix
	{
		protected Exception Exception = null!;

		protected void ThenAnExceptionIsThrown() => Exception.Should().NotBeNull();

		public class Null : InvalidValue
		{
			void WhenSettingPrefixToNull() => Exception = SUT.Invoking(x => x.Prefix = null).Should().Throw<Exception>().Subject.Single();
		}

		public class Empty : InvalidValue
		{
			void WhenSettingPrefixToEmpty() => Exception = SUT.Invoking(x => x.Prefix = "").Should().Throw<Exception>().Subject.Single();
		}

		public class Whitespace : InvalidValue
		{
			void WhenSettingPrefixToWhitespace() => Exception = SUT.Invoking(x => x.Prefix = " ").Should().Throw<Exception>().Subject.Single();
		}
	}
	
	public class ValidValue : SettingPrefix
	{
		void WhenSettingPrefixToValidValue() => SUT.Prefix = "NewPrefix";
		void ThenTheKeyContainsTheNewPrefix() => SUT.Value.Should().Be("NewPrefix");
	}
}

public class SettingVaryBy : ScenarioFor<CacheKey>
{
	public override void Setup()
	{
		SUT = new(TestPrefix.Value) { VaryBy = "VaryBy" };
		SUT.Value.Should().Be(TestPrefix.Value + "_VaryBy");
	}

	void WhenSettingVaryBy() => SUT.VaryBy = "NewVaryBy";
	void ThenTheKeyContainsTheNewVaryBy() => SUT.Value.Should().Be(TestPrefix.Value + "_NewVaryBy");
}

public abstract class GettingValue : ScenarioFor<CacheKey>
{
	protected object? Result;

	public override void Setup() => SUT = new(TestPrefix.Value);

	protected void WhenGettingKey() => Result = SUT.Value;

	public class NoVaryBy : GettingValue
	{
		void GivenNoVaryBy() => SUT.VaryBy = null;
		void ThenTheResultIsTheCachePrefix() => Result.Should().Be(TestPrefix.Value);
	}

	public class VaryByHavingOnlyNulls : GettingValue
	{
		void GivenAVaryByHavingOnlyNulls() => SUT.VaryBy = new { a = (object?)null, b = (object?)null };
		void ThenTheResultIsTheCachePrefixPlusUnderscoresForEachNull() => Result.Should().Be(TestPrefix.Value + "__");
	}

	public class VaryByAsAnObject : GettingValue
	{
		void GivenAVaryAsAnObject() => SUT.VaryBy = new
		{
			d = UriKind.Absolute,
			e = new[] { "one", "two", "three" },
			f = new[] { new[] { "x", "y", "z" }, ["i", "ii", "iii"], ["£", "$", "#"] },
			a = "string",
			b = 1,
			c = (object?)null,
			g = new { b = "second", a = "first" }
		};
		void ThenTheResultIsThePrefixAndTheFlattenedVaryByPropertyValuesOrderedByNameJoinedByUnderscores() => Result.Should().Be(TestPrefix.Value + "_string_1__Absolute_one_two_three_x_y_z_i_ii_iii_£_$_#_first_second");
	}

	public class VaryByAsAnArray : GettingValue
	{
		void GivenAVaryAsAnEnumerable() => SUT.VaryBy = new[]
		{
			1,
			2,
			3
		};
		void ThenTheResultIsThePrefixAndTheVaryByValuesJoinedByUnderscores() => Result.Should().Be(TestPrefix.Value + "_1_2_3");
	}

	public class VaryByAsAnEnumerable : GettingValue
	{
		void GivenAVaryAsAnEnumerable() => SUT.VaryBy = new[]
		{
			UriKind.Absolute,
			new[] { "one", "two", "three" },
			new[] { new[] { "x", "y", "z" }, ["i", "ii", "iii"], ["£", "$", "#"] },
			"string",
			1,
			(object?)null,
			new { b = "second", a = "first" }
		};
		void ThenTheResultIsThePrefixAndTheFlattenedVaryByValuesOrderedByNameJoinedByUnderscores() => Result.Should().Be(TestPrefix.Value + "_Absolute_one_two_three_x_y_z_i_ii_iii_£_$_#_string_1__first_second");
	}

	public class VaryByAsAStringOnly : GettingValue
	{
		void GivenAVaryByAsAStringOnly() => SUT.VaryBy = "varyby";
		void ThenTheResultIsTheCachePrefixPlusTheVaryByStringJoinedByAnUnderscore() => Result.Should().Be(TestPrefix.Value + "_varyby");
	}

	public class VaryByAsAPrimitive : GettingValue
	{
		public VaryByAsAPrimitive() => Examples = new("VaryBy")
		{
			true,
			byte.MinValue,
			sbyte.MinValue,
			short.MinValue,
			ushort.MinValue,
			int.MinValue,
			uint.MinValue,
			long.MinValue,
			ulong.MinValue,
			new IntPtr(int.MaxValue),
			new UIntPtr(uint.MaxValue),
			char.MinValue,
			double.MinValue,
			float.MinValue
		};

		void GivenAVaryByAsAPrimitive(object varyBy) => SUT.VaryBy = varyBy;
		void ThenTheResultIsThePrefixPlusTheVaryByJoinedByAnUnderscore(object varyBy) => Result.Should().Be(TestPrefix.Value + "_" + varyBy);
	}

	public class VaryByAsAnEnumValue : GettingValue
	{
		void GivenAVaryByAsAnEnumValue() => SUT.VaryBy = UriKind.Absolute;
		void ThenTheResultIsThePrefixPlusTheVaryByValueJoinedByAnUnderscore() => Result.Should().Be(TestPrefix.Value + "_" + UriKind.Absolute);
	}

	public class VaryByAsADateTime : GettingValue
	{
		void GivenAVaryByAsADateTime() => SUT.VaryBy = DateTime.Today;
		void ThenTheResultIsThePrefixPlusTheVaryByStringRepresentationJoinedByAnUnderscore() => Result.Should().Be(TestPrefix.Value + "_" + DateTime.Today);
	}

	public class VaryByAsADecimal : GettingValue
	{
		void GivenAVaryByAsADecimal() => SUT.VaryBy = decimal.MinValue;
		void ThenTheResultIsThePrefixPlusTheVaryByJoinedByAnUnderscore() => Result.Should().Be(TestPrefix.Value + "_" + decimal.MinValue);
	}

	public class VaryByAsAnObjectWithoutPublicProperties : GettingValue
	{
		readonly object _object = new();

		void GivenAVaryByAsAnObjectWithoutPublicProperties() => SUT.VaryBy = _object;
		void ThenTheResultIsThePrefixPlusTheVaryByStringRepresentationJoinedByAnUnderscore() => Result.Should().Be(TestPrefix.Value + "_" + _object);
	}
}

static class TestPrefix
{
	public const string Value = "Prefix";
}
