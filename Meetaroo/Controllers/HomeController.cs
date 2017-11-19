using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Service;

namespace Meetaroo.Controllers
{
    [TypeFilter(typeof(RetrieveSchemaActionFilter))]
    public class HomeController : Controller
    {
        private readonly ICurrentSchema schema;
        private readonly IRetrieveOrganizationsQuery organizations;
        private readonly IRetrievePresentationListingQuery presentations;

        public HomeController(
            ICurrentSchema schema,
            IRetrieveOrganizationsQuery organizations,
            IRetrievePresentationListingQuery presentations
        ) {
            this.schema = schema;
            this.organizations = organizations;
            this.presentations = presentations;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated) {
                return schema.HasSchema
                    ? await RenderOrganizationHome()
                    : await RenderAdminHome();
            }

            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> RenderAdminHome()
        {
            var dto = await organizations.Fetch();
            return View("AdminHome", dto);
        }

        public async Task<IActionResult> RenderOrganizationHome()
        {
            var dto = await presentations.Fetch();
            return View("OrganizationHome", dto);
        }
    }
}