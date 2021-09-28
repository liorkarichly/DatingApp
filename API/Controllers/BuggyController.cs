using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Entities;


namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {

        private readonly DataContext r_DataContext;
        public BuggyController(DataContext i_Context)
        {

                r_DataContext = i_Context;
        }

//Method diffrentlly that response is not exists!
#region Method Exception

        
        [Authorize] //Require the users to be authenticated.      
        [HttpGet("auth")]//Root parameter, this is finally way for our API and after going to 'auth'
        public ActionResult<string> GetSecret()//The purpose of this is to test our 401, unauthorised responses.
        {

                return "Secret text";
        }

              
        [HttpGet("not-found")]//Root parameter, this is finally way for our API and after going to 'auth'
        public ActionResult<AppUser> GetNotFound()//The purpose of this is to test our 401, unauthorised responses.
        {

                var thingsUser = r_DataContext.Users.Find(-1);//User not going to exists
           
           if (thingsUser == null)//Not found '404'
           {

               return NotFound();
           }

           return Ok(thingsUser); //Found!! '200'

        }


         [HttpGet("server-error")]//Root parameter, this is finally way for our API and after going to 'auth'
        public ActionResult<string> GetServerError()//The purpose of this is to return null reference exception
        {

                        var thingsUser = r_DataContext.Users.Find(-1);//User not going to exists
           
                        //when the parameter 'thingsUser' is null so we get null and its leading to null reference exception
                        var thingsToReturn = thingsUser.ToString();

                         return thingsToReturn;
               
        }

        [HttpGet("bad-request")]//Root parameter, this is finally way for our API and after going to 'auth'
        public ActionResult<string> GetBadRequest()//The purpose of this is to return gets that is not was good GET from user, when i sending bad request that we dont know what i do with this so we sending back the bad request too that the user knowledge that its was bad request
        {

             return BadRequest("This is not good request");

        }
        
#endregion
    }
}