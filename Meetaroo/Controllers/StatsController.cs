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
    public class StatsController : Controller
    {
        private readonly ICurrentSchema currentSchema;
        private readonly IStatsRepository stats;

        public StatsController(
            ICurrentSchema currentSchema,
            IStatsRepository statsRepository
         ) {
            this.currentSchema = currentSchema;
            this.stats = statsRepository;
        }

        public async Task<IActionResult> Index()
        {
            var model = await stats.ViewsPerDay();

            return View(model);
        }
    }
}