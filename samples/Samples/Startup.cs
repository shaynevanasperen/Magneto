using System;
using System.IO;
using Magneto;
using Magneto.Configuration;
using Magneto.Microsoft;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Samples.Domain;
using Samples.Infrastructure;

namespace Samples
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			Environment = env;
			InitializeAlbums();
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		void InitializeAlbums()
		{
			const string filename = "albums.json";
			using (var streamReader = new StreamReader(Environment.ContentRootFileProvider.GetFileInfo(filename).CreateReadStream()))
				File.WriteAllText(Path.Combine(Environment.WebRootPath, filename), streamReader.ReadToEnd());
		}

		public IConfigurationRoot Configuration { get; }

		protected IHostingEnvironment Environment { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddApplicationInsightsTelemetry(Configuration);

			// Here we add the Microsoft memory cache and our associated cache store.
			services.AddMemoryCache();
			services.AddSingleton<ICacheStore<MemoryCacheEntryOptions>, MemoryCacheStore>();

			// Here we add the Microsoft distributed cache and our associated cache store. Normally we'd only have one type of cache
			// in an application, but this is a sample application so we've got both here as examples.
			services.AddDistributedMemoryCache();
			services.AddSingleton<ICacheStore<DistributedCacheEntryOptions>, DistributedCacheStore>();

			// Here we add a decorator object which performs exception logging and timing telemetry for all our Magneto operations.
			services.AddSingleton<IDecorator, ApplicationInsightsDecorator>();

			// Here we add the two context objects with which our queries and commands are executed.
			services.AddSingleton(Environment.WebRootFileProvider);
			services
				.AddHttpClient<JsonPlaceHolderHttpClient>()
				.AddHttpMessageHandler(() => new EnsureSuccessHandler())
				.AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(new[]
				{
					TimeSpan.FromSeconds(1),
					TimeSpan.FromSeconds(5),
					TimeSpan.FromSeconds(10)
				}));
			
			// Here we add Magneto.IMagneto as the main entry point for consumers, because it can do everything. We could also add any of
			// the interfaces which Magneto.IMagneto is comprised of, to enable exposing limited functionality to some consumers.
			// Internally, Magneto.Magneto relies on Magneto.IMediary to do it's work, so we could also add that or any of the interfaces
			// it's comprised of in order to take control of passing the context when executing queries or commands.
			services.AddScoped<IMagneto, Magneto.Magneto>();
			
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
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

			app.UseMvc();
		}
	}
}
