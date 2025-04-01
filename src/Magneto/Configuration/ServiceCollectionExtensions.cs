using System;
using System.ComponentModel;
using Magneto;
using Magneto.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A <see cref="IServiceCollection"/> extension class for configuring Magneto.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ServiceCollectionExtensions
{

	/// <summary>
	/// Adds Magneto. Returns a <see cref="MagnetoBuilder"/> for use in
	/// specifying a decorator, cache key creator, and cache stores.
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
	/// <returns>An instance of <see cref="MagnetoBuilder"/> for use in
	/// specifying a decorator, cache key creator, and cache stores.</returns>
	public static MagnetoBuilder AddMagneto(this IServiceCollection services)
	{
		ArgumentNullException.ThrowIfNull(services);

		services.AddTransient<IMagneto, Conductor>();

		return new(services);
	}
}
