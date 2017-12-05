using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dto.Stats;
using Npgsql;

namespace DataAccess
{
    public interface IStatsRepository
    {
        Task<ViewsPerDay> ViewsPerDay();
    }

    public class StatsRepository : BaseRepository, IStatsRepository
    {
        public StatsRepository(NpgsqlConnection connection, ICurrentSchema currentSchema) : base(connection, currentSchema)
        {
        }

        public async Task<ViewsPerDay> ViewsPerDay() {
            await ConnectAndSetSchema();

            var samples = await connection.QueryAsync<DayViewSample>(@"
                SELECT
                    created_at::date AS date,
                    count(*) AS views
                FROM user_analytics_sessions
                GROUP BY date
                ORDER BY date;"
            );

            return new ViewsPerDay{
                Samples = samples
            };
        }
    }
}