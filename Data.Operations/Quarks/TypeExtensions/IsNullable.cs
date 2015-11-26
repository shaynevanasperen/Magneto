using System;

namespace Quarks.TypeExtensions
{
	static partial class TypeExtension
	{
		internal static bool IsNullable(this Type type)
		{
			return type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>));
		}
	}
}
