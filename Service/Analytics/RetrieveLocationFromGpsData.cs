using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dto;
using Newtonsoft.Json;

namespace Service
{
    public interface IRetrieveLocationFromGpsData
    {
        Task<LocationDto> Fetch(double latitude, double longitude);
    }

    public class RetrieveLocationFromGpsData : IRetrieveLocationFromGpsData
    {

        public RetrieveLocationFromGpsData()
        {
        }

        public async Task<LocationDto> Fetch(double latitude, double longitude)
        {
            using (var client = new HttpClient())
            {
                var apiKey = "AIzaSyDOjM2kYc8zFYx39A_dQT39gDDAg87rRjk";
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={apiKey}";
                var json = await client.GetStringAsync(url);
                var root = JsonConvert.DeserializeObject<RootObject>(json);

                var city = root.results.First().address_components.First(x => x.types.Contains("locality")).long_name;
                var country = root.results.First().address_components.First(x => x.types.Contains("country")).long_name;

                return new LocationDto { City = city, Country = country };
            }
        }
    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }

    public class RootObject
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }
}