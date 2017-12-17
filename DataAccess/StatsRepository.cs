using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dto.Stats;
using Npgsql;

namespace DataAccess
{
    public interface IStatsRepository
    {
        Task<ViewsOverTime> ViewsPerDay();
        Task<GeographicViews> GeographicViews();
        Task<RealtimeGeoViews> GeoViewsSince(long lastId);
    }

    public class StatsRepository : BaseRepository, IStatsRepository
    {
        public StatsRepository(NpgsqlConnection connection, ICurrentSchema currentSchema) : base(connection, currentSchema)
        {
        }

        public async Task<ViewsOverTime> ViewsPerDay() {
            await ConnectAndSetSchema();

            var samples = await connection.QueryAsync<DayViewSample>(@"
                SELECT
                    created_at::date AS date,
                    count(*) AS views
                FROM user_analytics_sessions
                GROUP BY date
                ORDER BY date"
            );

            return new ViewsOverTime{
                ViewsPerDay = samples
            };
        }

        public async Task<GeographicViews> GeographicViews()
        {
            await ConnectAndSetSchema();

            var samples = await connection.QueryAsync<GeographicViewSample>(@"
                WITH bounds AS (
                    SELECT
                        max(latitude) AS maxLatitude,
                        min(latitude) AS minLatitude,
                        max(longitude) AS maxLongitude,
                        min(longitude) AS minLongitude
                    FROM user_analytics_sessions
                )
                SELECT
                    width_bucket(latitude, bounds.minLatitude, bounds.maxLatitude, 10) AS latBin,
                    width_bucket(longitude, bounds.minLongitude, bounds.maxLongitude, 10) AS longBin,
                    avg(latitude) AS centroidLat,
                    avg(longitude) AS centroidLong,
                    count(*) AS views
                FROM user_analytics_sessions, bounds
                WHERE latitude IS NOT NULL AND longitude IS NOT NULL
                GROUP BY latBin, longBin
            ");

            return new GeographicViews
            {
                Samples = samples
            };
        }

        public async Task<RealtimeGeoViews> GeoViewsSince(long lastId)
        {
            await ConnectAndSetSchema();

            var samples = await connection.QueryAsync<GeoViewSample>(@"
                SELECT
                    session.id AS id,
                    session.latitude AS lat,
                    session.longitude AS long,
                    viewer.name AS name
                FROM user_analytics_sessions session
                    INNER JOIN meetaroo_shared.users viewer ON session.created_by = viewer.id
                WHERE
                    session.latitude IS NOT NULL
                    AND session.longitude IS NOT NULL
                    AND session.id > @lastId
                    AND session.created_at > now() - interval '1 minute'
            ",
            new { lastId });

            return new RealtimeGeoViews
            {
                Samples = samples
            };
        }
    }
}