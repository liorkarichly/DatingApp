using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController: BaseApiController
    {
        private readonly UserManager<AppUser> r_UserManager;
        private readonly IUnitOfWork r_UnitOfWork;
        private readonly IPhotoService r_PhotoService;

        public AdminController(UserManager<AppUser> i_UserManager
                            , IUnitOfWork i_UnitOfWork
                            , IPhotoService i_PhotoService)
        {

            this.r_UserManager = i_UserManager;
            this.r_UnitOfWork = i_UnitOfWork;
            this.r_PhotoService = i_PhotoService;
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
        public async Task<ActionResult> GetPhotosForModeration()
        {
            
            var photos = await r_UnitOfWork.PhotoRepository.GetUnapprovedPhotos();

            return Ok(photos);

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


        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            
            var photo = await r_UnitOfWork.PhotoRepository.GetPhotoById(photoId);

            if(photo.PublicId != null)
            {

                var result =  await r_PhotoService.DeletePhotoAsync(photo.PublicId);//Remove from cloudinary

                if(result.Result == "ok")
                {

                        r_UnitOfWork.PhotoRepository.RemovePhoto(photo);

                }

            }
            else
            {

                    r_UnitOfWork.PhotoRepository.RemovePhoto(photo);

            }

            await r_UnitOfWork.Complete();

            return Ok();

        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId){

            var photo = await r_UnitOfWork.PhotoRepository.GetPhotoById(photoId);

            if(photo == null)
            {

                return NotFound("Could not find photo");

            }

            photo.IsApproved = true;

            var user = await r_UnitOfWork.UserRepository.GetUserByPhotoId(photoId);

            if(!user.Photos.Any(photo => photo.IsMain))
            {

                photo.IsMain = true;

            }

            await r_UnitOfWork.Complete();

            return Ok();

        }

        
    }

}