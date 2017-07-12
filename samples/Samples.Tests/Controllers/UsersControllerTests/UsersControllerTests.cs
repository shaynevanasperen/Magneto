using System.Threading.Tasks;
using FluentAssertions;
using Magneto;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Samples.Controllers;
using Samples.Domain;
using Samples.Models;

namespace Samples.Tests.Controllers.UsersControllerTests
{
	public class GettingIndex : ScenarioFor<UsersController>
	{
		IActionResult _result;
		readonly User[] _users =
		{
			new User(),
			new User(),
			new User()
		};

		void GivenThereAreSomeKnownUsers() => The<IDispatcher>().QueryAsync(new AllUsers()).Returns(_users);
		async Task WhenGettingIndex() => _result = await SUT.Index();
		void ThenTheIndexViewIsDisplayed() => _result.Should().BeOfType<ViewResult>().Which.ViewName.Should().BeNull();
		void AndThenViewModelIsTheKnownUsers() => _result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<User[]>().Which.Should().BeSameAs(_users);
	}

	public class GettingUser : ScenarioFor<UsersController>
	{
		IActionResult _result;
		readonly User _user = new User { Id = 1 };
		readonly Album[] _albums =
		{
			new Album(),
			new Album(),
			new Album()
		};

		void GivenThereIsAKnownUser() => The<IDispatcher>().QueryAsync(new UserById { Id = _user.Id }).Returns(_user);
		void AndGivenThereAreSomeAlbumsForTheKnownUser() => The<IDispatcher>().Query(new AlbumsByUserId { UserId = _user.Id }).Returns(_albums);
		async Task WhenGettingIndexWithKnownUserId() => _result = await SUT.Index(_user.Id);
		void ThenTheUserViewIsDisplayed() => _result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("User");
		void AndThenViewModelIsTheKnownUserAndItsAlbums() => _result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<UserViewModel>().Which.ShouldBeEquivalentTo(new UserViewModel
		{
			User = _user,
			Albums = _albums
		});
	}

	public class PostingAlbum : ScenarioFor<UsersController>
	{
		IActionResult _result;

		void WhenPostingAlbum() => _result = SUT.Album(1, "Title");
		void ThenTheAlbumIsSaved() => The<IDispatcher>().Received().Command(new SaveAlbum { Album = new Album { UserId = 1, Title = "Title" } });
		void AndThenTheReponseIsRedirectedToTheUser() => _result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
	}
}
