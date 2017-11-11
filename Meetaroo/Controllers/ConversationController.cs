using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccess;
using Meetaroo.Services;
using Domain;
using System;

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

        public async Task<ViewResult> Index(long id)
        {
            var user = await this.GetCurrentUser();
            var conversation = await repository.GetConversation(id);
            bool isMod = await CurrentUserIsMod(user, id);
            ViewBag.CurrentUserIsMod = isMod;
            ViewBag.Schema = currentSchema.Name;
            return base.View(conversation);
        }

        public async Task<JsonResult> GetMessages(long conversationId, long since)
        {
            var user = await this.GetCurrentUser();
            var messages = await repository.GetMessages(conversationId, since, user.Id);

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
            return await repository.IsModerator(user.Id, conversationId);
        }
    }
}