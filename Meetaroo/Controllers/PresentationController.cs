using System.Threading.Tasks;
using DataAccess;
using Dto;
using Meetaroo.Services;
using Microsoft.AspNetCore.Authorization;
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

        public PresentationController(ICurrentSchema currentSchema, ICreatePresentationCommand createPresentationCommand, IRetrievePresentationToViewQuery retrievePresentationToViewQuery)
        {
            this.currentSchema = currentSchema;
            this.createPresentationCommand = createPresentationCommand;
            this.retrievePresentationToViewQuery = retrievePresentationToViewQuery;
        }

        [HttpPost]
        public async Task<IActionResult> Index(PresentationDto dto)
        {
            var user = await this.GetCurrentUser();
            dto.CreatedBy = user.Id;
            await createPresentationCommand.Execute(dto);
            return RedirectToAction("Index", "Presentation");
        }
    }
}