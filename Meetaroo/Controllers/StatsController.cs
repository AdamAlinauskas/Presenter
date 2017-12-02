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

        public StatsController(
            ICurrentSchema currentSchema
         ) {
            this.currentSchema = currentSchema;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}