using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Npgsql;

namespace Migrator
{
    class SharedSchemaMigrator : BaseMigrator
    {
        public SharedSchemaMigrator(Connector connector, DatabaseAssistant migrator) : base(connector, migrator, "meetaroo_shared")
        {
        }

        public override void Migrate()
        {
            EnsureSchemaExists(connection);
            migrator.Migrate(schema, "shared");
        }

        internal IEnumerable<string> GetOrgs()
        {
            var result = connection.Query("SELECT schema_name AS schema FROM organizations");
            return result.Select(row => row.schema as string);
        }
    }
}
