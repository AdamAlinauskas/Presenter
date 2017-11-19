using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dto.Organizations;
using Npgsql;

namespace DataAccess{
    public interface IOrganizationRepository
    {
        Task<bool> DoesSchemaExist(string schemaName);
        Task<IEnumerable<OrganizationSummaryDto>> FetchAll();
        OrganizationSummaryDto Fetch(string schemaName);
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

        public OrganizationSummaryDto Fetch(string schemaName)
        {
            connection.Open();
            return connection.QueryFirst<OrganizationSummaryDto>(
                "SELECT display_name AS name, schema_name AS schema FROM organizations WHERE schema_name = @schemaName",
                new { schemaName }
            );
        }

        public async Task<IEnumerable<OrganizationSummaryDto>> FetchAll()
        {
            await connection.OpenAsync();
            return await connection.QueryAsync<OrganizationSummaryDto>("SELECT display_name AS name, schema_name AS schema FROM organizations");
        }
    }
}