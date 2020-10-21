using System;
using System.ComponentModel;

namespace Magneto.Core
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	static class ServiceProviderExtensions
	{
		internal static T GetService<T>(this IServiceProvider serviceProvider) => (T)serviceProvider.GetService(typeof(T));

		internal static T GetRequiredService<T>(this IServiceProvider serviceProvider)
		{
			var serviceType = typeof(T);
			var service = serviceProvider.GetService(serviceType);

			if (service == null)
				throw new InvalidOperationException($"No service for type '{serviceType.FullName}' has been registered.");

			return (T)service;
		}
	}
}
