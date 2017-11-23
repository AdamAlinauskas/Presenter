using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dto;
using Npgsql;

namespace DataAccess
{
    public interface IUserAnalyticsSessionRepository
    {
        Task<long> CreateForEitherDocumentOrPresentation(TrackRequestDto dto);
    }

    public class UserAnalyticsSessionRepository : BaseRepository, IUserAnalyticsSessionRepository
    {
        private readonly ICurrentSchema currentSchema;

        public UserAnalyticsSessionRepository(NpgsqlConnection connection, ICurrentSchema currentSchema) : base(connection, currentSchema)
        {
            this.currentSchema = currentSchema;
        }

        public async Task<long> CreateForEitherDocumentOrPresentation(TrackRequestDto dto)
        {
            await ConnectAndSetSchema();
            var id = await connection.QuerySingleAsync<long>(
                @"INSERT INTO user_analytics_sessions (presentation_id, document_id, created_by, ip_address, continent, country, city)
                VALUES (@PresentationId, @DocumentId, @CreatedBy, @IpAddress, @Continent, @Country, @City) RETURNING id;",
                new
                {
                    dto.PresentationId,
                    dto.DocumentId,
                    dto.CreatedBy,
                    dto.IpAddress,
                    dto.Location.Continent,
                    dto.Location.Country,
                    dto.Location.City
                }
            );
            connection.Close();
            return id;
        }
    }
}