namespace API.Helpers
{
    public class UserParams: PaginationParams
    {
        //For sorting member
        public string CurrentUsername { get; set; }

        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 120;

        public string OrederBy { get; set; } = "lastActive";
       

    }
}