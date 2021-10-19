using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository r_UserRepository;
        private readonly ILikeRepository r_LikeRepository;

        public LikesController(IUserRepository i_UserRepository, ILikeRepository i_LikeRepository)
        {
            this.r_UserRepository = i_UserRepository;
            this.r_LikeRepository = i_LikeRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)                                                                //We  create resource on here, we dont return to client
        {
            
            var sourceUserId = User.GetUserID();//Take my id
            var likedUser = await r_UserRepository.GetUserByUsernameAsync(username);//Find username of other user
            var sourceUser = await r_LikeRepository.GetUserWithLikes(sourceUserId);//Take user with like

            if(likedUser == null)
            {

                return NotFound();

            }

            if(sourceUser.UserName == username)
            {

                return BadRequest("You cannot like yourself");

            }

            var userLike = await r_LikeRepository.GetUserLike(sourceUserId, likedUser.Id);

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

            if(await r_UserRepository.SaveAllAsAsync())
            {

                return Ok();
            
            }

            return BadRequest("Failed to like user!");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTOs>>> GetUserLikes(string predicate)
        {

                    var users = await r_LikeRepository.GetUserLikes(predicate, User.GetUserID());

                    return Ok(users);
        }
    }
}