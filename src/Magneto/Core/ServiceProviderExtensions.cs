using System;

namespace Magneto.Core
{
	static class ServiceProviderExtensions
	{
		internal static T GetService<T>(this IServiceProvider serviceProvider) => (T)serviceProvider.GetService(typeof(T));
	}
}
