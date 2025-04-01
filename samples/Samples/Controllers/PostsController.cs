using System;
using System.Threading;
using System.Threading.Tasks;
using Magneto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Samples.Domain;
using Samples.Models;

namespace Samples.Controllers;

[Route("[controller]")]
public class PostsController(IMagneto magneto) : Controller
{
	[HttpGet("")]
	public async Task<IActionResult> Index()
	{
		var posts = await magneto.QueryAsync(new AllPosts());
		return View(posts);
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Index(int id, CancellationToken token)
	{
		var post = await magneto.QueryAsync(new PostById { Id = id }, CacheOption.Default, token);
		var postComments = await magneto.QueryAsync(new CommentsByPostId { PostId = id }, CacheOption.Default, token);
		return View("Post", new PostViewModel { Post = post, Comments = postComments });
	}

	[HttpPost("{id:int}")]
	public async Task<IActionResult> Post(int id, string body)
	{
		var postById = new PostById { Id = id };
		// Make sure to bypass reading from the cache, as we know we are about to modify this entity
		var post = await magneto.QueryAsync(postById, CacheOption.Refresh);
		post.Body = body;
		await magneto.CommandAsync(new SavePost { Post = post });
		// When using a distributed cache, we should either evict or update the entry, since we know it to be stale now
		await magneto.UpdateCachedResultAsync(postById);
		//await magneto.EvictCachedResultAsync(postById);

		return RedirectToAction(nameof(Index), new { id });
	}
}

public class AllPosts : AsyncQuery<JsonPlaceHolderHttpClient, Post[]>
{
	protected override Task<Post[]> Query(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken) =>
		context.GetAsync<Post[]>("/posts", cancellationToken).AsTask();
}

public class PostById : AsyncCachedQuery<JsonPlaceHolderHttpClient, DistributedCacheEntryOptions, Post>
{
	protected override void CacheKey(IKey key) => key.VaryBy = Id;

	protected override DistributedCacheEntryOptions CacheEntryOptions(JsonPlaceHolderHttpClient context) =>
		new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

	protected override Task<Post> Query(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken) =>
		context.GetAsync<Post>($"/posts/{Id}", cancellationToken).AsTask();

	public int Id { get; init; }
}

public class CommentsByPostId : AsyncCachedQuery<JsonPlaceHolderHttpClient, MemoryCacheEntryOptions, Comment[]>
{
	protected override void CacheKey(IKey key) => key.VaryBy(PostId);

	protected override MemoryCacheEntryOptions CacheEntryOptions(JsonPlaceHolderHttpClient context) =>
		new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

	protected override Task<Comment[]> Query(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken) =>
		context.GetAsync<Comment[]>($"/posts/{PostId}/comments", cancellationToken).AsTask();

	public int PostId { get; init; }
}

public class SavePost : AsyncCommand<JsonPlaceHolderHttpClient>
{
	public override Task Execute(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken) =>
		context.PostAsync($"/posts/{Post.Id}", Post, cancellationToken);

	public required Post Post { get; init; }
}
