using System.Collections.Generic;
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
            await Connect();
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
            await Connect();
            return await connection.QueryFirstAsync<Conversation>(
                @"SELECT
                    conversations.id AS Id,
                    topic AS Topic,
                    created_at AS CreatedAt
                FROM conversations
                WHERE conversations.id = @id
                ORDER BY created_at",
                new { id }
            );
        }

        public async Task CreateConversation(string topic, long createdBy) {
            await Connect();
            await connection.ExecuteAsync(
                @"INSERT INTO conversations (topic, created_by)
                VALUES (@topic, @createdBy)",
                new { topic, createdBy }
            );
        }

        public async Task<IEnumerable<ViewMessageDto>> GetMessages(long conversationId, long lastSeenMessage)
        {
            await Connect();
            return await connection.QueryAsync<ViewMessageDto>(
                @"SELECT
                    message.id AS MessageId,
                    message.created_at AS CreatedAt,
                    message.text AS Text,
                    author.name AS Author,
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
            await Connect();
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
    }
}