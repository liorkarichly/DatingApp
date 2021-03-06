using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using API.Entities;
using System.Threading.Tasks;
using API.interfaces;
using API.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using API.Extensions;
using System.Linq;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{

    // [ApiController]
    // [Route("api/[controller]")]//api-start//[controller]-placeholder
    //[Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUnitOfWork r_UnitOfWork;

        // private readonly DataContext r_DataContext;
        //private readonly IUserRepository r_UserRepository;//Use in UnitOfWork
        private readonly IMapper r_Mapper;
        private readonly IPhotoService r_PhotoService;

        public UsersController(//IUserRepository i_UserRepository
         IUnitOfWork i_UnitOfWork, IMapper i_Mapper, IPhotoService i_PhotoService) //:base(i_Context)
        {

            this.r_UnitOfWork = i_UnitOfWork;
            this.r_Mapper = i_Mapper;
            this.r_PhotoService = i_PhotoService;
            //r_UserRepository = i_UserRepository;
            // r_DataContext = i_Context;

        }

        //End point for list users, result to client
        //Async Task its make to call to database in synchornous
        // [AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTOs>>> GetUsers([FromQuery] UserParams userParams)
        {

            //Ok() magically formats it to a  OkResult and OkResult inherits from ActionResult.
            //Return table.nameTable.List
            //await - Operator that wait to call to API for to work on background thread
            //return Ok(await r_UserRepository.GetUsersAsync());
            // OR 
            //return r_DataContext.Users.ToListAsync().result; -- not after that  Ok(await r_UserRepository.GetUsersAsync()); its version better


            // var users = await r_UserRepository.GetUsersAsync();
            // var usersToReturn = r_Mapper.Map<IEnumerable<MemberDTOs>>(users);
            //                                                //TO

            // var currentUser = await r_UserRepository.GetUserByUsernameAsync(User.GetUsername());
            // userParams.CurrentUsername =  currentUser.UserName;
            var gender = await r_UnitOfWork.UserRepository.GetUserGender(User.GetUsername());
            userParams.CurrentUsername = User.GetUsername();

            if(string.IsNullOrEmpty(userParams.Gender))
            {

                userParams.Gender = gender == "female" ? "male" : "female";
            
            }

             //   var users = await r_UserRepository.GetMembersAsync();
             //var users = await r_UserRepository.GetMembersAsync(userParams);//Use in UnitOfWork
             var users = await r_UnitOfWork.UserRepository.GetMembersAsync(userParams);//Photo Challenge

             Response.AddPaginationHeader(users.CurrentPage, users.PageSize
                                    , users.TotalCount, users.TotalPages);

            return Ok(users);
            
        }

        // //api/users/3
        // // [Authorize]
        // [HttpGet("{Id}")]
        // public async Task<ActionResult<AppUser>> GetUsers(int Id)///Endpoint for specifics parameter
        // {

        //     ///Return the user by id , if is exists in database
        //     return await r_UserRepository.GetUserByIdAsync(Id);

        // }

        //[Authorize(Roles = "Member")]
                                //For route name
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTOs>> GetUsers(string username)///Endpoint for specifics parameter
        {


            ///Return the user by id , if is exists in database
            //return await r_UserRepository.GetUserByUsernameAsync(username);
            var currentUsername = User.GetUsername();//Photo Challenge
            //return await r_UserRepository.GetMemberAsync(username);//Go and get the user 
            return await r_UnitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser: currentUsername == username);//Use in UnitOfWork
            ///return r_Mapper.Map<//MemberDTOs>(user);//Search the user and convert him to memberDTO
            ///We return from GetMemberAsync now!
        }
        //I remove the [Authorize] and [AllowAnonymous] because that i want to make to user 
        // to get the other users when they authentacion

