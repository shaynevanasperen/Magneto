using Samples.Domain;

namespace Samples.Models
{
    public class PostViewModel
    {
	    public Post Post { get; set; }
	    public Comment[] Comments { get; set; }
	}
}
