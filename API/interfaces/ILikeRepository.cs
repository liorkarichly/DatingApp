using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.interfaces
{

    public interface ILikeRepository
    {
         
            Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);

            Task<AppUser> GetUserWithLikes(int userId);

            Task<IEnumerable<LikeDTOs>> GetUserLikes(string predicate, int userId);//Search users that have connnection

    }

}