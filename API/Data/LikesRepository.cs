using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikeRepository
    {
        private readonly DataContext r_DataContext;

        public LikesRepository(DataContext i_DataContext)
        {
            this.r_DataContext = i_DataContext;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
                                                        //Primary keys
            return await r_DataContext.Likes.FindAsync(sourceUserId, likedUserId);

        }

        public async Task<PagedList<LikeDTOs>> GetUserLikes(LikesParams likesParams)
        {
            //Users like me
            var users = r_DataContext.Users.OrderBy(user => user.UserName).AsQueryable();
            var likes = r_DataContext.Likes.AsQueryable();

            //User that i like
            if(likesParams.Predicate.Equals("liked"))
            {

                //Take the likes by id
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);

                //Take the user by like
                users = likes.Select(like => like.LikedUser);
                
            }

            //User like me
            if(likesParams.Predicate.Equals("likedBy"))
            {

                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);

            }

            var likedUser =  users.Select(user => new LikeDTOs
            {

                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculatorAge(),
                PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain).Url,
                City = user.City,
                Id = user.Id

            });

            return await PagedList<LikeDTOs>.CreateAsync(likedUser
                                                        ,likesParams.PageNumber, likesParams.PageSize);

        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {

            return await r_DataContext.Users
                .Include(like => like.LikedUsers)
                .FirstOrDefaultAsync(comprare => comprare.Id == userId);
               
        }

    }

}