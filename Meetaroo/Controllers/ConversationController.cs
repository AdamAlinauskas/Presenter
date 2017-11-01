using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Service;
using DataAccess;

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
        public async Task<ActionResult> AddMessage(long conversationId, string message) {
            await Connect();
            var userId = await GetUserIdAsync();
            var transaction = connection.BeginTransaction();

            try
            {
                var messageResult = await connection.QueryFirstAsync<dynamic>(
                    @"INSERT INTO
                        messages (text, conversation_id, created_by)
                        VALUES (@message, @conversationId, @userId)
                        RETURNING id",
                    new { message, conversationId, userId }
                );
                await connection.ExecuteAsync(
                    @"INSERT INTO
                        message_events(message_id, event_type, created_by)
                        VALUES (@messageId, 'message_created', @userId)",
                    new { messageId = messageResult.id, userId }
                );

                await transaction.CommitAsync();
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return new ContentResult { Content = message };
        }

        public async Task<JsonResult> GetMessages(long conversationId, long since)
        {
            await Connect();
            var messages = await connection.QueryAsync(
                @"SELECT
                    message.id,
                    message.created_at,
                    message.text,
                    author.name AS author_name,
                    event.id AS event_id
                FROM message_events AS event
                INNER JOIN messages AS message ON event.message_id = message.id
                INNER JOIN meetaroo_shared.users AS author ON message.created_by = author.id
                WHERE
                    event.id > @since
                    AND message.conversation_id = @conversationId
                ORDER BY message.created_at",
                new { since, conversationId }
            );

            return new JsonResult(new {
                messages,
                lastEventId = messages
                    .OrderByDescending(message => message.event_id)
                    .FirstOrDefault()
                    ?.event_id ?? since
            });
        }

        private async Task Connect()
        {
            await connection.OpenAsync();

            // Can't parameterize this
            await connection.ExecuteAsync(
                "set search_path = " + currentSchema.Name
            );
        }

        private async Task<int> GetUserIdAsync()
        {
            var userIdentifier = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var user = await connection.QueryFirstAsync<dynamic>(
                "SELECT id FROM meetaroo_shared.users WHERE identifier = @identifier",
                new { identifier = userIdentifier }
            );
            return (int) user.id;
        }
    }
}