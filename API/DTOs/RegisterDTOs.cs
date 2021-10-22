using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTOs
    {
         
        [Required] public string UserName { get; set; }
  
        [Required] public string KnownAs { get; set; }

        [Required]  public string Gender { get; set; }
     
        [Required] public DateTime DateOfBirth { get; set; }

        [Required] public string City { get; set; }

        [Required] public string Country { get; set; }

        [Required]
        [StringLength(10,MinimumLength = 6)]//Maximum , Mininum 
        public string Password { get; set; }

    }
}