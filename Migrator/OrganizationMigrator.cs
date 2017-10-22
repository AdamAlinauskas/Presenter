using Npgsql;

namespace Migrator
{
    class OrganizationMigrator : BaseMigrator
    {
        public OrganizationMigrator(Connector connector, DatabaseAssistant migrator, string organization) : base(connector, migrator, organization)
        {
        }

        public override void Migrate()
        {
            EnsureSchemaExists(connection);
            migrator.Migrate(schema, "organization");
        }
    }
}