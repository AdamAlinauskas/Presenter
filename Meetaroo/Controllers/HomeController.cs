using Microsoft.AspNetCore.Mvc;
using Meetaroo.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Meetaroo.Controllers
{
    public class HomeController : Controller
    {
        private readonly MeetarooContext context;

        public HomeController(MeetarooContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orgs = await context.Organizations.ToListAsync();
            return new ContentResult {
                Content = "Organizations:\n" + string.Join("\n", orgs.Select(o => o.Name).ToArray())
            };
        }
    }
}