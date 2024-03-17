
using Microsoft.AspNetCore.Mvc;

namespace ErmitApi.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
