using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Domain;
using Dto.Conversations;
using Dto.Deck;
using Meetaroo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Meetaroo.Controllers
{    
    [TypeFilterAttribute(typeof(RetrieveSchemaActionFilter))]
    [Authorize]
    public class DeckController : Controller
    {
        private readonly IRetrievePresentationToViewQuery retrievePresentationToViewQuery;
        private readonly IConversationRepository conversations;
        private readonly ICurrentSchema currentSchema;

        public DeckController(
            IRetrievePresentationToViewQuery retrievePresentationToViewQuery,
            IConversationRepository conversations,
            ICurrentSchema currentSchema
        )
        {
            this.retrievePresentationToViewQuery = retrievePresentationToViewQuery;
            this.conversations = conversations;
            this.currentSchema = currentSchema;
        }

        public async Task<IActionResult> Present(long id = 1)
        {
            var user = await this.GetCurrentUser();
            System.Console.WriteLine($"Fetching presentation {id}");
            var presentation = await retrievePresentationToViewQuery.Fetch(id);
            long conversationId = presentation.ConversationId;
            System.Console.WriteLine($"Fetching conversation {conversationId}");
            var conversation = await conversations.GetConversation(conversationId);
            var conversationDto = new ConversationDto {
                Id = conversation.Id,
                Topic = conversation.Topic,
                CreatedAt = conversation.CreatedAt,
                CreatedBy = conversation.CreatedBy.Name,
                CurrentUserIsMod = await CurrentUserIsMod(user, conversationId),
                Schema = currentSchema.Name
            };

            var viewModel = new DeckDto
            {
                Presentation = presentation,
                Conversation = conversationDto
            };
            return View("Deck", viewModel);
        }

        public async Task<JsonResult> GetMessages(long conversationId, long since)
        {
            var user = await this.GetCurrentUser();
            var messages = await conversations.GetMessages(conversationId, since, user.Id);

            return new JsonResult(new {
                messages,
                lastEventId = messages
                    .OrderByDescending(message => message.EventId)
                    .FirstOrDefault()
                    ?.EventId ?? since
            });
        }

        private async Task<bool> CurrentUserIsMod(User user, long conversationId)
        {
            return await conversations.IsModerator(user.Id, conversationId);
        }
    }
}