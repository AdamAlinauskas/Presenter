using System;
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
        Task Create(PresentationDto dto);
        Task<PresentationDto> Get(long presentationId);
        Task UpdatePageNumber(long presenationId, int pageNumber);
        Task<int> GetCurrentPageNumber(long presentationId);
    }

    public class PresentationRepository : BaseRepository, IPresentationRepository
    {
        private readonly ICurrentSchema currentSchema;

        public PresentationRepository(NpgsqlConnection connection, ICurrentSchema currentSchema) : base(connection, currentSchema)
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
                        files.file_name as DocumentName                
                  FROM presentations inner join files on files.id = presentations.document_id")).AsList();
            connection.Close();
            return result;
        }

        public async Task Create(PresentationDto dto)
        {
            await ConnectAndSetSchema();
            await connection.ExecuteAsync(
                @"INSERT INTO presentations (name, document_id, conversation_id, created_by)
                VALUES (@Name, @DocumentId, @ConversationId, @CreatedBy)",
                dto
            );
        }

        public async Task<PresentationDto> Get(long presentationId)
        {
            await ConnectAndSetSchema();
            var presentation = (await connection.QuerySingleAsync<PresentationDto>(
                @"SELECT
                    presentations.id AS Id, 
                    presentations.name AS Name,
                    files.id AS DocumentId,
                    files.file_name AS DocumentName,
                    files.awsKey AS awsKey,
                    presentations.conversation_id AS ConversationId
                FROM presentations INNER JOIN files ON files.id = presentations.document_id
                WHERE presentations.id = @presentationId",
                new { presentationId }
               ));
            connection.Close();
            return presentation;
        }

        public async Task<int> GetCurrentPageNumber(long presentationId)
        {
            await ConnectAndSetSchema();
            var page = (await connection.QuerySingleAsync<int>(
                @"SELECT current_page_number FROM presentations where presentations.id = @presentationId", new { presentationId }));
            connection.Close();
            return page;
        }

        public async Task UpdatePageNumber(long presentationId, int pageNumber)
        {
            Console.WriteLine($"Presentation ID:{presentationId}    Page Number: {pageNumber}");
            await ConnectAndSetSchema();
            await connection.ExecuteAsync("UPDATE presentations SET current_page_number = @pageNumber where presentations.id = @presentationId ", new { pageNumber, presentationId });
            connection.Close();
        }
    }
}