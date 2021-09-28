namespace API.Errors
{
    public class ApiException
    {
        
        public ApiException(int i_StatusCode, string i_Messages = null, string i_Details = null)
        {

            StatusCode = i_StatusCode;
            Messages = i_Messages;
            Details = i_Details;
            
        }
        public int StatusCode { get; set; }
        public string Messages { get; set; }
        public string Details { get; set; }
    }
}