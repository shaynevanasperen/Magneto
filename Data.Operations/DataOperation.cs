using System;
using System.Linq;
using System.Reflection;
using Quarks.GenericExtensions;
using Quarks.ObjectExtensions;

namespace Data.Operations
{
	public abstract class DataOperation : IEquatable<DataOperation>
	{
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			var other = obj as DataOperation;

			return other != null && Equals(other);
		}

		public virtual bool Equals(DataOperation other)
		{
			if (other == null)
				return false;

			if (ReferenceEquals(this, other))
				return true;

			if (other.GetType() != GetType())
				return false;

			return GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).All(x =>
			{
				var thisValue = x.GetValue(this, null);
				var otherValue = x.GetValue(other, null);

				if (thisValue == null && otherValue == null) return true;

				if (ReferenceEquals(thisValue, otherValue)) return true;

				return thisValue != null && thisValue.QuasiEquals(otherValue);
			});
		}

		public override int GetHashCode()
		{
			return string.Join("|", this.ToEnumerable()).GetHashCode();
		}
	}
}