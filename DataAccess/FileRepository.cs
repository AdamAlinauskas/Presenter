using System;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace DataAccess{
    
    public interface IFileRepository{
        Task Save(string fileName, string awsKey);
    }
    public class FileRepository : BaseRepository, IFileRepository{
        private readonly NpgsqlConnection connection;
        private readonly ICurrentSchema currentSchema;

        public FileRepository(NpgsqlConnection connection, ICurrentSchema currentSchema) : base(connection, currentSchema)
        {
            this.connection = connection;
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

    }
}