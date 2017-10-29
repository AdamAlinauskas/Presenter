using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Service;

namespace Meetaroo.Controllers
{
    [TypeFilterAttribute(typeof(RetrieveSchemaActionFilter))]
    [Authorize]
    public class ConversationController : Controller
    {
        private readonly ICurrentSchema currentSchema;
        private readonly NpgsqlConnection connection;

        public ConversationController(ICurrentSchema currentSchema, NpgsqlConnection connection)
        {
            this.currentSchema = currentSchema;
            this.connection = connection;
        }

        public async Task<ViewResult> Index(long id) {
            await Connect();
            var conversation = await connection.QueryFirstAsync<dynamic>(
                "SELECT * FROM conversations WHERE id = @id",
                new { id }
            );
            return View(conversation);
        }

        [HttpPost]
        public async Task<ActionResult> AddMessage(string message) {
            return new ContentResult { Content = message };
        }

        private async Task Connect()
        {
            await connection.OpenAsync();

            // Can't parameterize this
            await connection.ExecuteAsync(
                "set search_path = " + currentSchema.Name
            );
        }
    }
}