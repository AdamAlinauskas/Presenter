
using System;
using System.Linq;
using System.Security.Claims;
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
    public class ConversationHubController : Controller
    {
        private readonly ICurrentSchema currentSchema;
        private readonly NpgsqlConnection connection;

        public ConversationHubController(ICurrentSchema currentSchema, NpgsqlConnection connection)
        {
            this.currentSchema = currentSchema;
            this.connection = connection;
        }

        public async Task<ViewResult> Index()
        {
            await Connect();

            // Joining on meetaroo_shared.users only works while
            // we have only one database server
            var queryResult = await connection.QueryAsync(
                @"SELECT
                    conversations.id AS id,
                    topic,
                    name as created_by
                FROM conversations
                INNER JOIN meetaroo_shared.users AS users ON conversations.created_by = users.id
                ORDER BY created_at"
            );

            return View(queryResult);
        }

        public async Task<ActionResult> Create(string topic)
        {
            await Connect();
            var userId = await GetUserIdAsync();
            System.Console.WriteLine($"User id is {userId}");

            // TODO AP : Actually create the conversation

            return RedirectToRoute(
                "schemaBased",
                new { schema = currentSchema.Name, controller = "ConversationHub", action = "Index" }
            );
        }

        private async Task<int> GetUserIdAsync()
        {
            var userIdentifier = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            System.Console.WriteLine($"Looking for user : {userIdentifier}");
            var user = await connection.QueryFirstAsync<dynamic>(
                "SELECT id FROM meetaroo_shared.users WHERE identifier = @identifier",
                new { identifier = userIdentifier }
            );
            return (int) user.id;
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