using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dto;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Model;
using Meetaroo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IRetrieveLocation retrieveLocation;
        private readonly IUpdateAnalyticsDurationCommand updateAnalyticsDurationCommand;

        public AnalyticsController(ICreateUserAnalyticsSessionCommand createUserAnalyticsSessionCommand, IRetrieveIpAddress retrieveIpAddress, IRetrieveLocation retrieveLocation, IUpdateAnalyticsDurationCommand updateAnalyticsDurationCommand)
        {
            this.createUserAnalyticsSessionCommand = createUserAnalyticsSessionCommand;
            this.retrieveIpAddress = retrieveIpAddress;
            this.retrieveLocation = retrieveLocation;
            this.updateAnalyticsDurationCommand = updateAnalyticsDurationCommand;
        }

        [HttpPost]
        public async Task<JsonResult> TrackFor(TrackRequestDto dto)
        {
            Console.WriteLine($"Lat: {dto.Latitude} long: {dto.Longitude}");
            var user = await this.GetCurrentUser();
            dto.CreatedBy = user.Id;
            dto.IpAddress = retrieveIpAddress.GetRequestIp();

            var analyticsId = await createUserAnalyticsSessionCommand.Execute(dto);

            return Json(new TrackResponseDto { AnalyticsId = analyticsId });
        }

        public async Task<JsonResult> UpdateDuration(UpdateAnalyticsDurationRequestDto dto)
        {
            Console.WriteLine($"Update Duration for session id: {dto.AnalyticsId} with duration of {dto.Duration}");
            await updateAnalyticsDurationCommand.Execute(dto);
            return Json(new {});
        }
    }
}