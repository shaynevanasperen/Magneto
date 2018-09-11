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

			services.AddMemoryCache();
			services.AddSingleton<ICacheStore<MemoryCacheEntryOptions>, MemoryCacheStore>();
			services.AddSingleton<IQueryCache<MemoryCacheEntryOptions>, QueryCache<MemoryCacheEntryOptions>>();

			services.AddDistributedMemoryCache();
			services.AddSingleton<ICacheStore<DistributedCacheEntryOptions>, DistributedCacheStore>();
			services.AddSingleton<IQueryCache<DistributedCacheEntryOptions>, QueryCache<DistributedCacheEntryOptions>>();

			services.AddSingleton<IDecorator, ApplicationInsightsDecorator>();
			services.AddScoped<IInvoker, Invoker>();

			services.AddSingleton(Environment.WebRootFileProvider);
			services
				.AddTransient<EnsureSuccessHandler>()
				.AddHttpClient<JsonPlaceHolderHttpClient>()
				.AddHttpMessageHandler<EnsureSuccessHandler>()
				.AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(new[]
				{
					TimeSpan.FromSeconds(1),
					TimeSpan.FromSeconds(5),
					TimeSpan.FromSeconds(10)
				}));
			services.AddScoped<IDispatcher, Dispatcher>();
			
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
