using System;

namespace Domain
{
    public class Message
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public Conversation Conversation { get; set; }
        public DateTime CreatedAt { get; set; }
        public User CreatedBy { get; set; }
    }
}