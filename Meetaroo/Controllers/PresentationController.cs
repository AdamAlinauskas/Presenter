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
        private readonly IRetrievePresentationListingQuery retrievePresentationListingQuery;
        private readonly ICurrentSchema currentSchema;
        private readonly ICreatePresentationCommand createPresentationCommand;
        private readonly IRetrievePresentationToViewQuery retrievePresentationToViewQuery;

        public PresentationController(IRetrievePresentationListingQuery retrievePresentationListingQuery, ICurrentSchema currentSchema, ICreatePresentationCommand createPresentationCommand, IRetrievePresentationToViewQuery retrievePresentationToViewQuery)
        {
            this.retrievePresentationListingQuery = retrievePresentationListingQuery;
            this.currentSchema = currentSchema;
            this.createPresentationCommand = createPresentationCommand;
            this.retrievePresentationToViewQuery = retrievePresentationToViewQuery;
        }

        public async Task<IActionResult> Index()
        {
            var dto = await retrievePresentationListingQuery.Fetch();
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PresentationDto dto)
        {
            var user = await this.GetCurrentUser();
            dto.CreatedBy = user.Id;
            await createPresentationCommand.Execute(dto);
            return RedirectToRoute(
                "schemaBased",
                new { schema = currentSchema.Name, controller = "Presentation", action = "index" }
            );
        }

        public async Task<IActionResult> ViewPresentation(long Id)
        {
            var dto = await retrievePresentationToViewQuery.Fetch(Id);
            return View(dto);
        }

        public async Task<IActionResult> PerformPresentation(long Id)
        {
             var dto = await retrievePresentationToViewQuery.Fetch(Id);
            return View(dto);
        }
    }
}