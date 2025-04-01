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

namespace Samples.Controllers;

[Route("[controller]")]
public class UsersController(IMagneto magneto) : Controller
{
	[HttpGet("")]
	public async Task<IActionResult> Index()
	{
		var users = await magneto.QueryAsync(new AllUsers(), CacheOption.Default);
		return View(users);
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Index(int id)
	{
		var user = await magneto.QueryAsync(new UserById { Id = id }, default); // Use either "default" or "CacheOption.Default"
		var userAlbums = magneto.Query(new AlbumsByUserId { UserId = id }, CacheOption.Default);
		return View("User", new UserViewModel { User = user!, Albums = userAlbums });
	}

	[HttpPost("{id:int}")]
	public IActionResult Album(int id, string title)
	{
		magneto.Command(new SaveAlbum { Album = new() { Title = title, UserId = id } });
		return RedirectToAction(nameof(Index), new { id });
	}
}

public class AllUsers : AsyncCachedQuery<JsonPlaceHolderHttpClient, DistributedCacheEntryOptions, User[]>
{
	protected override void CacheKey(IKey key) => key.Prefix = User.AllUsersCacheKeyPrefix;

	protected override DistributedCacheEntryOptions CacheEntryOptions(JsonPlaceHolderHttpClient context) =>
		User.AllUsersCacheEntryOptions();

	protected override Task<User[]> Query(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken) =>
		User.AllUsersAsync(context, cancellationToken).AsTask();
}

public class UserById : AsyncTransformedCachedQuery<JsonPlaceHolderHttpClient, DistributedCacheEntryOptions, User[], User?>
{
	protected override void CacheKey(IKey key) => key.UsePrefix(User.AllUsersCacheKeyPrefix);

	protected override DistributedCacheEntryOptions CacheEntryOptions(JsonPlaceHolderHttpClient context) =>
		User.AllUsersCacheEntryOptions();

	protected override Task<User[]> Query(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken) =>
		User.AllUsersAsync(context, cancellationToken).AsTask();

	protected override Task<User?> TransformCachedResult(User[] cachedResult, CancellationToken cancellationToken) =>
		Task.FromResult(cachedResult.SingleOrDefault(x => x.Id == Id));

	public required int Id { get; init; }
}

public class AlbumsByUserId : SyncTransformedCachedQuery<(IFileProvider, ILogger<AlbumsByUserId>), MemoryCacheEntryOptions, Album[], Album[]>
{
	protected override void CacheKey(IKey key) => key.VaryByNothing();

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
		return JsonSerializer.Deserialize<Album[]>(json) ?? [];
	}

	protected override Album[] TransformCachedResult(Album[] cachedResult) => [.. cachedResult.Where(x => x.UserId == UserId)];

	public required int UserId { get; init; }
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

			var existingAlbums = JsonSerializer.Deserialize<Album[]>(json)!;
			Album.Id = existingAlbums.Max(x => x.Id) + 1;
			json = JsonSerializer.Serialize(existingAlbums.Concat([Album]), jsonSerializerOptions);

			File.WriteAllText(fileInfo.PhysicalPath!, json);
		}
	}

	public required Album Album { get; init; }
}
