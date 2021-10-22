namespace API.Helpers
{
    public class MessageParams: PaginationParams
    {

        public string Usernmae { get; set; }
        public string Container { get; set; } = "Unread";
        
    }
}