using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Quarks.ObjectExtensions
{
	static partial class ObjectExtension
	{
		internal static IEnumerable<object> ToEnumerable(this object source)
		{
			var sourceEnumerable = source as IEnumerable<object>;
			return sourceEnumerable ?? source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(x => x.Name).Select(x => x.GetValue(source, null));
		}
	}
}
