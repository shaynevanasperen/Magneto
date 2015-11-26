using System.Collections.Generic;
using System.Linq;

namespace Quarks.ObjectExtensions
{
	static partial class ObjectExtension
	{
		internal static IEnumerable<object> Flatten(this object source)
		{
			if (source.CanBeRepresentedAsString())
				yield return source;
			else
				foreach (var flattened in source.ToEnumerable().SelectMany(Flatten))
					yield return flattened;
		}
	}
}
