using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Domain;
using Npgsql;

namespace DataAccess
{
    public interface IConversationRepository
    {
        Task<Conversation> GetConversation(long id);
        Task<IEnumerable<Conversation>> GetConversations();
        Task<IEnumerable<Message>> GetMessages(long conversationId, long lastSeenMessage);
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

        public async Task<IEnumerable<Message>> GetMessages(long conversationId, long lastSeenMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}