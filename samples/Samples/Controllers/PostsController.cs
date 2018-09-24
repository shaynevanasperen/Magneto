using System;
using System.Threading;
using System.Threading.Tasks;
using Magneto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Samples.Domain;
using Samples.Models;

namespace Samples.Controllers
{
	[Route("[controller]")]
	public class PostsController : Controller
	{
		readonly IMagneto _magneto;

		public PostsController(IMagneto magneto)
		{
			_magneto = magneto;
		}

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
	}

	public class AllPosts : AsyncQuery<JsonPlaceHolderHttpClient, Post[]>
	{
		public override Task<Post[]> ExecuteAsync(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken = default) => context.GetAsync<Post[]>("/posts", cancellationToken);
	}

	public class PostById : AsyncQuery<JsonPlaceHolderHttpClient, Post>
	{
		public override Task<Post> ExecuteAsync(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken = default) => context.GetAsync<Post>($"/posts/{Id}", cancellationToken);

		public int Id { get; set; }
	}

	public class CommentsByPostId : AsyncCachedQuery<JsonPlaceHolderHttpClient, MemoryCacheEntryOptions, Comment[]>
	{
		protected override void ConfigureCache(ICacheConfig cacheConfig) => cacheConfig.VaryBy(PostId);

		protected override MemoryCacheEntryOptions GetCacheEntryOptions(JsonPlaceHolderHttpClient context) => new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

		protected override Task<Comment[]> QueryAsync(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken = default) => context.GetAsync<Comment[]>($"/posts/{PostId}/comments", cancellationToken);

		public int PostId { get; set; }
	}
}
