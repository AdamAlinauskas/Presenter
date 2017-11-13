using System;

namespace Dto.Conversations
{
    public class ConversationDto
    {
        public long Id { get; set; }
        public string Topic { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool CurrentUserIsMod { get; set; }
        public string Schema { get; set; }
    }
}