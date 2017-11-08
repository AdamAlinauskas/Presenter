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
            await connection.ExecuteAsync("INSERT INTO presentations(name, document_id, created_by) values(@Name, @DocumentId, @CreatedBy)", dto);
        }

        public async Task<PresentationDto> Get(long presentationId)
        {
            await ConnectAndSetSchema();
            var presentation = (await connection.QuerySingleAsync<PresentationDto>(
                @"SELECT presentations.id as Id, 
                        presentations.name as Name,
                        files.id as DocumentId,
                        files.file_name as DocumentName,
                        files.awsKey as awsKey               
                  FROM presentations inner join files on files.id = presentations.document_id
                  where presentations.id = @presentationId", new { presentationId }));
            connection.Close();
            return presentation;
        }

        public async Task UpdatePageNumber(long presentationId, int pageNumber)
        {
            Console.WriteLine($"Presentation ID:{presentationId}    Page Number: {pageNumber}");
            await ConnectAndSetSchema();
            await connection.ExecuteAsync("UPDATE presentations SET current_page_number = @pageNumber where id = @presenationId ", new { pageNumber, presentationId });
            connection.Close();
        }
    }
}