using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        
        private readonly DataContext r_Context;

        //Repository injection to database
        public UserRepository(DataContext i_Context)
        {

            r_Context = i_Context;

        }

         /**
        Return user by id
        */
        public async Task<AppUser> GetUserByIdAsync(int id)
        {

           return await r_Context.Users.FindAsync(id);

        }

        /**
        Return user by username
        */
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {

                return await r_Context.Users
                        .Include(photosUser => photosUser.Photos)
                        .SingleOrDefaultAsync(
                            user => user.UserName == username.ToLower()
                );

        // .Include - //Take related collection, ,include with our response but its make a problme beacuse its do a cycle exceptions
        // When we allow to photo object so we have them ApppUser object and it access them
        //And its make a cycle calls
        
        }

        /**
        Return list of users
        */
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {

            return await r_Context.Users
                .Include(photosUser => photosUser.Photos)
                .ToListAsync();

        }

         /**
        Save changes  of context by user
        */
        public async Task<bool> SaveAllAsAsync()
        {

            return await r_Context.SaveChangesAsync() > 0;

        }

        public void Update(AppUser user)
        {

            //When we canges the property or state of user so we need to UPDATE that
                                            //Flag
           r_Context.Entry(user).State = EntityState.Modified;
           
        }
    }
}