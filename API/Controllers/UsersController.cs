using Microsoft.AspNetCore.Mvc;
using API.Data;
using System.Collections.Generic;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]//api-start//[controller]-placeholder
    public class UsersController: ControllerBase
    {
        
        private readonly DataContext r_DataContext;

        public UsersController(DataContext i_Context)
        {
    
                r_DataContext = i_Context;

        }

        
        //End point for list users, result to client
        //Async Task its make to call to database in synchornous
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {

            //Return table.nameTable.List
            //await - Operator that wait to call to API for to work on background thread
            return await r_DataContext.Users.ToListAsync();
            // OR 
            //return r_DataContext.Users.ToListAsync().result;
            
        }

        
        //api/users/3
        [HttpGet("{Id}")]
        public async Task<ActionResult<AppUser>> GetUsers(int Id)///Endpoint for specifics parameter
        {

            ///Return the user by id , if is exists in database
            return await r_DataContext.Users.FindAsync(Id);
            
        }


    }
}