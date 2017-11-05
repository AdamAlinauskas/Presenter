using System.Threading.Tasks;
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

        public PresentationController(IRetrievePresentationListingQuery retrievePresentationListingQuery)
        {
            this.retrievePresentationListingQuery = retrievePresentationListingQuery;
        }

        public async Task<IActionResult> Index()
        {
            var dto = await retrievePresentationListingQuery.Fetch();
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PresentationDto dto)
        {
            return View(new PresentationListingDto());
        }
    }
}