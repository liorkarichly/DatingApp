using System.Threading.Tasks;
using API.Entities;
using System.Collections.Generic;
using API.DTOs;

namespace API.interfaces
{
    //Update is function of tracking status so its not TASK type
    public interface IUserRepository
    {
         
         void Update(AppUser user);

         Task<bool> SaveAllAsAsync();

         Task<IEnumerable<AppUser>> GetUsersAsync();

         Task<AppUser> GetUserByIdAsync(int id);

         Task<AppUser> GetUserByUsernameAsync(string username);

         Task<IEnumerable<MemberDTOs>> GetMembersAsync();

         Task<MemberDTOs> GetMemberAsync(string username);

    }
}