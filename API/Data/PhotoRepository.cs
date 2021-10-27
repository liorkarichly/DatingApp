using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {

        private readonly DataContext r_DataContext;

        public PhotoRepository(DataContext i_DataContext)
        {

            this.r_DataContext = i_DataContext;

        }

        public async Task<Photo> GetPhotoById(int photoId)
        {
            
            return await r_DataContext.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(photo => photo.Id == photoId);

        }

        public async Task<IEnumerable<PhotoForApprovalDTOs>> GetUnapprovedPhotos()
        {
            
            return await r_DataContext.Photos
            .IgnoreQueryFilters()
            .Where(photo => photo.IsApproved == false)
            .Select(userWithApprovePhoto => new PhotoForApprovalDTOs
                    {

                        Id = userWithApprovePhoto.Id,
                        Username = userWithApprovePhoto.AppUser.UserName,
                        Url = userWithApprovePhoto.Url,
                        IsApproved = userWithApprovePhoto.IsApproved
                    })
            .ToListAsync();
            
        }

        public void RemovePhoto(Photo photoForApproval)
        {
            
            r_DataContext.Photos.Remove(photoForApproval);

        }

    }
    
}