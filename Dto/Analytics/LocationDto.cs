namespace Dto
{
    public class LocationDto
    {
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Continent { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}