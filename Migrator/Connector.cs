using Npgsql;
using System;

namespace Migrator
{
    class Connector
    {
        public NpgsqlConnection Connect(string schemaName)
        {
            var dbHost = Environment.GetEnvironmentVariable("DATABASE_HOST");
            var connectionString = string.Format(
                "Server={0};Database=meetaroo;Username=meetaroo;Password=x1Y6Dfb4ElF7C6JbEo170raDSaQRcb71;Search Path={1}",
                dbHost,
                schemaName
            );

            return new NpgsqlConnection(connectionString);
        }
    }
}
