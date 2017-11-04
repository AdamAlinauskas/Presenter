using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccess;
using Meetaroo.Services;

namespace Meetaroo.Controllers
{
    [TypeFilterAttribute(typeof(RetrieveSchemaActionFilter))]
    [Authorize]
    public class ConversationHubController : Controller
    {
        private readonly ICurrentSchema currentSchema;
        private readonly IConversationRepository repository;

        public ConversationHubController(
            ICurrentSchema currentSchema,
            IConversationRepository repository
        )
        {
            this.currentSchema = currentSchema;
            this.repository = repository;
        }

        public async Task<ViewResult> Index()
        {
            var conversations = await repository.GetConversations();
            return View(conversations);
        }

        public async Task<ActionResult> Create(string topic)
        {
            var user = await this.GetCurrentUser();
            await repository.CreateConversation(topic, user.Id);

            return RedirectToRoute(
                "schemaBased",
                new { schema = currentSchema.Name, controller = "ConversationHub", action = "Index" }
            );
        }
    }
}