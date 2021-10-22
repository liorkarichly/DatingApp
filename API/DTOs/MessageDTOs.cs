using System;

namespace API.DTOs
{
    public class MessageDTOs
    {
        
         //Sender
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }

        public string SenderPhotoUrl { get; set; }

        //Recipient
        public int RecipientId { get; set; }
        public string RecupientUsername { get; set; }
        public string  RecipientPhotoUrl { get; set; }

        public string Content { get; set; }//What i write
        public DateTime? DateRead { get; set; }//When i read the message
        public DateTime MessageSent { get; set; }
        
    }
}