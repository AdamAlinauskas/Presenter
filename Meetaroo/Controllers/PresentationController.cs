using System;
using System.IO;
using System.Threading.Tasks;
using DataAccess;
using Dto;
using Meetaroo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Meetaroo.Controllers
{
    [TypeFilterAttribute(typeof(RetrieveSchemaActionFilter))]
    [Authorize]
    public class PresentationController : Controller
    {
        private readonly ICurrentSchema currentSchema;
        private readonly ICreatePresentationCommand createPresentationCommand;
        private readonly IRetrievePresentationToViewQuery retrievePresentationToViewQuery;
        private readonly IUploadFileCommand uploadFileCommand;

        public PresentationController(
            ICurrentSchema currentSchema,
             ICreatePresentationCommand createPresentationCommand,
              IRetrievePresentationToViewQuery retrievePresentationToViewQuery,
              IUploadFileCommand uploadFileCommand
         ) {
            this.currentSchema = currentSchema;
            this.createPresentationCommand = createPresentationCommand;
            this.retrievePresentationToViewQuery = retrievePresentationToViewQuery;
            this.uploadFileCommand = uploadFileCommand;
        }

        // TODO : This whole action should happen in a transaction
        [HttpPost]
        public async Task<IActionResult> Index(PresentationDto dto, IFormFile file)
        {
            var user = await this.GetCurrentUser();
            dto.CreatedBy = user.Id;

            var fileStream = new MemoryStream();
            await file.CopyToAsync(fileStream);
            var documentId = await uploadFileCommand.Execute(fileStream, file.FileName, user.Id);
            dto.DocumentId = documentId;
            Console.WriteLine($"Created document ${dto.DocumentId}");
            
            await createPresentationCommand.Execute(dto);

            return RedirectToAction("Index", "Home");
        }
    }
}