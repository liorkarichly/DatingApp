using System;
using System.Text.Json.Serialization;

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
        public string RecipientUsername { get; set; }
        public string  RecipientPhotoUrl { get; set; }

        public string Content { get; set; }//What i write
        public DateTime? DateRead { get; set; }//When i read the message
        public DateTime MessageSent { get; set; }

        //Jason, ignore on these.
        [JsonIgnore]
        public bool SenderDeleted { get; set; }

        [JsonIgnore]
        public bool RecipientDeleted { get; set; }
        
    }
}