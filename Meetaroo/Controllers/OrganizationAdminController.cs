using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Threading.Tasks;
using Dapper;
using Migrator;
using Microsoft.AspNetCore.Authorization;
using DataAccess;

namespace Meetaroo.Controllers
{
    [Authorize]
    [TypeFilter(typeof(RetrieveSchemaActionFilter))]
    public class OrganizationAdminController : Controller
    {
        NpgsqlConnection connection;
        ICurrentSchema currentSchema;

        public OrganizationAdminController(NpgsqlConnection connection, ICurrentSchema currentSchema)
        {
            this.connection = connection;
            this.currentSchema = currentSchema;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string schemaName, string displayName)
        {
            // TODO : This should use OrganizationRepository
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                "INSERT INTO meetaroo_shared.organizations (schema_name, display_name) VALUES (@schemaName, @displayName)",
                new { schemaName, displayName }
            );
            MigrateOrg(schemaName);

            return RedirectToAction("Index");
        }

        private void MigrateOrg(string schemaName)
        {
            var connector = new Connector();
            var dbAssistant = new DatabaseAssistant();
            var migrator = new OrganizationMigrator(connector, dbAssistant, schemaName);
            migrator.Migrate();
        }
    }
       
}