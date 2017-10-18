using Npgsql;

namespace Migrator
{
    class DatabaseMigrator
    {
        // TODO AP : All of this shit should be async for use with asp.net

        public NpgsqlConnection Connection { get; set; }
        const string MIGRATIONS_EXIST_QUERY = "SELECT 1 FROM information_schema.tables WHERE table_schema = @schema_name AND table_name = 'migrations'";
        const string CREATE_MIGRATION_TABLE_COMMAND = "CREATE TABLE migrations (version TEXT NOT NULL UNIQUE)";

        public void Migrate(string schemaName, string migrationSet)
        {
            using (var cmd = new NpgsqlCommand(MIGRATIONS_EXIST_QUERY, Connection))
            {
                cmd.Parameters.AddWithValue("schema_name", schemaName);
                using (var result = cmd.ExecuteReader())
                {
                    if (!result.HasRows)
                        CreateMigrationTable();
                }

                // List migrations, check against versions table, load and run required ones
            }
        }

        private void CreateMigrationTable()
        {
            using (var cmd = new NpgsqlCommand(CREATE_MIGRATION_TABLE_COMMAND))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
