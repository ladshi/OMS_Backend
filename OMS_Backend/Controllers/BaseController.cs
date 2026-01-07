using Microsoft.AspNetCore.Mvc;

namespace OMS_Backend.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
