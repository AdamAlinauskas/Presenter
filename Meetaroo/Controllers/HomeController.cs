using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Meetaroo.Controllers
{
    public class HomeController : Controller
    {
        NpgsqlConnection connection;

        public HomeController(NpgsqlConnection connection)
        {
            this.connection = connection;
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "OrganizationAdmin");
            } else {
                return View();
            }
        }
    }
}