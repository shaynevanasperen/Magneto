using Microsoft.AspNetCore.Mvc;

namespace Samples.Controllers
{
	public class HomeController : Controller
	{
		[Route("")]
		public IActionResult Index() => View();

		[Route("Error")]
		public IActionResult Error() => View();
	}
}
