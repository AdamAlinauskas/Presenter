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
    }
}