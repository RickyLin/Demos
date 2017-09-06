using Microsoft.AspNetCore.Mvc;

namespace CustomizedViewLocation.Home
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}