using System;
using System.Threading;
using System.Threading.Tasks;
using Magneto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Samples.Domain;
using Samples.Models;

namespace Samples.Controllers
{
	[Route("[controller]")]
	public class PostsController : Controller
	{
		readonly IMagneto _magneto;

		public PostsController(IMagneto magneto) => _magneto = magneto;

		[HttpGet("")]
		public async Task<IActionResult> Index()
		{
			var posts = await _magneto.QueryAsync(new AllPosts());
			return View(posts);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> Index(int id)
		{
			var post = await _magneto.QueryAsync(new PostById { Id = id });
			var postComments = await _magneto.QueryAsync(new CommentsByPostId { PostId = id });
			return View("Post", new PostViewModel { Post = post, Comments = postComments });
		}

		[HttpPost("{id:int}")]
		public async Task<IActionResult> Post(Post model)
		{
			var postById = new PostById { Id = model.Id };
			var post = await _magneto.QueryAsync(postById);
			post.Body = model.Body;
			await _magneto.CommandAsync(new SavePost { Post = post });
			// When using a distributed cache, we should either evict or update the entry, since we know it to be stale now
			await _magneto.UpdateCachedResultAsync(postById);
			//await _magneto.EvictCachedResultAsync(postById);

			return RedirectToAction(nameof(Index), new { post.Id });
		}
	}

	public class AllPosts : AsyncQuery<JsonPlaceHolderHttpClient, Post[]>
	{
		protected override Task<Post[]> QueryAsync(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken = default) => context.GetAsync<Post[]>("/posts", cancellationToken);
	}

	public class PostById : AsyncCachedQuery<JsonPlaceHolderHttpClient, DistributedCacheEntryOptions, Post>
	{
		protected override void ConfigureCache(ICacheConfig cacheConfig) => cacheConfig.VaryBy(Id);

		protected override DistributedCacheEntryOptions GetCacheEntryOptions(JsonPlaceHolderHttpClient context) => new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

		protected override Task<Post> QueryAsync(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken = default) => context.GetAsync<Post>($"/posts/{Id}", cancellationToken);

		public int Id { get; set; }
	}

	public class CommentsByPostId : AsyncCachedQuery<JsonPlaceHolderHttpClient, MemoryCacheEntryOptions, Comment[]>
	{
		protected override void ConfigureCache(ICacheConfig cacheConfig) => cacheConfig.VaryBy(PostId);

		protected override MemoryCacheEntryOptions GetCacheEntryOptions(JsonPlaceHolderHttpClient context) => new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

		protected override Task<Comment[]> QueryAsync(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken = default) => context.GetAsync<Comment[]>($"/posts/{PostId}/comments", cancellationToken);

		public int PostId { get; set; }
	}

	public class SavePost : AsyncCommand<JsonPlaceHolderHttpClient>
	{
		public override Task ExecuteAsync(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken = default) => context.PostAsync($"/posts/{Post.Id}", Post, cancellationToken);

		public Post Post { get; set; }
	}
}
