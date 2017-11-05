using System.Threading.Tasks;
using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meetaroo.Controllers
{

    [TypeFilterAttribute(typeof(RetrieveSchemaActionFilter))]
    [Authorize]
    public class PresentationController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(new PresentationListingDto());
        }
    }
}