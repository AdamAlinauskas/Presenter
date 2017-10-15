using Microsoft.AspNetCore.Mvc;

namespace Meetaroo.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            //var orgs = await context.Organizations.ToListAsync();
            //return new ContentResult {
            //    Content = "Organizations:\n" + string.Join("\n", orgs.Select(o => o.Name).ToArray())
            //};
            return new ContentResult();
        }
    }
}