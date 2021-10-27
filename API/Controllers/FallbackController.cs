using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
/* 
fallback controller, what to do if it cannot find a route.
not going to drive from our base API controller.
We cannot derive from controller.
And the difference between this and what we're using is this is a class for an MVC controller with views.
Support now are angular app is the view for our application.
And what we're going to do is tell this full back controller what file to serve, and then we're going to
 tell our API what to do with any routes that it doesn't understand.
*/
    public class FallbackController:Controller
    {
        

        //Return the physical file, the index to its HTML.
        public ActionResult Index()
        {
                                                                            //Spcefiy folder
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html" ), "text/HTML");

        }
    }
}