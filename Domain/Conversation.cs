using System;

namespace Domain
{
    public class Conversation
    {
        public long Id { get; set; }
        public string Topic { get; set; }
        public DateTime CreatedAt { get; set; }
        public User CreatedBy { get; set; }
    }
}