using API.Entities;
using API.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;//DateTime
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        
        /*
        Got access to the symmetric secuirty key.
        Type of encryption where only one key is used to both encrypt
        and decrypt electronic information.
        Same key is used to both side JWT token.
        The key dosent need to leave the server.
        */
        private readonly SymmetricSecurityKey r_SymmetricSecurityKey;
        private readonly UserManager<AppUser> r_UserManger;

        public TokenService(IConfiguration i_Config, UserManager<AppUser> i_UserManger)
        {
            //Get byte[]                                            
            r_SymmetricSecurityKey = new SymmetricSecurityKey(
                Encoding
                .UTF8               //Property
                .GetBytes(i_Config["TokenKey"]));

            this.r_UserManger = i_UserManger;

        }

        ///Actual logic of what we need to do inside here to actually create the token
        public async Task<string> CreateToken(AppUser user)
        {
            
            //Insid the token, save claims
            var claims  = new List<Claim>
            {
                           
                            //When we used in our user name so its going to name identifier.
                            //We using in our username and store to inside
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),//Returning the user's username and a token, we're going to also sets the user ID inside there as well so that we've got easy access to either the ID or the user name when we receive a token.
                     new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)

            };

            var roles = await r_UserManger.GetRolesAsync(user);//list of roles that this user
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));//add them into our list of claims,The JWT registered claim names do not have an option for a role inside here.

            //Create credentials, we save all creds equals new signing credentials
            var credentials = new SigningCredentials(
                r_SymmetricSecurityKey//Security key
                , SecurityAlgorithms.HmacSha256Signature// Stromg Algo
                );
           
            //For describe our token 
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(claims),//Contains our claims identity and pass in the claims.
                Expires = DateTime.UtcNow.AddDays(7),//How long is this token going to be valid for?
                SigningCredentials = credentials//Need to pass in the signing credentials into less and well specify credits.

            };

            //Create token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        /*
        Flow create token 

        1. add claims
        2. Create some credentials
        3. Describe the our token
        4. Create token handler
        5. Write the token
        */

        }

    }
    
}