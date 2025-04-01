using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magneto;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Samples.Controllers;
using Samples.Domain;
using Samples.Models;

namespace Samples.Tests.Controllers.PostsControllerTests;

public class GettingIndex : ScenarioFor<PostsController>
{
	IActionResult _result = null!;
	readonly Post[] _posts =
	[
		new(),
		new(),
		new()
	];

	void GivenThereAreSomeKnownPosts() => The<IMagneto>().QueryAsync(new AllPosts()).Returns(_posts);
	async Task WhenGettingIndex() => _result = await SUT.Index();
	void ThenTheIndexViewIsDisplayed() => _result.Should().BeOfType<ViewResult>().Which.ViewName.Should().BeNull();
	void AndThenViewModelIsTheKnownPosts() => _result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<Post[]>().Which.Should().BeSameAs(_posts);
}

public class GettingPost : ScenarioFor<PostsController>
{
	IActionResult _result = null!;
	readonly Post _post = new() { Id = 1 };
	readonly Comment[] _comments =
	[
		new(),
		new(),
		new()
	];

	void GivenThereIsAKnownPost() => The<IMagneto>().QueryAsync(new PostById { Id = _post.Id }, CacheOption.Default).Returns(_post);
	void AndGivenThereAreSomeCommentsForTheKnownPost() => The<IMagneto>().QueryAsync(new CommentsByPostId { PostId = _post.Id }, CacheOption.Default).Returns(_comments);
	async Task WhenGettingIndexWithKnownPostId() => _result = await SUT.Index(_post.Id, CancellationToken.None);
	void ThenThePostViewIsDisplayed() => _result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Post");
	void AndThenViewModelIsTheKnownPostAndItsComments() => _result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<PostViewModel>().Which.Should().BeEquivalentTo(new PostViewModel
	{
		Post = _post,
		Comments = _comments
	});
}

public class PostingPost : ScenarioFor<PostsController>
{
	IActionResult _result = null!;
	readonly Post _post = new() { Id = 1, UserId = 2, Title = "title", Body = "body" };

	void GivenThereIsAKnownPost() => The<IMagneto>().QueryAsync(new PostById { Id = _post.Id }, CacheOption.Refresh).Returns(_post);
	async Task WhenPostingNewBodyForExistingPost() => _result = await SUT.Post(_post.Id, "updated");
	void ThenThePostIsSavedWithNewBody() => The<IMagneto>().Received().CommandAsync(new SavePost { Post = new() { Id = _post.Id, Title = _post.Title, UserId = _post.UserId, Body = "updated" } });
	void AndThenTheResponseIsRedirectedToThePost() => _result.Should().BeOfType<RedirectToActionResult>().Which.Should()
		.Match<RedirectToActionResult>(x => x.ActionName == "Index" && Equals(x.RouteValues!.SingleOrDefault(v => v.Key == "id").Value, 1));
}
