using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.interfaces
{

    public interface ILikeRepository
    {
         
            Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);

            Task<AppUser> GetUserWithLikes(int userId);

            Task<PagedList<LikeDTOs>> GetUserLikes(LikesParams likesParams);//Search users that have connnection

    }

}