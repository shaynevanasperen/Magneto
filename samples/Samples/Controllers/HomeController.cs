using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Samples.Models;

namespace Samples.Controllers;

public class HomeController : Controller
{
	[Route("")]
	public IActionResult Index() => View();

	[Route("Error")]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
