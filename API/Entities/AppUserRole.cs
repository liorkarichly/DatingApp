using Microsoft.AspNetCore.Identity;

namespace API.Entities
{

  //IdentityUserRole<int> - Represents the link between a user and a role.
    public class AppUserRole: IdentityUserRole<int>
    {
        
        /*we're going to do is we're going to specify the joint entities that we need 
          for this so we can have a properly up user.
          And this is going to be a user.
        */
        public AppUser User { get; set; }
        public AppRole Role { get; set; }

    }
}