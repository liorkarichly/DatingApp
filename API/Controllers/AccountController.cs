using API.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.interfaces;

namespace API.Controllers
{
    public class AccountController: BaseApiController
    {
        
        private readonly DataContext r_DataContext;
        private readonly ITokenService r_TokenServiceInterface;

        public AccountController(DataContext i_DataContext, ITokenService i_Token)
        {
            
            r_DataContext = i_DataContext;
            r_TokenServiceInterface = i_Token;

        }

        [HttpPost("register")]//automatically binds to any parameter that it finds in the parameters of our method here
        public async Task<ActionResult<UsersDTOs>> Register(RegisterDTOs registerDTOs)
        {

                //return status 404 code 
                if(await userExists(registerDTOs.Username))
                {

                  return BadRequest("Username is taken");

                }

            using var hmac = new HMACSHA512();//Create new hash for password

            var user = new AppUser
            {

                    UserName = registerDTOs.Username.ToLower(),
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTOs.Password)),
                    PasswordSalt = hmac.Key

            };

        r_DataContext.Users.Add(user);
        await r_DataContext.SaveChangesAsync();
      
        return new UsersDTOs
        {

            Username = user.UserName,
            Token = r_TokenServiceInterface.CreateToken(user)
            
        };

        }
 
        /// Checking if the user is exists
        private async Task<bool> userExists(string username)
        {

             return await r_DataContext.Users.AnyAsync(
                      userExists => userExists.UserName == username.ToLower()
             );

        }

        [HttpPost("login")]
        public async Task<ActionResult<UsersDTOs>> Login(LoginDTOs loginDTOs){


            var user = await r_DataContext
                            .Users
                            .SingleOrDefaultAsync(//Checking if we have the username in database and return the value
                                userExists => 
                                userExists.UserName 
                                == loginDTOs.Username
                                );

            if(user == null)
            {

                return Unauthorized("Invalid username");
            
            }

            //Checking if the password compueted to input password of user
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var compuedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTOs.Password));

            for(int i = 0; i < compuedHash.Length; i++)
            {

                if(compuedHash[i] != user.PasswordHash[i])
                {

                    return Unauthorized("Invalid password");

                }

            } 

             return new UsersDTOs
                {

                Username = user.UserName,
                Token = r_TokenServiceInterface.CreateToken(user)
            
                };

        }
        
    }

}