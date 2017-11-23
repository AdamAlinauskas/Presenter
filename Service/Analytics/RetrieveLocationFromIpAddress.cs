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
            try{
                using (var reader = new DatabaseReader("/usr/local/share/findecks/GeoLite2-City.mmdb"))
                {
                    
                    var cityResult = reader.City(ipAddress);
                    var cityName = cityResult.City.Name;
                    var countryName = cityResult.Country.Name;
                    var continent = cityResult.Continent.Name;
                    return new LocationDto { Country = countryName, City = cityName, Continent = continent };
                }
            }
            catch{
                return new LocationDto();
            }
        }
    }
}