/*------------------------Save Edit - Update User--------------------------------*/
        [HttpPut]
        public async Task<ActionResult> UpadteUser(MemberUpdateDTOs memberUpdateDTOs){

           // var username = User.GetUsername();//Take the username by the token that use in authenticate 
            //var user = await r_UserRepository.GetUserByUsernameAsync(User.GetUsername());//Use in UnitOfWork
            var user = await r_UnitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            r_Mapper.Map(memberUpdateDTOs, user);

            //r_UserRepository.Update(user);//Use in UnitOfWork
            r_UnitOfWork.UserRepository.Update(user);

            //Use in UnitOfWork
            // if (await r_UserRepository.SaveAllAsAsync())
            // {

            //     return NoContent();

            // }

            if (await r_UnitOfWork.Complete())
            {

                return NoContent();

            }

            return BadRequest("Failed to update user");
        
        }
        /*-----------------------------Cloudianry--------------------*/

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTOs>> AddPhoto(IFormFile file)
        {
            
            //Use in UnitOfWork
            //var user = await r_UserRepository.GetUserByUsernameAsync(User.GetUsername());//User.GetUsername() - User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//Take the username by the token that use in authenticate 
            
            var user = await r_UnitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var result = await r_PhotoService.AddPhotoAsync(file);//Get response from service

            //Checking if we have problem with upload photo
            if (result.Error != null)
            {

                    return BadRequest(result.Error.Message);

            }

            //Parse the new photo
            var photo = new Photo{

                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId

            };

            //Checking if to member have pictures
            // if(user.Photos.Count == 0 )//&& user.Photos.First().IsApproved)//Photo Challenge
            // {

            //     photo.IsMain = true;//The photo is tje main now 
            
            // }

            //Add photo
            user.Photos.Add(photo);

            // if(await r_UserRepository.SaveAllAsAsync())//Use in UnitOfWork
            // {

            //     // return r_Mapper.Map<PhotoDTOs>(photo);
            //  //Miss Match   // return CreatedAtRoute("GetUser",r_Mapper.Map<PhotoDTOs>(photo));//We use it because is easy to route to object and return back   
            //     return CreatedAtRoute("GetUser",new {username = user.UserName},r_Mapper.Map<PhotoDTOs>(photo));
            // }

             if(await r_UnitOfWork.Complete())
            {
            
              return CreatedAtRoute("GetUser",new {username = user.UserName},r_Mapper.Map<PhotoDTOs>(photo));
            
            }

            return BadRequest("Problem adding photo");
                 
        }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoID)
    {
        
        //var user = await r_UserRepository.GetUserByUsernameAsync(User.GetUsername());//Use in UnitOfWork

        var user = await r_UnitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        var photo = user.Photos.FirstOrDefault(matchPhoto => matchPhoto.Id == photoID);//This is not asynchronous  because its from memory we dont get to database 
  
        if(photo.IsMain)//Checking if the photo is not main
        {

            return BadRequest("This is already your main photo!");

        }

        var currentMainPhoto = user.Photos.FirstOrDefault(takeMainPhoto => takeMainPhoto.IsMain);// && takeMainPhoto.IsApproved == true);//Take the main photo, Photo Challenge

        if(currentMainPhoto != null)
        {

            currentMainPhoto.IsMain = false;

        }

        photo.IsMain = true;

        // if(await r_UserRepository.SaveAllAsAsync())//Use in UnitOfWork
        // {

        //     return NoContent();

        // }

        if(await r_UnitOfWork.Complete())
        {

            return NoContent();

        }

        return BadRequest("Failed to set photo");

    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {

        //var user = await r_UserRepository.GetUserByUsernameAsync(User.GetUsername());//Use in UnitOfWork

        var user = await r_UnitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        var photo =  await r_UnitOfWork.PhotoRepository.GetPhotoById(photoId);

            if(photo == null)
            {

                return NotFound();

            }

            if(photo.IsMain)
            {

                return BadRequest("You cannot delete your main photo");

            }

            if(photo.PublicId != null)//Checking if to phto have public id for to delete from cloudinary
            {

                //We need to check if the deleted in cloudinary is success
                var result = await r_PhotoService.DeletePhotoAsync(photo.PublicId);

                if(result.Error != null)
                {
                    
                    return BadRequest(result.Error.Message);

                }

            }

            user.Photos.Remove(photo);//Delete from database

            // if(await r_UserRepository.SaveAllAsAsync())//Use in UnitOfWork
            // {

            //     return Ok();

            // }

            if(await r_UnitOfWork.Complete())
            {

                return Ok();

            }

            return BadRequest("Failed to delete your photo");

    }

}

}