using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]//api-start//[controller]-placeholder
    public class BaseApiController:ControllerBase
    {
        
    }
}