using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Migrator;
using Npgsql;

namespace Meetaroo.Controllers
{
    // TODO AP : Own file
    public class CreateOrgDto
    {
        public string SchemaName { get; set; }
        public string DisplayName { get; set; }
    }

    [Authorize]
    public class SignupController : Controller
    {
        NpgsqlConnection connection;

        public SignupController(NpgsqlConnection connection)
        {
            this.connection = connection;
        }
        
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateOrg([FromBody] CreateOrgDto parameters)
        {
            // TODO AP : Dedupe with OrganizationAdminController
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                "INSERT INTO meetaroo_shared.organizations (schema_name, display_name) VALUES (@schemaName, @displayName)",
                new { parameters.SchemaName, parameters.DisplayName }
            );
            MigrateOrg(parameters.SchemaName);

            return new JsonResult(new object());
        }

        private void MigrateOrg(string schemaName)
        {
            // TODO AP : Dedupe with OrganizationAdminController
            var connector = new Connector();
            var dbAssistant = new DatabaseAssistant();
            var migrator = new OrganizationMigrator(connector, dbAssistant, schemaName);
            migrator.Migrate();
        }
    }
}