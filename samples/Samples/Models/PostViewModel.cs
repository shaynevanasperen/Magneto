using Samples.Domain;

namespace Samples.Models;

public class PostViewModel
{
	public required Post Post { get; init; }
	public required Comment[] Comments { get; init; }
}
