using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Quarks.FluentNHibernate.Conventions.Reference
{
	/// <summary>
	/// Applies PropertyRef to ManyToOne references to determine the referenced type's Id property
	/// using the property name convention of "PropertyEntityTypeId".
	/// Override convention by using Column() in your AutoMappingOverride class.
	/// </summary>
	class Column : IReferenceConvention, IReferenceConventionAcceptance
	{
		public void Apply(IManyToOneInstance instance)
		{
			var abbreviation = AttributeHelper.GetTypeAttribute<AbbreviationAttribute>(instance.Property.PropertyType);
			var prefix = abbreviation == null
				? instance.Property.PropertyType.Name
				: abbreviation.Abbreviation;
			instance.Column((prefix + "Id").EscapeColumnName());
		}

		public void Accept(IAcceptanceCriteria<IManyToOneInspector> criteria)
		{
			// Ignore properties that have already been set
			criteria.Expect(c => string.IsNullOrEmpty(c.PropertyRef));
		}
	}
}