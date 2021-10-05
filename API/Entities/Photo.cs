using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    //Many to One - many photos -> one user
    [Table("Photos")]
    public class Photo
    {

        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsMain { get; set; }//Main photo or not

        public string PublicId { get; set; }//For storage photo
        
        public AppUser AppUser { get; set; }

        public int AppUserId { get; set; }
    }
}