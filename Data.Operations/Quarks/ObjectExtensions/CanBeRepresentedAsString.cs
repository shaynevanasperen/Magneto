using System;
using System.Linq;

namespace Quarks.ObjectExtensions
{
	static partial class ObjectExtension
	{
		internal static bool CanBeRepresentedAsString(this object source)
		{
			return source == null ||
				source is string ||
				source is Enum ||
				source is DateTime ||
				source.GetType().IsPrimitive ||
				!source.GetType().GetProperties().Any();
		}
	}
}
