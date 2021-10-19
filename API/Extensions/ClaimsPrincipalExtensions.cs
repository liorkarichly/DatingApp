using System.Security.Claims;

namespace API.Extensions
{
   
    public static class ClaimsPrincipalExtensions
    {

        public static string GetUsername(this ClaimsPrincipal username)
        {

           //return username.FindFirst(ClaimTypes.NameIdentifier)?.Value;//Take the username by the token that use in authenticate 
            return username.FindFirst(ClaimTypes.Name)?.Value;//use in here is claim types name for the user name.

        } 

        
        public static int GetUserID(this ClaimsPrincipal username)
        {

           return int.Parse(username.FindFirst(ClaimTypes.NameIdentifier)?.Value);//Take the username by the token that use in authenticate 

        }   

    }

}