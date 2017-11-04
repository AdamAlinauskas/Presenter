using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain;
using Npgsql;

namespace DataAccess
{
    public interface IUserRepository
    {
        Task<bool> DoesUserExist(string identifier);
        Task CreateUser(User userProfile);
        Task<User> GetByIdentifier(string userIdentifier);
    }

    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection connection;

        public UserRepository(NpgsqlConnection connection )
        {
            this.connection = connection;
        }

        public async Task<bool> DoesUserExist(string identifier)
        {
            var result = await connection.QueryFirstAsync<int>(
                "SELECT count(*) FROM meetaroo_shared.users WHERE identifier=@identifier",
                new {identifier}
            );
            return result > 0;     
        }

        public async Task CreateUser(User userProfile)
        {
            await connection.ExecuteAsync(
                @"INSERT INTO meetaroo_shared.users (name, email, picture, identifier)
                VALUES (@name, @email, @picture, @identifier)
                ON CONFLICT DO NOTHING",
                userProfile
            );
        }

        public async Task<User> GetByIdentifier(string userIdentifier)
        {
            return await connection.QueryFirstAsync<User>(
                @"SELECT
                    id AS Id,
                    name AS Name,
                    email AS Email,
                    picture AS Picture,
                    identifier AS Identifier
                FROM meetaroo_shared.users
                WHERE identifier = @userIdentifier",
                new { userIdentifier }
            );
        }
    }
}