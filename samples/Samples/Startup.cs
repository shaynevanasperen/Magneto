using System;
using System.IO;
using System.Text.Json;
using Magneto;
using Magneto.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Samples.Domain;
using Samples.Infrastructure;

namespace Samples;

public class Startup
{
	public Startup(IWebHostEnvironment environment, IConfiguration configuration)
	{
		Environment = environment;
		Configuration = configuration;
		InitializeAlbums();
	}

	void InitializeAlbums()
	{
		using var streamReader = new StreamReader(Environment.ContentRootFileProvider.GetFileInfo(Album.AllAlbumsFilename).CreateReadStream());
		File.WriteAllText(Path.Combine(Environment.WebRootPath, Album.AllAlbumsFilename), streamReader.ReadToEnd());
	}

	public IConfiguration Configuration { get; }

	protected IWebHostEnvironment Environment { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		// Here we add ApplicationInsights, which is a dependency of our Magneto decorator.
		services.AddApplicationInsightsTelemetry(Configuration);

		// Here we add the Microsoft memory cache, used by some of our Magneto cached queries.
		services.AddMemoryCache();

		// Here we add the Microsoft distributed cache, used by some of our Magneto cached queries.
		// Normally we'd only have one type of cache in an application, but this is a
		// sample application so we've got both here as examples.
		services.AddDistributedMemoryCache();

		// Here we add the three context objects with which our queries and commands are executed. The first two are actually
		// used together in a ValueTuple and are resolved as such by a special wrapper around IServiceProvider.
		services.AddSingleton(Environment.WebRootFileProvider);
		services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true });
		services.AddHttpClient<JsonPlaceHolderHttpClient>()
			.AddHttpMessageHandler(() => new EnsureSuccessHandler())
			.AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(
			[
				TimeSpan.FromSeconds(1),
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(10)
			]));

		// Here we configure Magneto fluently.
		services.AddMagneto()
			.WithDecorator<ApplicationInsightsDecorator>()
			.WithCacheKeyCreator((prefix, varyBy) => $"{prefix}.{JsonSerializer.Serialize(varyBy)}")
			.WithMemoryCacheStore()
			.WithDistributedCacheStore();

		// Or we could configure it manually.
		//ConfigureMagnetoManually(services);

		services.AddControllersWithViews();
	}

	static void ConfigureMagnetoManually(IServiceCollection services)
	{
		// Here we add IMagneto as the main entry point for consumers, because it can do everything. We could also add any of
		// the interfaces which IMagneto comprises, to enable exposing limited functionality to some consumers.
		// Internally, Conductor relies on IMediary to do its work, so we could also add that or any of the interfaces
		// it's comprised of in order to take control of passing the context when executing queries or commands.
		services.AddTransient<IMagneto, Conductor>();

		// Here we add a decorator object which performs exception logging and timing telemetry for all our Magneto operations.
		services.AddSingleton<IDecorator, ApplicationInsightsDecorator>();

		// Here we add our cache store associated with the Microsoft memory cache.
		services.AddSingleton<ICacheStore<MemoryCacheEntryOptions>, MemoryCacheStore>();

		// Here we add our cache store and serializer associated with the Microsoft distributed cache. Normally we'd only
		// have one type of cache in an application, but this is a sample application so we've got both here as examples.
		services.AddSingleton<IStringSerializer, SystemTextStringSerializer>();
		services.AddSingleton<ICacheStore<DistributedCacheEntryOptions>, DistributedCacheStore>();

		// Here we specify how cache keys are created. This is optional as there is already a default built-in method,
		// but consumers may want to use their own method instead.
		CachedQuery.UseKeyCreator((prefix, varyBy) => $"{prefix}.{JsonSerializer.Serialize(varyBy)}");
	}

	public void Configure(IApplicationBuilder app)
	{
		if (Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}
		else
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseCookiePolicy();

		app.UseRouting();
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
}
