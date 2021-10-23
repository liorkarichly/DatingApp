using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController: BaseApiController
    {
        private readonly UserManager<AppUser> r_UserManager;

        public AdminController(UserManager<AppUser> i_UserManager)
        {

            this.r_UserManager = i_UserManager;

        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWoles()
        {

            var users = await r_UserManager.Users
                        .Include(role => role.UserRoles)
                        .ThenInclude(role => role.Role)
                        .OrderBy(user => user.UserName)
                        .Select(user => new {
                            user.Id,
                            Username = user.UserName,
                            Roles = user.UserRoles.Select(role => role.Role.Name).ToList()
                        })
                        .ToListAsync();
            
            return Ok(users);

        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            
            return Ok("Amdin ot Moderators can see this");

        }

         [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {

                var selectedRoles = roles.Split(",").ToArray();
                var user = await r_UserManager.FindByNameAsync(username);//Find username
                
                if(user == null)
                {

                    return NotFound("Could not find user");

                }
                
                var userRoles = await r_UserManager.GetRolesAsync(user);//Return list of roles ("Admin", "Moderator", "Member") but specific to user

                var result = await r_UserManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));//Retrun user with his roles

                if(!result.Succeeded)
                {

                    return BadRequest("Failed to add to roles");

                }

                result = await r_UserManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));


                if(!result.Succeeded)
                {

                    return BadRequest("Failed to remove from roles");

                }

            return Ok(await r_UserManager.GetRolesAsync(user));

        }


        
    }
}