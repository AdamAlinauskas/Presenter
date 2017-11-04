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
using Amazon.S3.Model;
using Service;
using DataAccess;

namespace Meetaroo.Controllers
{

    [TypeFilterAttribute(typeof(RetrieveSchemaActionFilter))]
    [Authorize]
    public class DocumentController : Controller{
        private readonly IUploadFileCommand uploadFileCommand;
        private readonly ICurrentSchema currentSchema;
        private readonly IRetrieveDocumentsQuery retrieveDocumentsQuery;
        private readonly IRetrieveDocumentUrlQuery retrieveDocumentUrlQuery;

        public DocumentController(IUploadFileCommand uploadFileCommand, ICurrentSchema currentSchema, IRetrieveDocumentsQuery retrieveDocumentsQuery, IRetrieveDocumentUrlQuery retrieveDocumentUrlQuery)
        {
            this.uploadFileCommand = uploadFileCommand;
            this.currentSchema = currentSchema;
            this.retrieveDocumentsQuery = retrieveDocumentsQuery;
            this.retrieveDocumentUrlQuery = retrieveDocumentUrlQuery;
        }

        public async Task<IActionResult> Index(){
            var dto = await retrieveDocumentsQuery.Fetch();
            return View(dto);
        }

        [HttpPost]
        public  async Task<IActionResult> Index(IFormFile file){    
            var fileStream = new MemoryStream();
            await file.CopyToAsync(fileStream);
            await uploadFileCommand.Execute(fileStream, file.FileName);

            return RedirectToRoute(
                "schemaBased",
                new { schema = currentSchema.Name, controller = "Document", action = "index" }
            ); 
        }

        public async Task<IActionResult> View(long id){
            var dto = await retrieveDocumentUrlQuery.FetchAsync(id);
            return View(dto);
        }
    }
}
   
