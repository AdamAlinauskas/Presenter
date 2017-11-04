using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Service;
using DataAccess;
using Meetaroo.Services;

namespace Meetaroo.Controllers
{
    [TypeFilterAttribute(typeof(RetrieveSchemaActionFilter))]
    [Authorize]
    public class ConversationHubController : Controller
    {
        private readonly ICurrentSchema currentSchema;
        private readonly NpgsqlConnection connection;
        private readonly IConversationRepository repository;

        public ConversationHubController(
            ICurrentSchema currentSchema,
            NpgsqlConnection connection,
            IConversationRepository repository
        )
        {
            this.currentSchema = currentSchema;
            this.connection = connection;
            this.repository = repository;
        }

        public async Task<ViewResult> Index()
        {
            var conversations = await repository.GetConversations();
            return View(conversations);
        }

        public async Task<ActionResult> Create(string topic)
        {
            await Connect();
            var user = await this.GetCurrentUser();

            // TODO AP : Actually create the conversation
            await connection.ExecuteAsync(
                "INSERT INTO conversations (topic, created_by) VALUES (@topic, @userId)",
                new { topic, userId = user.Id }
            );

            return RedirectToRoute(
                "schemaBased",
                new { schema = currentSchema.Name, controller = "ConversationHub", action = "Index" }
            );
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