namespace API.Entities
{
    public class UserLike
    {
        
       public AppUser SourceUser { get; set; }//First user like
        public int SourceUserId { get; set; }//Id of first user the like 
        public AppUser LikedUser { get; set; }//second user like him
        public int LikedUserId { get; set; }//Id of second user like

    }

}