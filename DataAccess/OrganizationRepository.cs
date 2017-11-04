using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace DataAccess{
    public interface IOrganizationRepository
    {
        Task<bool> DoesSchemaExist(string schemaName);
    }

    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly NpgsqlConnection connection;

        public OrganizationRepository(NpgsqlConnection connection )
        {
            this.connection = connection;
        }

        public async Task<bool> DoesSchemaExist(string schemaName)
        {
            await connection.OpenAsync();
            var result = await connection.QueryFirstAsync<int>("SELECT count(*) FROM Organizations where schema_name=@schemaName",
            new {schemaName});
            connection.Close();
            
            return result > 0;     
        }
    }
}