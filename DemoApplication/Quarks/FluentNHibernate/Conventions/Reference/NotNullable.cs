using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Utils;

namespace Quarks.FluentNHibernate.Conventions.Reference
{
	class NotNullable : IReferenceConvention, IReferenceConventionAcceptance
	{
		public void Apply(IManyToOneInstance instance)
		{
			instance.Not.Nullable();
		}

		public void Accept(IAcceptanceCriteria<IManyToOneInspector> criteria)
		{
			criteria.Expect(x => x.Property.PropertyType.IsValueType && !x.Property.PropertyType.IsNullable() || x.Property.MemberInfo.GetMemberAttribute<RequiredAttribute>() != null);
		}
	}
}
