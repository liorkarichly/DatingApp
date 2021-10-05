using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace API.Data
{
    public class Seed
    {
        
        ///Write the details in json file to database.
        ///The method is async because that i want to effective 
        //async Task -> return "void"
        public static async Task SeedUsers(DataContext dataContext)
        {

            if (await dataContext.Users.AnyAsync())
            {

                return;//We have any users

            }

            //We haven't ANY user

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");

             //Deserialize - Input inside user data
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            
            foreach(var user in users)
            {

                    using var hmac = new HMACSHA512();
                    user.UserName = user.UserName.ToLower();
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("PasswordDev"));
                    user.PasswordSalt = hmac.Key;

                    //We didnt need to use in operator 'await' because when we 
                    //add to database so we need to track in through entity
                    dataContext.Users.Add(user);

            }

                await dataContext.SaveChangesAsync();
   
        }
    }
}