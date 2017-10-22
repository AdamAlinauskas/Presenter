using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using Dapper;

namespace Migrator
{
    class DatabaseAssistant
    {
        // TODO AP : All of this shit should be async for use with asp.net

        public DbConnection Connection { get; set; }
        const string MIGRATIONS_EXIST_QUERY = "SELECT 1 FROM information_schema.tables WHERE table_schema = @schema_name AND table_name = 'versions'";
        const string CREATE_MIGRATION_TABLE_COMMAND = "CREATE TABLE IF NOT EXISTS {0}.versions (version TEXT NOT NULL UNIQUE)";
        private string migrationFolder;

        public void Migrate(string schemaName, string migrationSet)
        {
            EnsureMigrationsTableExists(schemaName);
            migrationFolder = $"/srv/migrations/{migrationSet}/";

            // List migrations, check against versions table, load and run required ones
            var allMigrations = GetMigrationFileNames();
            var runMigrations = GetAlreadyRunMigrations();
            var migrationsToRun = FindMigrationsToRun(allMigrations, runMigrations);

            foreach (var migration in migrationsToRun)
                Run(migration);
        }

        private IEnumerable<string> FindMigrationsToRun(IEnumerable<string> allMigrations, IEnumerable<string> runMigrations)
        {
            // Dumb, O(N^2) comparison. Could take advantage of ordered list and do it in O(N)
            // To begin with, perf isn't going to kill us here, though
            var runList = runMigrations.ToList(); // Really don't want to evaluate this multiple times
            return allMigrations.Where(version => !runList.Contains(version));
        }

        private void EnsureMigrationsTableExists(string schemaName) => Connection.Execute(
            string.Format(CREATE_MIGRATION_TABLE_COMMAND, schemaName)
        );

        private IEnumerable<string> GetMigrationFileNames() => Directory
            .EnumerateFiles(migrationFolder, "*.sql", SearchOption.TopDirectoryOnly)
            .Select(filename => Path.GetFileNameWithoutExtension(filename))
            .OrderBy(filename => filename);

        private IEnumerable<string> GetAlreadyRunMigrations() => Connection
            .Query<dynamic>("SELECT version FROM versions ORDER BY version")
            .Select(row => row.version as string);

        private void Run(string migration)
        {
            Console.WriteLine($"Running {migration}");
            var path = $"{migrationFolder}{migration}.sql";
            var command = File.ReadAllText(path);
            Connection.Execute(command);
            Connection.Execute($"INSERT INTO versions VALUES ('{migration}')");
            Console.WriteLine(" ...success");
        }
    }
}
