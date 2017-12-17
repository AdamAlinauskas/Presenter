using System;
using Dto;
using MaxMind.GeoIP2;

namespace Service{

    public interface IRetrieveLocationFromIpAddress
    {
        LocationDto Fetch(string ipAddress);
    }

    public class RetrieveLocationFromIpAddress : IRetrieveLocationFromIpAddress
    {
        public RetrieveLocationFromIpAddress()
        {
        
        }

        public LocationDto Fetch(string ipAddress)
        {
            try
            {
                using (var reader = new DatabaseReader("/usr/local/share/findecks/GeoLite2-City.mmdb"))
                {
                    var cityResult = reader.City(ipAddress);

                    return new LocationDto {
                        City = cityResult.City.Name,
                        Country = cityResult.Country.Name,
                        Continent = cityResult.Continent.Name,
                        Latitude = cityResult.Location.Latitude,
                        Longitude = cityResult.Location.Longitude
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Could not fetch address from IP\n${ex.Message}");
                return new LocationDto();
            }
        }
    }
}