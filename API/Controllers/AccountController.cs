using API.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.interfaces;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> r_UserManager;
        private readonly SignInManager<AppUser> r_SignInManager;

        //private readonly DataContext r_DataContext;
        private readonly ITokenService r_TokenServiceInterface;
        private readonly IMapper r_MapperController;

        public AccountController(//DataContext i_DataContext
        UserManager<AppUser> i_UserManager
        ,SignInManager<AppUser> i_SignInManager
        , ITokenService i_Token
        , IMapper i_MapperController)
        {

           
            //r_DataContext = i_DataContext;//We have UserManager and SignManager
            this.r_TokenServiceInterface = i_Token;
            this.r_MapperController = i_MapperController;
             this.r_UserManager = i_UserManager;
            this.r_SignInManager = i_SignInManager;
        }

        [HttpPost("register")]//automatically binds to any parameter that it finds in the parameters of our method here
        public async Task<ActionResult<UsersDTOs>> Register(RegisterDTOs registerDTOs)
        {

            //return status 404 code 
            if (await userExists(registerDTOs.UserName))
            {

                return BadRequest("Username is taken");

            }

            var user = r_MapperController.Map<AppUser>(registerDTOs);

             user.UserName = registerDTOs.UserName.ToLower();

            //using var hmac = new HMACSHA512();//Create new hash for password

            // var user = new AppUser - I create mapper with register
            // {

               //user.UserName = registerDTOs.UserName.ToLower();
                //We have the identity role
                // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTOs.Password));
                // user.PasswordSalt = hmac.Key;

            //};

            //r_DataContext.Users.Add(user);
            //await r_DataContext.SaveChangesAsync();

            var result = await r_UserManager.CreateAsync(user, registerDTOs.Password);

            if(!result.Succeeded)
            {

                return BadRequest(result.Errors);

            }

            //Add the role of user
            var roleResult = await r_UserManager.AddToRoleAsync(user, "Member");

            if(!result.Succeeded)
            {

                return BadRequest(result.Errors);

            }
            
            return new UsersDTOs
            {

                Username = user.UserName,
                Token = await r_TokenServiceInterface.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender

            };

        }

        /// Checking if the user is exists
        private async Task<bool> userExists(string username)
        {
                        ////Insted r_DataContext
            return await r_UserManager.Users.AnyAsync(
                     userExists => userExists.UserName == username.ToLower()
            );

        }

        [HttpPost("login")]
        public async Task<ActionResult<UsersDTOs>> Login(LoginDTOs loginDTOs)
        {
            
            var user = await r_UserManager//Insted r_DataContext
                            .Users
                            .Include(photo => photo.Photos)
                            .SingleOrDefaultAsync(//Checking if we have the username in database and return the value
                                userExists =>
                                userExists.UserName
                                == loginDTOs.Username);
                                
            if (user == null)
            {

                return Unauthorized("Invalid username");

            }
                                                                                        //we want to lock out the user on failure, we're not going to do that.
            var result = await r_SignInManager.CheckPasswordSignInAsync(user, loginDTOs.Password, false);
            
            if(!result.Succeeded)
            {

                return Unauthorized();
                
            }
            //We have the identity role
            //Checking if the password compueted to input password of user
            //using var hmac = new HMACSHA512(user.PasswordSalt);

            // var compuedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTOs.Password));

            // for (int i = 0; i < compuedHash.Length; i++)
            // {

            //     if (compuedHash[i] != user.PasswordHash[i])
            //     {

            //         return Unauthorized("Invalid password");

            //     }

            // }

            return new UsersDTOs
            {

                Username = user.UserName,
                Token = await r_TokenServiceInterface.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(photoIsMain => photoIsMain.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
                
            };

        }

    }

}