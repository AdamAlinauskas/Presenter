using Dto;

namespace Service
{
    public interface IRetrieveLocation
    {
        LocationDto Fetch(TrackRequestDto dto);
    }

    public class RetrieveLocation : IRetrieveLocation
    {
        private readonly IRetrieveLocationFromIpAddress retrievelocationFromIpAddress;

        public RetrieveLocation(IRetrieveLocationFromIpAddress retrievelocationFromIpAddress)
        {
            this.retrievelocationFromIpAddress = retrievelocationFromIpAddress;
        }

        public LocationDto Fetch(TrackRequestDto dto)
        {
            return retrievelocationFromIpAddress.Fetch(dto.IpAddress);
        }
    }
}