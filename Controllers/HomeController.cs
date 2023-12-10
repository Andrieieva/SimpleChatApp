using Microsoft.AspNetCore.Mvc;

namespace SimpleChatApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
