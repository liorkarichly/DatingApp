using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {

        public static IServiceCollection AddIdentityServices(this IServiceCollection i_Service, IConfiguration i_Config){


            //Add middleware,
            //First, add authentication             
            i_Service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                options => {

                    options.TokenValidationParameters = new TokenValidationParameters{//Supply token validation 

                        ValidateIssuerSigningKey = true,//validate the issue, assigning key and server is going to sign the token and we need to tell it to actully validated this token is correct.
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(i_Config["TokenKey"])),
                        ValidateIssuer = false,//Flag , validate the issuer, token is obviously our API server  
                        ValidateAudience = false// Flag, validate the audience, token is our angular application
                    };
                }
            );


            return i_Service;
        }
        
    }
}