using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Utils;

namespace Quarks.FluentNHibernate.Conventions.Property
{
	class NotNullable : IPropertyConvention, IPropertyConventionAcceptance
	{
		public void Apply(IPropertyInstance instance)
		{
			instance.Not.Nullable();
		}

		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
		{
			criteria.Expect(x => x.Property.PropertyType.IsValueType && !x.Property.PropertyType.IsNullable() || x.Property.MemberInfo.GetMemberAttribute<RequiredAttribute>() != null);
		}
	}
}
