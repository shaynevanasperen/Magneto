using System;
using System.Linq;
using FluentAssertions;
using Magneto.Core;
using NSubstitute;
using TestStack.BDDfy;

namespace Magneto.Tests.Core.CacheInfoTests
{
	public interface IKeyCreator
	{
		string CreateKey(string keyPrefix, object varyBy);
	}

	public abstract class SettingKeyPrefix : ScenarioFor<CacheInfo>
	{
		public override void Setup()
		{
			The<IKeyCreator>().CreateKey(Arg.Any<string>(), Arg.Any<object>()).Returns(x => x.ArgAt<string>(0));
			SUT = new CacheInfo(TestKeyPrefix.Value, The<IKeyCreator>().CreateKey);
			SUT.Key.Should().Be(TestKeyPrefix.Value);
		}

		public abstract class InvalidValue : SettingKeyPrefix
		{
			protected Exception Exception;

			protected void ThenAnExceptionIsThrown() => Exception.Should().NotBeNull();

			public class Null : InvalidValue
			{
				void WhenSettingKeyPrefixToNull() => Exception = SUT.Invoking(x => x.KeyPrefix = null).Should().Throw<Exception>().Subject.Single();
			}

			public class Empty : InvalidValue
			{
				void WhenSettingKeyPrefixToEmpty() => Exception = SUT.Invoking(x => x.KeyPrefix = "").Should().Throw<Exception>().Subject.Single();
			}

			public class Whitespace : InvalidValue
			{
				void WhenSettingKeyPrefixToWhitespace() => Exception = SUT.Invoking(x => x.KeyPrefix = " ").Should().Throw<Exception>().Subject.Single();
			}
		}
		
		public class ValidValue : SettingKeyPrefix
		{
			void WhenSettingKeyPrefixToValidValue() => SUT.KeyPrefix = "NewKeyPrefix";
			void ThenTheKeyContainsTheNewKeyPrefix() => SUT.Key.Should().Be("NewKeyPrefix");
		}
	}

	public class SettingVaryBy : ScenarioFor<CacheInfo>
	{
		public override void Setup()
		{
			The<IKeyCreator>().CreateKey(Arg.Any<string>(), Arg.Any<object>()).Returns(x => x.ArgAt<object>(1));
			SUT = new CacheInfo(TestKeyPrefix.Value, The<IKeyCreator>().CreateKey) { VaryBy = "VaryBy" };
			SUT.Key.Should().Be("VaryBy");
		}

		void WhenSettingVaryBy() => SUT.VaryBy = "NewVaryBy";
		void ThenTheKeyContainsTheNewVaryBy() => SUT.Key.Should().Be("NewVaryBy");
	}

	public abstract class GettingKey : ScenarioFor<CacheInfo>
	{
		protected object Result;

		public override void Setup() => SUT = new CacheInfo(TestKeyPrefix.Value);

		protected void WhenGettingKey() => Result = SUT.Key;

		public class NoVaryBy : GettingKey
		{
			void GivenNoVaryBy() => SUT.VaryBy = null;
			void ThenTheResultIsTheCacheKeyPrefix() => Result.Should().Be(TestKeyPrefix.Value);
		}

		public class VaryByHavingOnlyNulls : GettingKey
		{
			void GivenAVaryByHavingOnlyNulls() => SUT.VaryBy = new { a = (object)null, b = (object)null };
			void ThenTheResultIsTheCacheKeyPrefixPlusUnderscoresForEachNull() => Result.Should().Be(TestKeyPrefix.Value + "__");
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
			void ThenTheResultIsTheKeyPrefixAndTheFlattenedVaryByPropertyValuesOrderedByNameJoinedByUnderscores() => Result.Should().Be(TestKeyPrefix.Value + "_string_1__Absolute_one_two_three_x_y_z_i_ii_iii_£_$_#_first_second");
		}

		public class VaryByAsAnArray : GettingKey
		{
			void GivenAVaryAsAnEnumerable() => SUT.VaryBy = new[]
			{
				1,
				2,
				3
			};
			void ThenTheResultIsTheKeyPrefixAndTheVaryByValuesJoinedByUnderscores() => Result.Should().Be(TestKeyPrefix.Value + "_1_2_3");
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
			void ThenTheResultIsTheKeyPrefixAndTheFlattenedVaryByValuesOrderedByNameJoinedByUnderscores() => Result.Should().Be(TestKeyPrefix.Value + "_Absolute_one_two_three_x_y_z_i_ii_iii_£_$_#_string_1__first_second");
		}

		public class VaryByAsAStringOnly : GettingKey
		{
			void GivenAVaryByAsAStringOnly() => SUT.VaryBy = "varyby";
			void ThenTheResultIsTheCacheKeyPrefixPlusTheVaryByStringJoinedByAnUnderscore() => Result.Should().Be(TestKeyPrefix.Value + "_varyby");
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
			void ThenTheResultIsTheKeyPrefixPlusTheVaryByJoinedByAnUnderscore(object varyBy) => Result.Should().Be(TestKeyPrefix.Value + "_" + varyBy);
		}

		public class VaryByAsAnEnumValue : GettingKey
		{
			void GivenAVaryByAsAnEnumValue() => SUT.VaryBy = UriKind.Absolute;
			void ThenTheResultIsTheKeyPrefixPlusTheVaryByValueJoinedByAnUnderscore() => Result.Should().Be(TestKeyPrefix.Value + "_" + UriKind.Absolute);
		}

		public class VaryByAsADateTime : GettingKey
		{
			void GivenAVaryByAsADateTime() => SUT.VaryBy = DateTime.Today;
			void ThenTheResultIsTheKeyPrefixPlusTheVaryByStringRepresentationJoinedByAnUnderscore() => Result.Should().Be(TestKeyPrefix.Value + "_" + DateTime.Today);
		}

		public class VaryByAsADecimal : GettingKey
		{
			void GivenAVaryByAsADecimal() => SUT.VaryBy = decimal.MinValue;
			void ThenTheResultIsTheKeyPrefixPlusTheVaryByJoinedByAnUnderscore() => Result.Should().Be(TestKeyPrefix.Value + "_" + decimal.MinValue);
		}

		public class VaryByAsAnObjectWithoutPublicProperties : GettingKey
		{
			readonly Guid _guid = Guid.NewGuid();

			void GivenAVaryByAsAnObjectWithoutPublicProperties() => SUT.VaryBy = _guid;
			void ThenTheResultIsTheKeyPrefixPlusTheVaryByStringRepresentationJoinedByAnUnderscore() => Result.Should().Be(TestKeyPrefix.Value + "_" + _guid);
		}
	}

	static class TestKeyPrefix
	{
		public const string Value = "KeyPrefix";
	}
}
