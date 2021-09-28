using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTOs
    {
        
        [Required]
        public string Username { get; set; }

         [Required]
         [StringLength(10,MinimumLength = 6)]//Maximum and Mininum 
        public string Password { get; set; }

    }
}