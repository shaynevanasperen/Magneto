using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Quarks.EntityFramework.Conventions.StoreModel
{
	class RemoveUnderscoreFromDefaultForeignKeyNames : IStoreModelConvention<AssociationType>
	{
		public void Apply(AssociationType item, DbModel model)
		{
			if (!item.IsForeignKey)
				return;

			var constraint = item.Constraint;
			var fromProperties = constraint.FromProperties;
			var toProperties = constraint.ToProperties;

			if (doPropertiesHaveDefaultNames(fromProperties, toProperties))
				normalizeForeignKeyProperties(fromProperties);

			if (doPropertiesHaveDefaultNames(toProperties, fromProperties))
				normalizeForeignKeyProperties(toProperties);
		}

		static bool doPropertiesHaveDefaultNames(ICollection<EdmProperty> properties, ICollection<EdmProperty> otherEndProperties)
		{
			if (properties.Count != otherEndProperties.Count)
				return false;

			using (var propertiesEnumerator = properties.GetEnumerator())
			using (var otherEndPropertiesEnumerator = otherEndProperties.GetEnumerator())
			{
				while (propertiesEnumerator.MoveNext() && otherEndPropertiesEnumerator.MoveNext())
					if (!propertiesEnumerator.Current.Name.EndsWith("_" + otherEndPropertiesEnumerator.Current.Name))
						return false;
			}

			return true;
		}

		static void normalizeForeignKeyProperties(IEnumerable<EdmProperty> properties)
		{
			foreach (var edmProperty in properties)
			{
				var underscoreIndex = edmProperty.Name.IndexOf('_');
				if (underscoreIndex > 0)
					edmProperty.Name = edmProperty.Name.Remove(underscoreIndex, 1);
			}
		}
	}
}
