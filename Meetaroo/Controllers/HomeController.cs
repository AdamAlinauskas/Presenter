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

        public async Task<IActionResult> Index()
        {
            // When we have a proper signup process, this should be
            // run as a part of it.

            var userProfile = new {
                name = User.Identity.Name,
                email = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value,
                picture = User.Claims.FirstOrDefault(claim => claim.Type == "picture")?.Value,
                identifier = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value
            };

            await connection.OpenAsync();
            await connection.ExecuteAsync(
                @"INSERT INTO meetaroo_shared.users (name, email, picture, identifier)
                VALUES (@name, @email, @picture, @identifier)
                ON CONFLICT DO NOTHING",
                userProfile
            );

            return RedirectToAction("Index", "OrganizationAdmin");
        }
    }
}