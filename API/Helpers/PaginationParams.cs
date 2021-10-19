namespace API.Helpers
{
    public class PaginationParams
    {
        
        private const int  MaxPageSize = 50;

        private int m_PageSize = 10;//defualt

        public int PageNumber { get; set; } = 1;


        public int PageSize
        { 
            
            get => m_PageSize ;
            set =>  m_PageSize = (value > MaxPageSize)? MaxPageSize : value;
        }
    }
}