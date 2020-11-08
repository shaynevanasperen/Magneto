using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Magneto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Samples.Domain;
using Samples.Models;

namespace Samples.Controllers
{
	[Route("[controller]")]
	public class UsersController : Controller
	{
		readonly IMagneto _magneto;

		public UsersController(IMagneto magneto) => _magneto = magneto;

		[HttpGet("")]
		public async Task<IActionResult> Index()
		{
			var users = await _magneto.QueryAsync(new AllUsers(), CacheOption.Default);
			return View(users);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> Index(int id)
		{
			var user = await _magneto.QueryAsync(new UserById { Id = id }, default); // Use either "default" or "CacheOption.Default"
			var userAlbums = _magneto.Query(new AlbumsByUserId { UserId = id }, CacheOption.Default);
			return View("User", new UserViewModel { User = user, Albums = userAlbums });
		}

		[HttpPost("{id:int}")]
		public IActionResult Album(int id, string title)
		{
			_magneto.Command(new SaveAlbum { Album = new Album { Title = title, UserId = id } });
			return RedirectToAction(nameof(Index), new { id });
		}
	}

	public class AllUsers : AsyncCachedQuery<JsonPlaceHolderHttpClient, DistributedCacheEntryOptions, User[]>
	{
		protected override void CacheKey(ICache cache) => cache.Prefix = User.AllUsersCacheKeyPrefix;

		protected override DistributedCacheEntryOptions CacheEntryOptions(JsonPlaceHolderHttpClient context) =>
			User.AllUsersCacheEntryOptions();

		protected override Task<User[]> Query(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken) =>
			User.AllUsersAsync(context, cancellationToken);
	}

	public class UserById : AsyncTransformedCachedQuery<JsonPlaceHolderHttpClient, DistributedCacheEntryOptions, User[], User>
	{
		protected override void CacheKey(ICache cache) => cache.UsePrefix(User.AllUsersCacheKeyPrefix);

		protected override DistributedCacheEntryOptions CacheEntryOptions(JsonPlaceHolderHttpClient context) =>
			User.AllUsersCacheEntryOptions();

		protected override Task<User[]> Query(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken) =>
			User.AllUsersAsync(context, cancellationToken);

		protected override Task<User> TransformCachedResult(User[] cachedResult, CancellationToken cancellationToken) =>
			Task.FromResult(cachedResult.SingleOrDefault(x => x.Id == Id));

		public int Id { get; set; }
	}

	public class AlbumsByUserId : SyncTransformedCachedQuery<(IFileProvider, ILogger<AlbumsByUserId>), MemoryCacheEntryOptions, Album[], Album[]>
	{
		protected override void CacheKey(ICache cache) => cache.VaryByNothing();

		protected override MemoryCacheEntryOptions CacheEntryOptions((IFileProvider, ILogger<AlbumsByUserId>) context)
		{
			var (fileProvider, logger) = context;
			return new MemoryCacheEntryOptions()
				.AddExpirationToken(fileProvider.Watch(Album.AllAlbumsFilename))
				.RegisterPostEvictionCallback((key, value, reason, state) =>
					logger.LogInformation($"{key} : {value} was evicted due to {reason}"));
		}

		protected override Album[] Query((IFileProvider, ILogger<AlbumsByUserId>) context)
		{
			var (fileProvider, _) = context;
			using var streamReader = new StreamReader(fileProvider.GetFileInfo(Album.AllAlbumsFilename).CreateReadStream());
			var json = streamReader.ReadToEnd();
			return JsonSerializer.Deserialize<Album[]>(json);
		}

		protected override Album[] TransformCachedResult(Album[] cachedResult) => cachedResult.Where(x => x.UserId == UserId).ToArray();

		public int UserId { get; set; }
	}

	public class SaveAlbum : SyncCommand<(IFileProvider, JsonSerializerOptions)>
	{
		public override void Execute((IFileProvider, JsonSerializerOptions) context)
		{
			var (fileProvider, jsonSerializerOptions) = context;
			lock (fileProvider)
			{
				var fileInfo = fileProvider.GetFileInfo(Album.AllAlbumsFilename);

				string json;
				using (var streamReader = new StreamReader(fileInfo.CreateReadStream()))
					json = streamReader.ReadToEnd();

				var existingAlbums = JsonSerializer.Deserialize<Album[]>(json);
				Album.Id = existingAlbums.Max(x => x.Id) + 1;
				json = JsonSerializer.Serialize(existingAlbums.Concat(new[] { Album }), jsonSerializerOptions);

				File.WriteAllText(fileInfo.PhysicalPath, json);
			}
		}

		public Album Album { get; set; }
	}
}
