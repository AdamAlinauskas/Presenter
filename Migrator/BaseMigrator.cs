using System;
using Dapper;
using Npgsql;

namespace Migrator
{
    abstract class BaseMigrator : IDisposable
    {
        protected DatabaseAssistant migrator;
        protected NpgsqlConnection connection;
        protected string schema;

        public BaseMigrator(Connector connector, DatabaseAssistant migrator, string schema)
        {
            this.schema = schema;
            this.migrator = migrator;
            connection = connector.Connect(schema);
            migrator.Connection = connection;
        }

        public abstract void Migrate();

        protected void EnsureSchemaExists(NpgsqlConnection connection)
        {
            connection.Execute($"CREATE SCHEMA IF NOT EXISTS {schema}");
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}