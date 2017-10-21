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
            var content = string.Join("\n", result.Select(row => $"{row.id}: {row.display_name}"));
            
            return new ContentResult
            {
                ContentType = "text",
                Content = content,
                StatusCode = 200
            };
        }
    }
}