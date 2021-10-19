using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
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

        public async Task<IEnumerable<LikeDTOs>> GetUserLikes(string predicate, int userId)
        {
            //Users like me
            var users = r_DataContext.Users.OrderBy(user => user.UserName).AsQueryable();
            var likes = r_DataContext.Likes.AsQueryable();

            //User that i like
            if(predicate.Equals("liked"))
            {

                //Take the likes by id
                likes = likes.Where(like => like.SourceUserId == userId);

                //Take the user by like
                users = likes.Select(like => like.LikedUser);
                
            }

            //User like me
            if(predicate.Equals("likedBy"))
            {

                likes = likes.Where(like => like.LikedUserId == userId);
                users = likes.Select(like => like.SourceUser);

            }

            return await users.Select(user => new LikeDTOs
            {

                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculatorAge(),
                PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain).Url,
                City = user.City,
                Id = user.Id

            }).ToListAsync();

        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {

            return await r_DataContext.Users
                .Include(like => like.LikedUsers)
                .FirstOrDefaultAsync(comprare => comprare.Id == userId);
               
        }

    }

}