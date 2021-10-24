using System;

namespace API.Entities
{
    public class Message
    {
        
        //Sender
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }

        //Recipient
        public int RecipientId { get; set; }
        public string RecupientUsername { get; set; }
        public AppUser Recipient { get; set; }

        public string Content { get; set; }//What i write
        public DateTime? DateRead { get; set; }//When i read the message
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;// When i send the message,set the time to the current server timestamp
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }

    }
}