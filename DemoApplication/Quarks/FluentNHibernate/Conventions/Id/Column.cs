using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Quarks.FluentNHibernate.Conventions.Id
{
	/// <summary>
	/// Applies standard column naming convention for Id fields.
	/// </summary>
	class Column : IIdConvention, IIdConventionAcceptance
	{
		public void Apply(IIdentityInstance instance)
		{
			// Standard identity column name is "Id"
			var abbreviation = AttributeHelper.GetTypeAttribute<AbbreviationAttribute>(instance.EntityType);
			var prefix = abbreviation == null
				? ""
				: abbreviation.Abbreviation;
			instance.Column((prefix + instance.Name).EscapeColumnName());
		}

		public void Accept(IAcceptanceCriteria<IIdentityInspector> criteria)
		{
			// Ignore id properties that already have column name set
			criteria.Expect(id => id.Columns, Is.Not.Set);
			criteria.Expect(id => id.Name == "Id");
		}
	}
}
