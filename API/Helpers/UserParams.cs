namespace API.Helpers
{
    public class UserParams
    {
        
        private const int  MaxPageSize = 50;

        private int m_PageSize = 10;//defualt

        public int PageNumber { get; set; } = 1;

        public string CurrentUsername { get; set; }

        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 120;

        public string OrederBy { get; set; } = "lastActive";
        public int PageSize
        { 
            
            get => m_PageSize ;
            set =>  m_PageSize = (value > MaxPageSize)? MaxPageSize : value;
        }

    }
}