namespace Quarks.GenericExtensions
{
	static partial class GenericExtension
	{
		internal static bool QuasiEquals<T>(this T x, T y)
		{
			return new AssertComparer<T>().Equals(x, y);
		}
	}
}
