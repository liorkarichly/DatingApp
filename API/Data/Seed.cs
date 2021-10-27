using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace API.Data
{
    public class Seed
    {
        
        ///Write the details in json file to database.
        ///The method is async because that i want to effective 
        //async Task -> return "void"
        public static async Task SeedUsers(UserManager<AppUser> userManager
        , RoleManager<AppRole> roleManager)// we replace to UserManger insted DataContext- כדי שיהווה לנו את הניהול של המשתמשים בממשק API
        {
            /*
            3 role in our system
                1. Role for Admin
                2. Role for Moderator
                3. Role for Member
            */
            //if (await dataContext.Users.AnyAsync())
            if(await userManager.Users.AnyAsync())
            {

                return;//We have any users

            }

            //We haven't ANY user

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            
             //Deserialize - Input inside user data
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            if(users == null)
            {

                return;

            }

            var roles = new List<AppRole>
            {

                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"}

            };

            foreach(var role in roles)
            {

                await roleManager.CreateAsync(role);
            }
            
            foreach(var user in users)
            {

                    //using var hmac = new HMACSHA512();
                    user.Photos.First().IsApproved = true;//Its take the first picture
                    user.UserName = user.UserName.ToLower();
                 
                    //We have the identity role
                    // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("PasswordDev"));
                    // user.PasswordSalt = hmac.Key;

                    //We didnt need to use in operator 'await' because when we 
                    //add to database so we need to track in through entity
                    //dataContext.Users.Add(user);
                    //await userManager.CreateAsync(user,  "Pa$$w0rd");
                    await userManager.AddToRoleAsync(user, "Member");//We put the role of user in system

           
            }
                //The UserManager save about the changes 
                //await dataContext.SaveChangesAsync();

                var admin = new AppUser{//Create admin of the system    

                    UserName = "admin"
                    
                };

                await userManager.CreateAsync(admin, "Pa$$w0rd");
                await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
   
        }
    }
}