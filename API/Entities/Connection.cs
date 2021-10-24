namespace API.Entities
{
    public class Connection
    {

        public Connection()//Default Cto'r, because that is entities
        {
            
        }
        public Connection(string i_ConnectionId, string i_Username)
        {

            ConnectionId = i_ConnectionId;

            Username = i_Username;

        }

        public string ConnectionId { get; set; }
        public string Username { get; set; }

    }
}