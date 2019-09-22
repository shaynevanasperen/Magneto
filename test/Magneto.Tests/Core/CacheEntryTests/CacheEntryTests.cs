using FluentAssertions;
using Magneto.Core;

namespace Magneto.Tests.Core.CacheEntryTests
{
	public abstract class CheckingEquality : ScenarioFor<CacheEntry<object>>
	{
		protected object Object;

		bool _objectsAreEqual;
		bool _cacheEntriesAreEqual;
		bool _hashCodesAreEqual;

		protected void WhenCheckingEquality()
		{
			_objectsAreEqual = SUT.Equals(Object);
			_cacheEntriesAreEqual = SUT.Equals((CacheEntry<object>)Object);
			_hashCodesAreEqual = SUT.GetHashCode() == Object?.GetHashCode();
		}

		protected void AssertEqual()
		{
			_objectsAreEqual.Should().BeTrue();
			_cacheEntriesAreEqual.Should().BeTrue();
			_hashCodesAreEqual.Should().BeTrue();
		}

		protected void AssertNotEqual()
		{
			_objectsAreEqual.Should().BeFalse();
			_cacheEntriesAreEqual.Should().BeFalse();
			_hashCodesAreEqual.Should().BeFalse();
		}

		public class WithNull : CheckingEquality
		{
			void GivenACacheEntryAndANullReference()
			{
				SUT = new CacheEntry<object>(new object());
				Object = null;
			}
			void ThenTheyAreNotEqual() => AssertNotEqual();
		}

		public class WithItself : CheckingEquality
		{
			void GivenTwoReferencesToTheSameCacheEntry()
			{
				SUT = new CacheEntry<object>(new object());
				Object = SUT;
			}
			void ThenTheyAreEqual() => AssertEqual();
		}

		public class WithAnotherCacheEntryHavingTheSameValue : CheckingEquality
		{
			void GivenTwoReferencesToTheSameCacheEntry()
			{
				var value = new object();
				SUT = new CacheEntry<object>(value);
				Object = new CacheEntry<object>(value);
			}
			void ThenTheyAreEqual() => AssertEqual();
		}

		public class WithAnotherCacheEntryHavingADifferentValue : CheckingEquality
		{
			void GivenTwoReferencesToTheSameCacheEntry()
			{
				SUT = new CacheEntry<object>(new object());
				Object = new CacheEntry<object>(new object());
			}
			void ThenTheyAreNotEqual() => AssertNotEqual();
		}
	}
}
