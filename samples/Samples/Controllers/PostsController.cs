using System;
using System.Net.Http;
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
		readonly IInvoker _invoker;
		readonly JsonPlaceHolderHttpClient _httpClient;

		public PostsController(IInvoker invoker, JsonPlaceHolderHttpClient httpClient)
		{
			_invoker = invoker;
			_httpClient = httpClient;
		}

		[HttpGet("")]
		public async Task<IActionResult> Index()
		{
			var posts = await _invoker.QueryAsync(new AllPosts(), _httpClient);
			return View(posts);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> Index(int id)
		{
			var post = await _invoker.QueryAsync(new PostById { Id = id }, _httpClient);
			var postComments = await _invoker.QueryAsync(new CommentsByPostId { PostId = id }, _httpClient);
			return View("Post", new PostViewModel { Post = post, Comments = postComments });
		}
	}

	public class AllPosts : AsyncQuery<JsonPlaceHolderHttpClient, Post[]>
	{
		public override async Task<Post[]> ExecuteAsync(JsonPlaceHolderHttpClient context)
		{
			var response = await context.GetAsync("/posts");
			return await response.Content.ReadAsAsync<Post[]>();
		}

		public int Id { get; set; }
	}

	public class PostById : AsyncQuery<JsonPlaceHolderHttpClient, Post>
	{
		public override async Task<Post> ExecuteAsync(JsonPlaceHolderHttpClient context)
		{
			var response = await context.GetAsync($"/posts/{Id}");
			return await response.Content.ReadAsAsync<Post>();
		}

		public int Id { get; set; }
	}

	public class CommentsByPostId : AsyncCachedQuery<JsonPlaceHolderHttpClient, MemoryCacheEntryOptions, Comment[]>
	{
		protected override void ConfigureCache(ICacheConfig cacheConfig) => cacheConfig.VaryBy(PostId);

		protected override MemoryCacheEntryOptions GetCacheEntryOptions(JsonPlaceHolderHttpClient context) => new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

		protected override async Task<Comment[]> QueryAsync(JsonPlaceHolderHttpClient context)
		{
			var response = await context.GetAsync($"/posts/{PostId}/comments");
			return await response.Content.ReadAsAsync<Comment[]>();
		}

		public int PostId { get; set; }
	}
}
