using System.Threading.Tasks;
using Dto;

namespace Service
{
    public interface IRetrieveLocation
    {
        Task<LocationDto> Fetch(TrackRequestDto dto);
    }

    public class RetrieveLocation : IRetrieveLocation
    {
        private readonly IRetrieveLocationFromIpAddress retrievelocationFromIpAddress;
        private readonly IRetrieveLocationFromGpsData retrieveLocationFromGpsData;

        public RetrieveLocation(IRetrieveLocationFromIpAddress retrievelocationFromIpAddress, IRetrieveLocationFromGpsData retrieveLocationFromGpsData)
        {
            this.retrievelocationFromIpAddress = retrievelocationFromIpAddress;
            this.retrieveLocationFromGpsData = retrieveLocationFromGpsData;
        }

        public async Task<LocationDto> Fetch(TrackRequestDto dto)
        {
            var locationFromIp = retrievelocationFromIpAddress.Fetch(dto.IpAddress);

            if(dto.Latitude != null && dto.Longitude != null){
                var locationFromGPS = await retrieveLocationFromGpsData.Fetch(50.919515,-113.932079d);
                locationFromIp.Country = locationFromGPS.Country;
                locationFromIp.City = locationFromGPS.City;
            }
            return locationFromIp;
        }
    }
}