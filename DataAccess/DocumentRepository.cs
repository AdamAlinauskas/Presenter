using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dto;
using Npgsql;

namespace DataAccess
{
    public interface IDocumentRepository
    {
        Task<long> Save(string fileName, string awsKey, long userId);
        Task<IList<DocumentDto>> All();
        Task<string> GetAwsKeyFor(long id);
    }
    public class DocumentRepository : BaseRepository, IDocumentRepository
    {
        private readonly ICurrentSchema currentSchema;

        public DocumentRepository(NpgsqlConnection connection, ICurrentSchema currentSchema) : base(connection, currentSchema)
        {
            this.currentSchema = currentSchema;
        }

        public async Task<long> Save(string fileName, string awsKey, long userId)
        {
            await ConnectAndSetSchema();
            var result = await connection.QuerySingleAsync<dynamic>(
                "INSERT INTO files (file_name, awsKey, created_by) VALUES (@fileName, @awsKey, @userId) RETURNING id",
                new { awsKey, fileName, userId }
            );
            return result.id;
        }

        public async Task<IList<DocumentDto>> All()
        {
            await ConnectAndSetSchema();
            var result = (await connection.QueryAsync<DocumentDto>("SELECT Id, file_name as FileName FROM FILES")).AsList();
            connection.Close();
            return result;
        }

        public async Task<string> GetAwsKeyFor(long id)
        {
            await ConnectAndSetSchema();
            var awsKey = (await connection.QuerySingleAsync<string>("SELECT awsKey FROM files WHERE ID =@id", new { id }));
            return awsKey;
        }
    }
}