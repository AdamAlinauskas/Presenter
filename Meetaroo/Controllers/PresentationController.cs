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

        public PresentationController(IRetrievePresentationListingQuery retrievePresentationListingQuery, ICurrentSchema currentSchema, ICreatePresentationCommand createPresentationCommand)
        {
            this.retrievePresentationListingQuery = retrievePresentationListingQuery;
            this.currentSchema = currentSchema;
            this.createPresentationCommand = createPresentationCommand;
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

        public IActionResult ViewPresentation(){
            return View();
        }
    }
}