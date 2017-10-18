using Npgsql;

namespace Migrator
{
    class SharedSchemaMigrator
    {
        private Connector connector;
        private DatabaseMigrator migrator;
        private NpgsqlConnection connection;

        public SharedSchemaMigrator(Connector connector, DatabaseMigrator migrator)
        {
            this.connector = connector;
            this.migrator = migrator;
        }

        public void Migrate()
        {
            using (connection = connector.Connect("meetaroo_shared"))
            {
                migrator.Connection = connection;
                migrator.Migrate("meetaroo_shared", "shared");
            }
        }
    }
}
