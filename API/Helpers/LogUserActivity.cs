using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using API.Extensions;
using API.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

                var resultContext = await next();

                if(!resultContext.HttpContext.User.Identity.IsAuthenticated)
                {

                        return;

                } 

                var userId = resultContext.HttpContext.User.GetUserID();
                var report = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
                var user = await report.GetUserByIdAsync(userId);
                user.LastActive = DateTime.Now;
                await report.SaveAllAsAsync();
                
        }
        
    }

}