using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Samples.Domain
{
	public class Address
	{
		public string Street { get; set; }
		public string Suite { get; set; }
		public string City { get; set; }
		public string Zipcode { get; set; }
		public Geo Geo { get; set; }
	}

	public class Album
	{
		public int UserId { get; set; }
		public int Id { get; set; }
		public string Title { get; set; }

		public const string AllAlbumsFilename = "albums.json";
	}

	public class Comment
	{
		public int PostId { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Body { get; set; }
	}

	public class Company
	{
		public string Name { get; set; }
		public string CatchPhrase { get; set; }
		public string Bs { get; set; }
	}

	public class Geo
	{
		public string Lat { get; set; }
		public string Lng { get; set; }
	}

	public class Post
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }
	}

	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public Address Address { get; set; }
		public string Phone { get; set; }
		public string Website { get; set; }
		public Company Company { get; set; }

		public const string AllUsersCacheKeyPrefix = "Samples.Domain.Users";

		public static DistributedCacheEntryOptions AllUsersCacheEntryOptions() =>
			new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

		public static Task<User[]> AllUsersAsync(JsonPlaceHolderHttpClient context, CancellationToken cancellationToken = default) =>
			context.GetAsync<User[]>("/users", cancellationToken);
	}
}
