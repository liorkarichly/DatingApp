using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using API.Data;
using System.Threading.Tasks;

namespace API.Extensions
{
    //Identity service extensions.
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection i_Service, IConfiguration i_Config)
        {
/*We've gone for a single page application using ANGULAR and we're using token based authorization. so we use in AddIdentityCore*/
                //AddIdentityCore - Adds and configures the identity system for the specified User type. Role services are not added by default
                //AddRoles - Adds Role related services for TRole, including IRoleStore, IRoleValidator, and RoleManager.
                //AddRoleManager - Adds a RoleManager<TRole> for the IdentityBuilder.RoleType.
                i_Service.AddIdentityCore<AppUser>(options =>
                {
                    options.Password.RequireNonAlphanumeric = false;
                    
                })
                .AddRoles<AppRole>()//Get entity from API
                .AddRoleManager<RoleManager<AppRole>>()//Manager our entities and we gice to role manager the type of AppRole,Provides the APIs for managing roles in a persistence store.
                .AddSignInManager<SignInManager<AppUser>>()//Provides the APIs for user sign in.
                .AddRoleValidator<RoleValidator<AppRole>>()//RoleValidator - Provides the default validation of roles.
                .AddEntityFrameworkStores<DataContext>();//Adds an Entity Framework implementation of identity information stores.

            //Add middleware,
            //First, add authentication             
            i_Service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                options => 
                {

                    options.TokenValidationParameters = new TokenValidationParameters
                    {//Supply token validation 

                        ValidateIssuerSigningKey = true,//validate the issue, assigning key and server is going to sign the token and we need to tell it to actully validated this token is correct.
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(i_Config["TokenKey"])),
                        ValidateIssuer = false,//Flag , validate the issuer, token is obviously our API server  
                        ValidateAudience = false// Flag, validate the audience, token is our angular application
                   
                    };

                    options.Events = new JwtBearerEvents{
                        OnMessageReceived = context => 
                        {
                            var accessToken = context.Request.Query["access_token"];//This needs to be specific because signalR by default will send up our token.
                            
                            //We want to do is take the path
                            var path = context.HttpContext.Request.Path;

                            if(!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs")) 
                            {

                                context.Token = accessToken;

                            }
                        
                            return Task.CompletedTask;
                            
                        }};

                }

            );

        i_Service.AddAuthorization(
            options =>
            {

                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            
            });

            return i_Service;

        }
        
    }

}