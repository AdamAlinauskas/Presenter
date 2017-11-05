using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain;
using Dto;
using Npgsql;

namespace DataAccess
{
    public interface IConversationRepository
    {
        Task<Conversation> GetConversation(long id);
        Task<IEnumerable<Conversation>> GetConversations();
        Task CreateConversation(string topic, long createdBy);
        Task<IEnumerable<ViewMessageDto>> GetMessages(long conversationId, long lastSeenMessage);
        Task AddMessage(long conversationId, string message, long id);
        Task AddReply(long conversationId, long messageId, string message, long createdBy);
        Task<bool> IsModerator(long userId, long conversationId);
    }

    public class ConversationRepository : BaseRepository, IConversationRepository
    {
        public ConversationRepository(NpgsqlConnection connection, ICurrentSchema currentSchema) : base(connection, currentSchema)
        {
        }

        public async Task<IEnumerable<Conversation>> GetConversations()
        {
            // Joining on meetaroo_shared.users only works while
            // we have only one database server
            await ConnectAndSetSchema();
            return await connection.QueryAsync<Conversation, User, Conversation>(
                @"SELECT
                    conversations.id AS Id,
                    topic AS Topic,
                    created_at AS CreatedAt,
                    users.id AS Id,
                    name
                FROM conversations
                INNER JOIN meetaroo_shared.users users ON conversations.created_by = users.id
                ORDER BY created_at",
                (conversation, createdBy) => {
                    conversation.CreatedBy = createdBy;
                    return conversation;
                }
            );
        }

        public async Task<Conversation> GetConversation(long id)
        {
            // Joining on meetaroo_shared.users only works while
            // we have only one database server
            await ConnectAndSetSchema();
            var result = await connection.QueryAsync<Conversation, User, Conversation>(
                @"SELECT
                    conversations.id AS Id,
                    topic AS Topic,
                    created_at AS CreatedAt,
                    users.id AS Id,
                    name
                FROM conversations
                JOIN meetaroo_shared.users users ON conversations.created_by = users.id
                WHERE conversations.id = @id
                ORDER BY created_at",
                (conversation, createdBy) => {
                    conversation.CreatedBy = createdBy;
                    return conversation;
                },
                new { id }
            );
            return result.First();
        }

        public async Task CreateConversation(string topic, long createdBy) {
            await ConnectAndSetSchema();
            await connection.ExecuteAsync(
                @"INSERT INTO conversations (topic, created_by)
                VALUES (@topic, @createdBy)",
                new { topic, createdBy }
            );
        }

        public async Task<IEnumerable<ViewMessageDto>> GetMessages(long conversationId, long lastSeenMessage)
        {
            await ConnectAndSetSchema();
            return await connection.QueryAsync<ViewMessageDto>(
                @"SELECT
                    message.id AS MessageId,
                    replies_to AS RepliesToId,
                    message.created_at AS CreatedAt,
                    message.text AS Text,
                    author.name AS Author,
                    author.picture AS AuthorPicture,
                    event.id AS EventId
                FROM message_events AS event
                INNER JOIN messages AS message ON event.message_id = message.id
                INNER JOIN meetaroo_shared.users AS author ON message.created_by = author.id
                WHERE
                    event.id > @lastSeenMessage
                    AND message.conversation_id = @conversationId
                ORDER BY message.created_at",
                new { lastSeenMessage, conversationId }
            );
        }

        public async Task AddMessage(long conversationId, string message, long createdBy)
        {
            await ConnectAndSetSchema();
            var transaction = connection.BeginTransaction();

            try
            {
                var messageResult = await connection.QueryFirstAsync<dynamic>(
                    @"INSERT INTO
                        messages (text, conversation_id, created_by)
                        VALUES (@message, @conversationId, @createdBy)
                        RETURNING id",
                    new { message, conversationId, createdBy }
                );
                await connection.ExecuteAsync(
                    @"INSERT INTO
                        message_events(message_id, event_type, created_by)
                        VALUES (@messageId, 'message_created', @createdBy)",
                    new { messageId = messageResult.id, createdBy }
                );

                await transaction.CommitAsync();
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddReply(long conversationId, long messageId, string message, long createdBy)
        {
            await ConnectAndSetSchema();
            var transaction = connection.BeginTransaction();

            try
            {
                var messageResult = await connection.QueryFirstAsync<dynamic>(
                    @"INSERT INTO
                        messages (text, conversation_id, replies_to, created_by)
                        VALUES (@message, @conversationId, @messageId, @createdBy)
                        RETURNING id",
                    new { message, conversationId, messageId, createdBy }
                );
                await connection.ExecuteAsync(
                    @"INSERT INTO
                        message_events(message_id, event_type, created_by)
                        VALUES (@messageId, 'reply_created', @createdBy)",
                    new { messageId = messageResult.id, createdBy }
                );

                await transaction.CommitAsync();
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> IsModerator(long userId, long conversationId)
        {
            await ConnectAndSetSchema();
            var result = await connection.QueryFirstAsync<dynamic>(
                "SELECT created_by = @userId AS ismod FROM conversations WHERE id = @conversationId",
                new { userId, conversationId }
            );
            return result.ismod;
        }
    }
}