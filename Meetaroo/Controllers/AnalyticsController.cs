using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dto;
using Meetaroo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Service;

namespace Meetaroo.Controllers
{

    [TypeFilterAttribute(typeof(RetrieveSchemaActionFilter))]
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly ICreateUserAnalyticsSessionCommand createUserAnalyticsSessionCommand;
        private readonly IRetrieveIpAddress retrieveIpAddress;

        public AnalyticsController(ICreateUserAnalyticsSessionCommand createUserAnalyticsSessionCommand, IRetrieveIpAddress retrieveIpAddress)
        {
            this.createUserAnalyticsSessionCommand = createUserAnalyticsSessionCommand;
            this.retrieveIpAddress = retrieveIpAddress;
        }

        [HttpPost]
        public async Task<JsonResult> TrackFor(TrackRequestDto dto)
        {
            var user = await this.GetCurrentUser();
            dto.CreatedBy = user.Id;
            dto.IpAddress = retrieveIpAddress.GetRequestIp();
            var analyticsId = await createUserAnalyticsSessionCommand.Execute(dto);
            
            return Json(new TrackResponseDto { AnalyticsId = analyticsId });
        }
    }
}