namespace Quarks.FluentNHibernate.Conventions
{
	static partial class StringExtension
	{
		public const char EscapeCharacter = '`';
		/// <summary>
		/// Return the standard column name convention for the given property name.
		/// </summary>
		internal static string EscapeColumnName(this string columnName)
		{
			return string.Format("{1}{0}{1}", columnName, EscapeCharacter);
		}
	}
}
