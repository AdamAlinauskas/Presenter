namespace Dto
{
    public class PresentationDto
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public string DocumentName { get; set; }
        public long DocumentId { get; set; }
        public long CreatedBy {get;set;}
        public string awsKey {get;set;}
        public long ConversationId { get; set; }
    }
}