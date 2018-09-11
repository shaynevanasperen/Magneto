using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Magneto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Samples.Domain;
using Samples.Models;

namespace Samples.Controllers
{
	[Route("[controller]")]
	public class UsersController : Controller
	{
		readonly IDispatcher _dispatcher;

		public UsersController(IDispatcher dispatcher) => _dispatcher = dispatcher;

		[HttpGet("")]
		public async Task<IActionResult> Index()
		{
			var users = await _dispatcher.QueryAsync(new AllUsers());
			return View(users);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> Index(int id)
		{
			var user = await _dispatcher.QueryAsync(new UserById { Id = id });
			var userAlbums = _dispatcher.Query(new AlbumsByUserId { UserId = id });
			return View("User", new UserViewModel { User = user, Albums = userAlbums });
		}

		[HttpPost("{id:int}")]
		public IActionResult Album(int id, string title)
		{
			_dispatcher.Command(new SaveAlbum { Album = new Album { Title = title, UserId = id } });
			return RedirectToAction(nameof(Index), new { id });
		}
	}

	public class AllUsers : AsyncCachedQuery<JsonPlaceHolderHttpClient, DistributedCacheEntryOptions, User[]>
	{
		protected override void ConfigureCache(ICacheConfig cacheConfig) => cacheConfig.KeyPrefix = User.AllUsersCacheKeyPrefix;

		protected override DistributedCacheEntryOptions GetCacheEntryOptions(JsonPlaceHolderHttpClient context) => User.AllUsersCacheEntryOptions(context);

		protected override async Task<User[]> QueryAsync(JsonPlaceHolderHttpClient context)
		{
			var response = await context.GetAsync("/users");
			return await response.Content.ReadAsAsync<User[]>();
		}

		public int Id { get; set; }
	}

	public class UserById : AsyncTransformedCachedQuery<JsonPlaceHolderHttpClient, DistributedCacheEntryOptions, User[], User>
	{
		protected override void ConfigureCache(ICacheConfig cacheConfig) => cacheConfig.KeyPrefix = User.AllUsersCacheKeyPrefix;

		protected override DistributedCacheEntryOptions GetCacheEntryOptions(JsonPlaceHolderHttpClient context) => User.AllUsersCacheEntryOptions(context);

		protected override async Task<User[]> QueryAsync(JsonPlaceHolderHttpClient context)
		{
			var response = await context.GetAsync("/users");
			return await response.Content.ReadAsAsync<User[]>();
		}

		protected override Task<User> TransformCachedResultAsync(User[] cachedResult) => Task.FromResult(cachedResult.Single(x => x.Id == Id));

		public int Id { get; set; }
	}

	public class AlbumsByUserId : SyncTransformedCachedQuery<IFileProvider, MemoryCacheEntryOptions, Album[], Album[]>
	{
		const string Filename = "albums.json";

		protected override MemoryCacheEntryOptions GetCacheEntryOptions(IFileProvider context) =>
			new MemoryCacheEntryOptions()
				.AddExpirationToken(context.Watch(Filename))
				.RegisterPostEvictionCallback((echoKey, value, reason, state) =>
				{
					Console.WriteLine($"{echoKey} : {value} was evicted due to {reason}");
				});

		protected override Album[] Query(IFileProvider context)
		{
			using (var streamReader = new StreamReader(context.GetFileInfo(Filename).CreateReadStream()))
			{
				var json = streamReader.ReadToEnd();
				return JsonConvert.DeserializeObject<Album[]>(json);
			}
		}

		protected override Album[] TransformCachedResult(Album[] cachedResult) => cachedResult.Where(x => x.UserId == UserId).ToArray();

		public int UserId { get; set; }
	}

	public class SaveAlbum : SyncCommand<IFileProvider>
	{
		const string Filename = "albums.json";

		public override void Execute(IFileProvider context)
		{
			lock (context)
			{
				var fileInfo = context.GetFileInfo(Filename);
				Album[] albums;
				using (var streamReader = new StreamReader(fileInfo.CreateReadStream()))
				{
					var json = streamReader.ReadToEnd();
					var existingAlbums = JsonConvert.DeserializeObject<Album[]>(json);
					Album.Id = existingAlbums.Max(x => x.Id) + 1;
					albums = existingAlbums.Concat(new[] { Album }).ToArray();
				}
				File.WriteAllText(fileInfo.PhysicalPath, JsonConvert.SerializeObject(albums, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
			}
		}

		public Album Album { get; set; }
	}
}
