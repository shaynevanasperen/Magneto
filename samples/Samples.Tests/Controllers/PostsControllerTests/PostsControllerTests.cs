using System.Threading.Tasks;
using FluentAssertions;
using Magneto;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Samples.Controllers;
using Samples.Domain;
using Samples.Models;

namespace Samples.Tests.Controllers.PostsControllerTests
{
	public class GettingIndex : ScenarioFor<PostsController>
	{
		IActionResult _result;
		readonly Post[] _posts =
		{
			new Post(),
			new Post(),
			new Post()
		};

		void Setup() => SetThe(new JsonPlaceHolderHttpClient());
		void GivenThereAreSomeKnownPosts() => The<IInvoker>().QueryAsync(new AllPosts(), The<JsonPlaceHolderHttpClient>()).Returns(_posts);
		async Task WhenGettingIndex() => _result = await SUT.Index();
		void ThenTheIndexViewIsDisplayed() => _result.Should().BeOfType<ViewResult>().Which.ViewName.Should().BeNull();
		void AndThenViewModelIsTheKnownPosts() => _result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<Post[]>().Which.Should().BeSameAs(_posts);
	}

	public class GettingPost : ScenarioFor<PostsController>
	{
		IActionResult _result;
		readonly Post _post = new Post { Id = 1 };
		readonly Comment[] _comments =
		{
			new Comment(),
			new Comment(),
			new Comment()
		};

		void Setup() => SetThe(new JsonPlaceHolderHttpClient());
		void GivenThereIsAKnownPost() => The<IInvoker>().QueryAsync(new PostById { Id = _post.Id }, The<JsonPlaceHolderHttpClient>()).Returns(_post);
		void AndGivenThereAreSomeCommentsForTheKnownPost() => The<IInvoker>().QueryAsync(new CommentsByPostId { PostId = _post.Id }, The<JsonPlaceHolderHttpClient>()).Returns(_comments);
		async Task WhenGettingIndexWithKnownPostId() => _result = await SUT.Index(_post.Id);
		void ThenThePostViewIsDisplayed() => _result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Post");
		void AndThenViewModelIsTheKnownPostAndItsComments() => _result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<PostViewModel>().Which.ShouldBeEquivalentTo(new PostViewModel
		{
			Post = _post,
			Comments = _comments
		});
	}
}
