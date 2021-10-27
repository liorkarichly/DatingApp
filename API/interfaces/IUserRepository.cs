using System.Threading.Tasks;
using API.Entities;
using System.Collections.Generic;
using API.DTOs;
using API.Helpers;

namespace API.interfaces
{
    //Update is function of tracking status so its not TASK type
    public interface IUserRepository
    {
         
         void Update(AppUser user);

         //Task<bool> SaveAllAsAsync();Use in UnitOfWork

         Task<IEnumerable<AppUser>> GetUsersAsync();

         Task<AppUser> GetUserByIdAsync(int id);

         Task<AppUser> GetUserByUsernameAsync(string username);

         //Task<IEnumerable<MemberDTOs>> GetMembersAsync();
         Task<PagedList<MemberDTOs>> GetMembersAsync(UserParams userParams);

         Task<MemberDTOs> GetMemberAsync(string username, bool? isCurrentUser);
         
         Task<string> GetUserGender(string username);

         Task<AppUser> GetUserByPhotoId(int photoId);
         
    }
    
}