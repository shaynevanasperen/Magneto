using System;
using System.Linq;
using FluentAssertions;
using Magneto.Core;
using TestStack.BDDfy;

namespace Magneto.Tests.Core.KeyConfigTests
{
	public abstract class SettingPrefix : ScenarioFor<KeyConfig>
	{
		public override void Setup()
		{
			SUT = new KeyConfig(TestPrefix.Value);
			SUT.Key.Should().Be(TestPrefix.Value);
		}

		public abstract class InvalidValue : SettingPrefix
		{
			protected Exception Exception;

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
			void ThenTheKeyContainsTheNewPrefix() => SUT.Key.Should().Be("NewPrefix");
		}
	}

	public class SettingVaryBy : ScenarioFor<KeyConfig>
	{
		public override void Setup()
		{
			SUT = new KeyConfig(TestPrefix.Value) { VaryBy = "VaryBy" };
			SUT.Key.Should().Be(TestPrefix.Value + "_VaryBy");
		}

		void WhenSettingVaryBy() => SUT.VaryBy = "NewVaryBy";
		void ThenTheKeyContainsTheNewVaryBy() => SUT.Key.Should().Be(TestPrefix.Value + "_NewVaryBy");
	}

	public abstract class GettingKey : ScenarioFor<KeyConfig>
	{
		protected object Result;

		public override void Setup() => SUT = new KeyConfig(TestPrefix.Value);

		protected void WhenGettingKey() => Result = SUT.Key;

		public class NoVaryBy : GettingKey
		{
			void GivenNoVaryBy() => SUT.VaryBy = null;
			void ThenTheResultIsTheCachePrefix() => Result.Should().Be(TestPrefix.Value);
		}

		public class VaryByHavingOnlyNulls : GettingKey
		{
			void GivenAVaryByHavingOnlyNulls() => SUT.VaryBy = new { a = (object)null, b = (object)null };
			void ThenTheResultIsTheCachePrefixPlusUnderscoresForEachNull() => Result.Should().Be(TestPrefix.Value + "__");
		}

		public class VaryByAsAnObject : GettingKey
		{
			void GivenAVaryAsAnObject() => SUT.VaryBy = new
			{
				d = UriKind.Absolute,
				e = new[] { "one", "two", "three" },
				f = new[] { new[] { "x", "y", "z" }, new[] { "i", "ii", "iii" }, new[] { "£", "$", "#" } },
				a = "string",
				b = 1,
				c = (object)null,
				g = new { b = "second", a = "first" }
			};
			void ThenTheResultIsThePrefixAndTheFlattenedVaryByPropertyValuesOrderedByNameJoinedByUnderscores() => Result.Should().Be(TestPrefix.Value + "_string_1__Absolute_one_two_three_x_y_z_i_ii_iii_£_$_#_first_second");
		}

		public class VaryByAsAnArray : GettingKey
		{
			void GivenAVaryAsAnEnumerable() => SUT.VaryBy = new[]
			{
				1,
				2,
				3
			};
			void ThenTheResultIsThePrefixAndTheVaryByValuesJoinedByUnderscores() => Result.Should().Be(TestPrefix.Value + "_1_2_3");
		}

		public class VaryByAsAnEnumerable : GettingKey
		{
			void GivenAVaryAsAnEnumerable() => SUT.VaryBy = new[]
			{
				UriKind.Absolute,
				new[] { "one", "two", "three" },
				new[] { new[] { "x", "y", "z" }, new[] { "i", "ii", "iii" }, new[] { "£", "$", "#" } },
				"string",
				1,
				(object)null,
				new { b = "second", a = "first" }
			};
			void ThenTheResultIsThePrefixAndTheFlattenedVaryByValuesOrderedByNameJoinedByUnderscores() => Result.Should().Be(TestPrefix.Value + "_Absolute_one_two_three_x_y_z_i_ii_iii_£_$_#_string_1__first_second");
		}

		public class VaryByAsAStringOnly : GettingKey
		{
			void GivenAVaryByAsAStringOnly() => SUT.VaryBy = "varyby";
			void ThenTheResultIsTheCachePrefixPlusTheVaryByStringJoinedByAnUnderscore() => Result.Should().Be(TestPrefix.Value + "_varyby");
		}

		public class VaryByAsAPrimitive : GettingKey
		{
			public VaryByAsAPrimitive() => Examples = new ExampleTable("VaryBy")
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

		public class VaryByAsAnEnumValue : GettingKey
		{
			void GivenAVaryByAsAnEnumValue() => SUT.VaryBy = UriKind.Absolute;
			void ThenTheResultIsThePrefixPlusTheVaryByValueJoinedByAnUnderscore() => Result.Should().Be(TestPrefix.Value + "_" + UriKind.Absolute);
		}

		public class VaryByAsADateTime : GettingKey
		{
			void GivenAVaryByAsADateTime() => SUT.VaryBy = DateTime.Today;
			void ThenTheResultIsThePrefixPlusTheVaryByStringRepresentationJoinedByAnUnderscore() => Result.Should().Be(TestPrefix.Value + "_" + DateTime.Today);
		}

		public class VaryByAsADecimal : GettingKey
		{
			void GivenAVaryByAsADecimal() => SUT.VaryBy = decimal.MinValue;
			void ThenTheResultIsThePrefixPlusTheVaryByJoinedByAnUnderscore() => Result.Should().Be(TestPrefix.Value + "_" + decimal.MinValue);
		}

		public class VaryByAsAnObjectWithoutPublicProperties : GettingKey
		{
			readonly Guid _guid = Guid.NewGuid();

			void GivenAVaryByAsAnObjectWithoutPublicProperties() => SUT.VaryBy = _guid;
			void ThenTheResultIsThePrefixPlusTheVaryByStringRepresentationJoinedByAnUnderscore() => Result.Should().Be(TestPrefix.Value + "_" + _guid);
		}
	}

	static class TestPrefix
	{
		public const string Value = "Prefix";
	}
}
