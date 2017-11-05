using System.Threading.Tasks;
using DataAccess;
using Dto;
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

        public PresentationController(IRetrievePresentationListingQuery retrievePresentationListingQuery, ICurrentSchema currentSchema)
        {
            this.retrievePresentationListingQuery = retrievePresentationListingQuery;
            this.currentSchema = currentSchema;
        }

        public async Task<IActionResult> Index()
        {
            var dto = await retrievePresentationListingQuery.Fetch();
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PresentationDto dto)
        {
            return RedirectToRoute(
                "schemaBased",
                new { schema = currentSchema.Name, controller = "Presentation", action = "index" }
            );
        }
    }
}