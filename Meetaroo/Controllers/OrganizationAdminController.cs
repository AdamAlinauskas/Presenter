using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace Meetaroo.Controllers
{
    public class OrganizationAdminController : Controller
    {
        NpgsqlConnection connection;

        public OrganizationAdminController(NpgsqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<IActionResult> Index()
        {
            await connection.OpenAsync();

            var result = await connection.QueryAsync("SELECT id, display_name FROM Organizations");
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string schemaName, string displayName)
        {
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                "INSERT INTO meetaroo_shared.organizations (schema_name, display_name) VALUES (@schemaName, @displayName)",
                new { schemaName, displayName }
            );

            // TODO AP : Actually create schema and migrate it up

            return RedirectToAction("Index");
        }
    }
}