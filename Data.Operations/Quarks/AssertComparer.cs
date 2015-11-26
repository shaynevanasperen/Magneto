using System;
using System.Collections;
using System.Collections.Generic;
using Quarks.GenericExtensions;
using Quarks.TypeExtensions;

namespace Quarks
{
	class AssertComparer<T> : IComparer<T>, IEqualityComparer<T>
	{
		public int Compare(T x, T y)
		{
			foreach (var comparerStrategy in ComparerFactory.GetComparers<T>())
			{
				var comparisionResult = comparerStrategy.Compare(x, y);
				if (comparisionResult.FoundResult)
					return comparisionResult.Result;
			}
			return ComparerFactory.GetDefaultComparer<T>().Compare(x, y);
		}

		public bool Equals(T x, T y)
		{
			return Compare(x, y) == 0;
		}

		public int GetHashCode(T obj)
		{
			return obj.GetHashCode();
		}
	}

	static class ComparerFactory
	{
		internal static IEnumerable<IComparerStrategy<T>> GetComparers<T>()
		{
			return new IComparerStrategy<T>[]
			{
				new EnumerableComparer<T>(),
				new GenericTypeComparer<T>(),
				new ComparableComparer<T>(),
				new EquatableComparer<T>(),
				new TypeComparer<T>()
			};
		}

		internal static IComparer<T> GetDefaultComparer<T>()
		{
			return new DefaultComparer<T>();
		}
	}

	interface IComparerStrategy<in T>
	{
		ComparisionResult Compare(T x, T y);
	}

	class ComparisionResult
	{
		public bool FoundResult { get; protected set; }

		public int Result { get; private set; }

		public ComparisionResult(int result)
		{
			FoundResult = true;
			Result = result;
		}

		protected ComparisionResult()
		{
		}
	}

	class NoResult : ComparisionResult
	{
		public NoResult()
		{
			FoundResult = false;
		}
	}

	class DefaultComparer<T> : IComparer<T>
	{
		public int Compare(T x, T y)
		{
			return !Equals(x, y) ? -1 : 0;
		}
	}

	class EnumerableComparer<T> : IComparerStrategy<T>
	{
		public ComparisionResult Compare(T x, T y)
		{
			var enumerable1 = (object)x as IEnumerable;
			var enumerable2 = (object)y as IEnumerable;
			if (enumerable1 == null || enumerable2 == null)
				return new NoResult();
			var enumerator1 = enumerable1.GetEnumerator();
			var enumerator2 = enumerable2.GetEnumerator();
			do
			{
				var item1 = enumerator1.MoveNext();
				var item2 = enumerator2.MoveNext();
				if (!item1 || !item2)
					return new ComparisionResult(item1 == item2 ? 0 : -1);
			} while (Equals(enumerator1.Current, enumerator2.Current));
			return new ComparisionResult(-1);
		}
	}

	class GenericTypeComparer<T> : IComparerStrategy<T>
	{
		public ComparisionResult Compare(T x, T y)
		{
			var type = typeof(T);
			if (!type.IsValueType || type.IsGenericType && type.IsNullable())
			{
				if (x.IsEqualToDefault())
					return new ComparisionResult(y.IsEqualToDefault() ? 0 : -1);
				if (y.IsEqualToDefault())
					return new ComparisionResult(-1);
			}
			return new NoResult();
		}
	}

	class ComparableComparer<T> : IComparerStrategy<T>
	{
		public ComparisionResult Compare(T x, T y)
		{
			var comparable1 = (object)x as IComparable<T>;
			if (comparable1 != null)
				return new ComparisionResult(comparable1.CompareTo(y));
			var comparable2 = (object)x as IComparable;
			if (comparable2 != null)
				return new ComparisionResult(comparable2.CompareTo(y));
			return new NoResult();
		}
	}

	class EquatableComparer<T> : IComparerStrategy<T>
	{
		public ComparisionResult Compare(T x, T y)
		{
			var equatable = (object)x as IEquatable<T>;
			if (equatable == null)
				return new NoResult();
			return new ComparisionResult(equatable.Equals(y) ? 0 : -1);
		}
	}

	class TypeComparer<T> : IComparerStrategy<T>
	{
		public ComparisionResult Compare(T x, T y)
		{
			if (x.GetType() != y.GetType())
				return new ComparisionResult(-1);
			return new NoResult();
		}
	}
}