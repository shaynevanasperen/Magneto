using Samples.Domain;

namespace Samples.Models;

public class UserViewModel
{
	public required User User { get; init; }
	public required Album[] Albums { get; init; }
}
