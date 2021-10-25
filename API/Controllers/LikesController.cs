using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    [Authorize]
    public class LikesController : BaseApiController
    {

        //----Use in UnitOfWork---//
        // private readonly IUserRepository r_UserRepository;
        // private readonly ILikeRepository r_UnitOfWork.LikeRepository;

        // public LikesController(IUserRepository i_UserRepository, ILikeRepository i_LikeRepository)
        // {
        //     this.r_UserRepository = i_UserRepository;
        //     this.r_UnitOfWork.LikeRepository = i_LikeRepository;
        // }

        private readonly IUnitOfWork r_UnitOfWork;


        public LikesController(IUnitOfWork i_UnitOfWork)
        {

            this.r_UnitOfWork = i_UnitOfWork;

        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)                                                                //We  create resource on here, we dont return to client
        {
            
            var sourceUserId = User.GetUserID();//Take my id
            var likedUser = await r_UnitOfWork.UserRepository.GetUserByUsernameAsync(username);//Find username of other user
            var sourceUser = await r_UnitOfWork.LikeRepository.GetUserWithLikes(sourceUserId);//Take user with like

            if(likedUser == null)
            {

                return NotFound();

            }

            if(sourceUser.UserName == username)
            {

                return BadRequest("You cannot like yourself");

            }

            var userLike = await r_UnitOfWork.LikeRepository.GetUserLike(sourceUserId, likedUser.Id);

            if(userLike != null)//I like him! Was matching
            {

                return BadRequest("You already like this user");

            }

            userLike = new UserLike
            {

                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
                
            };

            sourceUser.LikedUsers.Add(userLike);


            //Use in UnitOfWork
            // if(await r_UnitOfWork.UserRepository.SaveAllAsAsync())
            // {

            //     return Ok();
            
            // }

            if(await r_UnitOfWork.Complete())
            {

                return Ok();
            
            }



            return BadRequest("Failed to like user!");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTOs>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {

                    likesParams.UserId = User.GetUserID();
                    var users = await r_UnitOfWork.LikeRepository.GetUserLikes(likesParams);

                    Response.AddPaginationHeader(users.CurrentPage, users.PageSize,users.TotalCount, users.TotalPages);
                    return Ok(users);
        }
    }
}