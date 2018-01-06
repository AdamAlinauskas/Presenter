using System.Threading.Tasks;
using Dapper;
using DataAccess;
using Meetaroo.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Migrator;
using Npgsql;
using Service;

namespace Meetaroo.Controllers
{
    // TODO AP : Own file
    public class CreateOrgDto
    {
        public string SchemaName { get; set; }
        public string DisplayName { get; set; }
    }

    // TODO AP : Own file
    public class CreatePresentationDto
    {
        public string SchemaName { get; set; }
        public long DocumentId { get; set; }
        public string PresentationName { get; set; }
    }

    [Authorize]
    public class SignupController : Controller
    {
        private readonly NpgsqlConnection connection;
        private readonly ICurrentSchema currentSchema;
        private readonly ICreatePresentationCommand createPresentationCommand;
        private readonly IRetrievePresentationToViewQuery retrievePresentationToViewQuery;
        private readonly IUploadFileCommand uploadFileCommand;

        public SignupController(
            NpgsqlConnection connection,
            ICurrentSchema currentSchema,
            ICreatePresentationCommand createPresentationCommand,
            IRetrievePresentationToViewQuery retrievePresentationToViewQuery,
            IUploadFileCommand uploadFileCommand)
        {
            this.connection = connection;
            this.currentSchema = currentSchema;
            this.createPresentationCommand = createPresentationCommand;
            this.retrievePresentationToViewQuery = retrievePresentationToViewQuery;
            this.uploadFileCommand = uploadFileCommand;
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

        [HttpPost]
        public async Task<JsonResult> UploadPresentation(string schemaName)
        {
            currentSchema.Name = schemaName;
            var user = await this.GetCurrentUser();

            var file = Request.Form.Files[0];

            using (var filestream = file.OpenReadStream())
            {
                var documentId = await uploadFileCommand.Execute(filestream, file.FileName, user.Id);
                return new JsonResult(new { documentId });
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreatePresentation([FromBody] CreatePresentationDto parameters)
        {
            var user = await this.GetCurrentUser();
            currentSchema.Name = parameters.SchemaName;
            
            await createPresentationCommand.Execute(new Dto.PresentationDto {
                CreatedBy = user.Id,
                DocumentId = parameters.DocumentId,
                Name = parameters.PresentationName
            });

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