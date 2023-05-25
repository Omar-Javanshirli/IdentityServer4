using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.AuthServer.Controllers
{
    public class Ir : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
