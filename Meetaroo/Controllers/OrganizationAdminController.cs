using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using System;
using Migrator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;
using System.IO;

namespace Meetaroo.Controllers
{
    public class OrganizationAdminController : Controller
    {
        NpgsqlConnection connection;

        public OrganizationAdminController(NpgsqlConnection connection)
        {
            this.connection = connection;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            await connection.OpenAsync();

            var result = await connection.QueryAsync("SELECT id, display_name FROM Organizations");
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string schemaName, string displayName)
        {
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

        public  IActionResult UploadPresentation(){
            
            return View();
        }

        [HttpPost]
        public  async Task<IActionResult> UploadPresentation(IFormFile file){    
            var client = new AmazonS3Client("AKIAJNOS24TJ3PWZHKEQ", "+d+qIQ5Uv8dfFTdsdvBd0Hp0Exm5QY2YH1ZL8903", RegionEndpoint.USWest2);
            var transfer = new Amazon.S3.Transfer.TransferUtility(client);
            var request  = new TransferUtilityUploadRequest();
            request.BucketName = "sakjfkls-test-bucket";
            request.InputStream = new MemoryStream();
            request.Key = Guid.NewGuid().ToString();
            await file.CopyToAsync(request.InputStream);

            request.CannedACL = S3CannedACL.AuthenticatedRead;

            await transfer.UploadAsync(request);
            
            return RedirectToAction("UploadPresentation","OrganizationAdmin");
        }
    }
}