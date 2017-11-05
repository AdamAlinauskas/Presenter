using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dto;
using Npgsql;

namespace DataAccess
{
    public interface IPresentationRepository
    {
        Task<IList<PresentationDto>> All();
    }
    public class PresentationRepository : BaseRepository, IPresentationRepository
    {
        private readonly ICurrentSchema currentSchema;

        public PresentationRepository(NpgsqlConnection connection, ICurrentSchema currentSchema): base(connection, currentSchema)
        {
            this.currentSchema = currentSchema;
        }

        public async Task<IList<PresentationDto>> All()
        {
            await ConnectAndSetSchema();
            var result = (await connection.QueryAsync<PresentationDto>(
                @"SELECT presentations.id as Id, 
                        presentations.name as Name,
                        files.id as DocumentId,
                        files.file_name                
                  FROM Presentations inner join files on files.id = presentations.document_id")).AsList();
            connection.Close();
            return result;
        }
    }
}