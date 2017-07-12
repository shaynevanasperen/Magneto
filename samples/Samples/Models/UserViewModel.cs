using Samples.Domain;

namespace Samples.Models
{
    public class UserViewModel
    {
	    public User User { get; set; }
	    public Album[] Albums { get; set; }
	}
}
