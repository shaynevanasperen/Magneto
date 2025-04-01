using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code.Extensions.Generic;
using Code.Extensions.Object;

namespace Magneto.Core;

/// <summary>
/// A base class for all query and command objects which makes them behave like value types,
/// by overriding the <see cref="Equals(object)"/> and <see cref="GetHashCode"/> methods. Reflection is used
/// to compare properties for determining object equality. This is purely a convenience to facilitate easy
/// mocking in unit tests.
/// </summary>
public abstract class Operation : IEquatable<Operation>
{
	/// <inheritdoc />
	public override bool Equals(object? obj)
	{
		if (obj == null)
			return false;

		if (ReferenceEquals(this, obj))
			return true;

		return obj is Operation other && Equals(other);
	}
	
	/// <inheritdoc />
	public virtual bool Equals(Operation? other)
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
	
	/// <inheritdoc />
	public override int GetHashCode()
	{
		var segments = new List<object?> { GetType().FullName };
		segments.AddRange(this.Flatten());
		return string.Join("|", segments).GetHashCode();
	}
}
