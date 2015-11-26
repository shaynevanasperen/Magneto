using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Quarks.FluentNHibernate.Conventions.Id
{
	class GeneratedByAssigned : IIdConvention, IIdConventionAcceptance
	{
		public void Apply(IIdentityInstance instance)
		{
			instance.GeneratedBy.Assigned();
		}

		/// <summary>
		/// Apply convention only to Guid types.
		/// </summary>
		public void Accept(IAcceptanceCriteria<IIdentityInspector> criteria)
		{
			criteria.Expect(id => id.Type.GetUnderlyingSystemType() == typeof(Guid));
		}
	}
}
