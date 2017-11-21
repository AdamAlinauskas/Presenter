namespace Dto
{
    public class TrackRequestDto
    {
        public long? PresentationId { get; set; }
        public long? DocumentId { get; set; }
        public long CreatedBy { get; set; }
        public string IpAddress { get; set; }
        //public LocationDto Location { get; set; }
    }
}