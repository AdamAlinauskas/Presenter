using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Service;

namespace Meetaroo.Hubs
{
    public class Conversation : Hub
    {
        private IServiceProvider services;
        private ICurrentSchema currentSchema;
        private HttpContext httpContext;
        private IUserRepository users;
        private IConversationRepository conversations;

        public Conversation(IServiceProvider services, ICurrentSchema currentSchema, IUserRepository users, IConversationRepository conversations)
        {
            var contextAccess = services.GetService<IHttpContextAccessor>();
            httpContext = contextAccess.HttpContext;
            
            this.services = services;
            this.currentSchema = currentSchema;
            this.users = users;
            this.conversations = conversations;
        }

        public async Task JoinConversation(string conversationId)
        {
            Console.WriteLine($"Conversation {conversationId} joined");
            await Groups.AddAsync(Context.ConnectionId, conversationId);
        }

        public async Task PostMessage(string conversationIdentifier, string schema, string text)
        {
            var user = await GetCurrentUser();
            currentSchema.Name = schema;
            Console.WriteLine($"Message posted by {user.Name}: {text}");

            var conversationId = long.Parse(conversationIdentifier);
            var messageId = await conversations.AddMessage(conversationId, text, user.Id);
            // Don't really need to fetch message if we have events for each type of action
            // But safer against desync issues
            var message = await conversations.GetMessage(messageId, user.Id);
            await Clients.Group(conversationIdentifier).InvokeAsync(
                "message",
                // This is silly. The JSONifier from Controller lowercases all the properties,
                // but the one from Hub doesn't. I just want consistency.
                new {
                    messageId = message.MessageId,
                    repliesToId = message.RepliesToId,
                    createdAt = message.CreatedAt,
                    text = message.Text,
                    author = message.Author,
                    authorPicture = message.AuthorPicture,
                    eventId = message.EventId,
                    boosts = message.Boosts,
                    boostedByCurrentUser = message.BoostedByCurrentUser
                }
            );
        }

        // This is pinched from ControllerExtensions. It belongs in a proper-ass service
        private async Task<User> GetCurrentUser() {
            var userIdentifier = httpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            return await users.GetByIdentifier(userIdentifier);
        }
    }
}