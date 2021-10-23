
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{

    //IdentityUser<int> - Represents a user in the identity system, we define 'int' for id
    public class AppUser: IdentityUser<int>//Define user identity
    {

            //We dont need this anymore because that we have the identity of .NET
            // public int Id { get; set; }

            // public string UserName { get; set; }
            
            // public byte[] PasswordHash { get; set; }

            // public byte[] PasswordSalt { get; set; }

            public DateTime DateOfBirth { get; set; }

            public string KnownAs { get; set; }

            public DateTime Created { get; set; } = DateTime.Now;

            public DateTime LastActive { get; set; } = DateTime.Now;

            public string Gender { get; set; }

            public string Introduction { get; set; }

            public string LookingFor { get; set; }

            public string Interests { get; set; }

            public string City { get; set; }

            public string Country { get; set; }

            public ICollection<Photo> Photos { get; set; }

            public ICollection<UserLike> LikedByUsers { get; set; }//Users did liked

            public ICollection<UserLike> LikedUsers { get; set; }//Liked to our user
            
            public ICollection<Message> MessagesSent { get; set; }
            
            public ICollection<Message> MessagesReceived { get; set; }

            public ICollection<AppUserRole> UserRoles { get; set; }

            //We implement in AutoMapperProfiles in ForMember
            // public int GetAge(){

            //     return DateOfBirth.CalculatorAge();
            // }

    }

}