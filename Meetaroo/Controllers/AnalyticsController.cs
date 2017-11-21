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
        private readonly IRetrieveLocationFromIpAddress retrieveLocationFromIpAddress;

        public AnalyticsController(ICreateUserAnalyticsSessionCommand createUserAnalyticsSessionCommand, IRetrieveIpAddress retrieveIpAddress, IRetrieveLocationFromIpAddress retrieveLocationFromIpAddress)
        {
            this.createUserAnalyticsSessionCommand = createUserAnalyticsSessionCommand;
            this.retrieveIpAddress = retrieveIpAddress;
            this.retrieveLocationFromIpAddress = retrieveLocationFromIpAddress;
        }

        [HttpPost]
        public async Task<JsonResult> TrackFor(TrackRequestDto dto)
        {
            var user = await this.GetCurrentUser();
            dto.CreatedBy = user.Id;
            dto.IpAddress = retrieveIpAddress.GetRequestIp();
           // dto.Location = retrieveLocationFromIpAddress.Fetch(dto.IpAddress);

            var analyticsId = await createUserAnalyticsSessionCommand.Execute(dto);
            
            return Json(new TrackResponseDto { AnalyticsId = analyticsId });
        }
    }

    public class LocationDto{
        public string City{get;set;}
        public string Country {get;set;}
        public Continent Contient { get; internal set; }
        public string IspOrganizationName { get; internal set; }
    }

    public interface IRetrieveLocationFromIpAddress{
        LocationDto Fetch(string ipAddress);
    }

    public class RetrieveLocationFromIpAddress : IRetrieveLocationFromIpAddress{
        private readonly IHostingEnvironment hostingEnvironment;

        public RetrieveLocationFromIpAddress(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public LocationDto Fetch(string ipAddress){
            ipAddress = "68.147.135.54";
             using (var reader = new DatabaseReader("/usr/local/share/findecks/GeoLite2-City.mmdb"))
            {
                var cityResult = reader.City(ipAddress);
                var cityName = cityResult.City.Name;
                var countryName = cityResult.Country.Name;
                var continent = cityResult.Continent;
                Console.WriteLine($"Continent: {continent} Country: {countryName} City: {cityName} ");
                return new LocationDto{Country= countryName, City = cityName, Contient = continent};
            }
        }
    }
}