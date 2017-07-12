using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code.Extensions.Generic;
using Code.Extensions.Object;

namespace Magneto.Core
{
	public abstract class Operation : IEquatable<Operation>
	{
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			var other = obj as Operation;

			return other != null && Equals(other);
		}

		public virtual bool Equals(Operation other)
		{
			if (other == null)
				return false;

			if (ReferenceEquals(this, other))
				return true;

			if (other.GetType() != GetType())
				return false;

			return GetType().GetRuntimeProperties().All(x =>
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
			var segments = new List<object> { GetType().FullName };
			segments.AddRange(this.ToEnumerable());
			return string.Join("|", segments).GetHashCode();
		}
	}
}