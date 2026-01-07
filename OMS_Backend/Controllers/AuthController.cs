using Microsoft.AspNetCore.Mvc;

namespace OMS_Backend.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
