using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Quarks.FluentNHibernate.Conventions.UserType
{
	/// <summary>
	/// Apply convention to map enumerations without a wrapper class.
	/// See http://stackoverflow.com/questions/439003/how-do-you-map-an-enum-as-an-int-value-with-fluent-nhibernate
	/// </summary>
	class CustomTypeEnum : IUserTypeConvention
	{
		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
		{
			// Apply only to Enum properties
			criteria.Expect(x => x.Property.PropertyType.IsEnum);
		}

		public void Apply(IPropertyInstance instance)
		{
			instance.CustomType(instance.Property.PropertyType);
			// Enumerations can't be null
			instance.Not.Nullable();
		}
	}
}
