using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccess;
using Meetaroo.Services;

namespace Meetaroo.Controllers
{
    [TypeFilterAttribute(typeof(RetrieveSchemaActionFilter))]
    [Authorize]
    public class ConversationController : Controller
    {
        private readonly ICurrentSchema currentSchema;
        private readonly IConversationRepository repository;

        public ConversationController(ICurrentSchema currentSchema, IConversationRepository repository)
        {
            this.currentSchema = currentSchema;
            this.repository = repository;
        }

        public async Task<ViewResult> Index(long id) {
            var conversation = await repository.GetConversation(id);
            return View(conversation);
        }

        [HttpPost]
        public async Task<ActionResult> AddMessage(long conversationId, string message) {
            var user = await this.GetCurrentUser();
            await repository.AddMessage(conversationId, message, user.Id);

            return new EmptyResult();
        }

        public async Task<JsonResult> GetMessages(long conversationId, long since)
        {
            var messages = await repository.GetMessages(conversationId, since);

            return new JsonResult(new {
                messages,
                lastEventId = messages
                    .OrderByDescending(message => message.EventId)
                    .FirstOrDefault()
                    ?.EventId ?? since
            });
        }
    }
}