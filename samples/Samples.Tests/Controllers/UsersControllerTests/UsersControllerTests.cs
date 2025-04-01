using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Magneto;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Samples.Controllers;
using Samples.Domain;
using Samples.Models;

namespace Samples.Tests.Controllers.UsersControllerTests;

public class GettingIndex : ScenarioFor<UsersController>
{
	IActionResult _result = null!;
	readonly User[] _users =
	[
		new(),
		new(),
		new()
	];

	void GivenThereAreSomeKnownUsers() => The<IMagneto>().QueryAsync(new AllUsers(), CacheOption.Default).Returns(_users);
	async Task WhenGettingIndex() => _result = await SUT.Index();
	void ThenTheIndexViewIsDisplayed() => _result.Should().BeOfType<ViewResult>().Which.ViewName.Should().BeNull();
	void AndThenViewModelIsTheKnownUsers() => _result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<User[]>().Which.Should().BeSameAs(_users);
}

public class GettingUser : ScenarioFor<UsersController>
{
	IActionResult _result = null!;
	readonly User _user = new() { Id = 1 };
	readonly Album[] _albums =
	[
		new(),
		new(),
		new()
	];

	void GivenThereIsAKnownUser() => The<IMagneto>().QueryAsync(new UserById { Id = _user.Id }, CacheOption.Default).Returns(_user);
	void AndGivenThereAreSomeAlbumsForTheKnownUser() => The<IMagneto>().Query(new AlbumsByUserId { UserId = _user.Id }, CacheOption.Default).Returns(_albums);
	async Task WhenGettingIndexWithKnownUserId() => _result = await SUT.Index(_user.Id);
	void ThenTheUserViewIsDisplayed() => _result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("User");
	void AndThenViewModelIsTheKnownUserAndItsAlbums() => _result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<UserViewModel>().Which.Should().BeEquivalentTo(new UserViewModel
	{
		User = _user,
		Albums = _albums
	});
}

public class PostingAlbum : ScenarioFor<UsersController>
{
	IActionResult _result = null!;

	void WhenPostingAlbum() => _result = SUT.Album(1, "title");
	void ThenTheAlbumIsSaved() => The<IMagneto>().Received().Command(new SaveAlbum { Album = new() { UserId = 1, Title = "title" } });
	void AndThenTheResponseIsRedirectedToTheUser() => _result.Should().BeOfType<RedirectToActionResult>().Which.Should()
		.Match<RedirectToActionResult>(x => x.ActionName == "Index" && Equals(x.RouteValues!.SingleOrDefault(v => v.Key == "id").Value, 1));
}
