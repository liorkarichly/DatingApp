using System.Security.Claims;

namespace API.Extensions
{
   
    public static class ClaimsPrincipalExtensions
    {
        

        public static string GetUsername(this ClaimsPrincipal username)
        {

           return username.FindFirst(ClaimTypes.NameIdentifier)?.Value;//Take the username by the token that use in authenticate 

        }   

    }

}