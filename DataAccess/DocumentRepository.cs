using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dto;
using Npgsql;

namespace DataAccess{
    
    public interface IDocumentRepository{
        Task Save(string fileName, string awsKey);
        Task<IList<DocumentDto>> All();
    }
    public class DocumentRepository : BaseRepository, IDocumentRepository{
        
        private readonly ICurrentSchema currentSchema;

        public DocumentRepository(NpgsqlConnection connection, ICurrentSchema currentSchema) : base(connection, currentSchema)
        {
            
            this.currentSchema = currentSchema;
        }

        public async Task Save(string fileName, string awsKey){
            int createdBy = 0;
            await ConnectAndSetSchema();
            
            await connection.ExecuteAsync(
                "INSERT INTO files (file_name, awsKey, created_by) VALUES (@fileName, @awsKey,@createdBy)",
                new { awsKey, fileName,createdBy }
            );
      
            connection.Close();
        }

        public async Task<IList<DocumentDto>> All(){ 
            await ConnectAndSetSchema();
            var result = (await connection.QueryAsync<DocumentDto>("SELECT Id, file_name as FileName FROM FILES")).AsList();
            connection.Close();
            return result;
        }
    }
}