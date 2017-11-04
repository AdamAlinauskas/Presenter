namespace Dto
{
    public class ViewMessageDto
    {
        public long MessageId { get; set; }
        public string CreatedAt { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public long EventId { get; set; }
    }
}