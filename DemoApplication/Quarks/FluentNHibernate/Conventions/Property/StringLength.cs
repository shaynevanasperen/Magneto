using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Quarks.FluentNHibernate.Conventions.Property
{
	class StringLength : IPropertyConvention, IPropertyConventionAcceptance
	{
		public void Apply(IPropertyInstance instance)
		{
			var attribute = instance.Property.MemberInfo.GetMemberAttribute<StringLengthAttribute>();
			var length = attribute != null ? attribute.MaximumLength : 4001; // Anything over 4000 equates to (max)
			instance.Length(length);
		}

		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
		{
			criteria.Expect(x => x.Property.PropertyType == typeof(string));
		}
	}
}
