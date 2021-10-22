using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using API.Extensions;
using API.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace API.Helpers
{

    //IAsyncActionFilter - Action filters allow us to do
    //something even before the request is executing or after the request is executed.
    public class LogUserActivity : IAsyncActionFilter
    {

        //ActionExecutionDelegate - Indicating the action or the next action filter has executed.
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

                var resultContext = await next();//We're going to execute and updates this last active property.

                if(!resultContext.HttpContext.User.Identity.IsAuthenticated)//Check to see if the users authenticated 
                {

                        return;

                } 

                var userId = resultContext.HttpContext.User.GetUserID();
                var report = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();//Get access to our repository 
                var user = await report.GetUserByIdAsync(userId);
                user.LastActive = DateTime.Now;//Update time activity
                await report.SaveAllAsAsync();//Save changes
                
        }
        
    }

}