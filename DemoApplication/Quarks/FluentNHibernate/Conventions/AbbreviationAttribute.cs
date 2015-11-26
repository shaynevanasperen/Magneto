using System;
using Remotion.Linq.Utilities;

namespace Quarks.FluentNHibernate.Conventions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class AbbreviationAttribute : Attribute
	{
		public AbbreviationAttribute(string abbreviation)
		{
			if (string.IsNullOrWhiteSpace(abbreviation)) throw new ArgumentEmptyException("abbreviation");
			Abbreviation = abbreviation;
		}

		public string Abbreviation { get; private set; }
	}
}
