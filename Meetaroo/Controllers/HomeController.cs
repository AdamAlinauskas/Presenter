using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

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
            //var orgs = await context.Organizations.ToListAsync();
            //return new ContentResult {
            //    Content = "Organizations:\n" + string.Join("\n", orgs.Select(o => o.Name).ToArray())
            //};
            await connection.OpenAsync();

            var result = new StringBuilder();

            using (var cmd = new NpgsqlCommand("SELECT id, display_name FROM Organizations", connection))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    result.AppendLine(reader.GetString(1));
                }
            }

            return new ContentResult
            {
                ContentType = "text",
                Content = result.ToString(),
                StatusCode = 200
            };
        }
    }
}