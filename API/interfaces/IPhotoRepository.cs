using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.interfaces
{
    public interface IPhotoRepository
    {
         
         Task<IEnumerable<PhotoForApprovalDTOs>> GetUnapprovedPhotos();

         Task<Photo> GetPhotoById(int photoId);

         void RemovePhoto(Photo photoForApproval);

    }
}