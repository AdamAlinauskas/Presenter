using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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

        public IActionResult HealthCheck()
        {
            StringValues values;
            
            
            var proto = Request.Headers.TryGetValue("Proto", out values) ? values[0] : "none";
            var forwardedProto = Request.Headers.TryGetValue("X-Forwarded-Proto", out values) ? values[0] : "none";

            return new ContentResult {
                Content = $"Health check okay\nProtocol: ${proto}\nForwarded protocol: ${proto}",
                ContentType = "text",
                StatusCode = 200
            };
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