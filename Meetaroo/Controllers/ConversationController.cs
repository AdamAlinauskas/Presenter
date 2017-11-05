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
            return base.View(conversation);
        }

        [HttpPost]
        public async Task<ActionResult> AddMessage(long conversationId, string message)
        {
            var user = await this.GetCurrentUser();
            await repository.AddMessage(conversationId, message, user.Id);

            return new EmptyResult();
        }

        [HttpPost]
        public async Task<ActionResult> AddReply(long conversationId, long messageId, string message)
        {
            var user = await this.GetCurrentUser();
            if (await CurrentUserIsMod(user, conversationId))
                await repository.AddReply(conversationId, messageId, message, user.Id);

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

        public async Task<ActionResult> Boost(long id)
        {
            return new EmptyResult();
        }

        public async Task<ActionResult> RemoveBoost(long id)
        {
            return new EmptyResult();
        }

        private async Task<bool> CurrentUserIsMod(User user, long conversationId)
        {
            return await repository.IsModerator(user.Id, conversationId);
        }
    }
}