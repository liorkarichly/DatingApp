using System;
using System.Collections.Generic;

namespace API.DTOs
{
    public class MemberDTOs
    {
        
        //We dont want to get back the password...
            public int Id { get; set; }

            public string Username { get; set; }
            
            public string PhotoUrl { get; set; }
            
            public int Age { get; set; }

            public string KnownAs { get; set; }
            
            public DateTime Created { get; set; } 

            public DateTime LastActive { get; set; } 

            public string Gender { get; set; }

            public string Introduction { get; set; }

            public string LookingFor { get; set; }

            public string Interests { get; set; }

            public string City { get; set; }

            public string Country { get; set; }

            public ICollection<PhotoDTOs> Photos { get; set; }
            
    }

}