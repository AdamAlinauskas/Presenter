using Microsoft.AspNetCore.Mvc;

namespace Meetaroo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "OrganizationAdmin");
        }
    }
}