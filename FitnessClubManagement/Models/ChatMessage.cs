using System;

namespace FitnessClubManagement.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        
        public int SenderId { get; set; }
        public User? Sender { get; set; }
        
        public int ReceiverId { get; set; }
        public User? Receiver { get; set; }

        public bool IsRead { get; set; } = false;

        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}