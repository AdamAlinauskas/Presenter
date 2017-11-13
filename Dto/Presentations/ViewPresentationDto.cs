namespace Dto
{
    public class ViewPresentationDto
    {
        public string Url { get; set; }
        public string Schema { get; set; } = "";
        public string PresentationName { get; set; }
        public long PresentationId { get; set; }
        public string ViewPresentationKey => Schema + PresentationId;
        public bool IsPresenter { get; set; }
        public long ConversationId { get; set; }
    }
}