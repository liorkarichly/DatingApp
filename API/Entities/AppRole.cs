using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    
    //IdentityRole<int> - Represents a role in the identity system
    public class AppRole: IdentityRole<int>
    {
        
        /*We member can be many to many role and our identity its not enough
         each app user can be in multiple roles and each role can contain multiple users. */
        
        public ICollection<AppUserRole> UserRoles { get; set; }//Its like AppUser and we need it for relationship

    }
}