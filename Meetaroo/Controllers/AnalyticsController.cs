using System;
using System.Threading.Tasks;
using Dto;
using Meetaroo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Meetaroo.Controllers
{

    [TypeFilterAttribute(typeof(RetrieveSchemaActionFilter))]
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly ICreateUserAnalyticsSessionCommand createUserAnalyticsSessionCommand;

        public AnalyticsController(ICreateUserAnalyticsSessionCommand createUserAnalyticsSessionCommand)
        {
            this.createUserAnalyticsSessionCommand = createUserAnalyticsSessionCommand;
        }

        [HttpPost]
        public async Task<JsonResult> TrackFor(TrackRequestDto dto)
        {
            var user = await this.GetCurrentUser();
            var createdBy = user.Id;
            dto.CreatedBy = createdBy;
            var analyticsId = await createUserAnalyticsSessionCommand.Execute(dto);
            return Json(new TrackResponseDto { AnalyticsId = analyticsId });
        }
    }
}