using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace API.interfaces
{
    public interface IPhotoService
    {
         
         //ImageUploadResult - Message from Cloudinary ,result about the operation,Parsed response after upload of the image resource.
         //IFormFile - Represents a file sent with the HttpRequest.
           Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

            //DeletionResult - Parsed result of asset deletion.
            //publicId - GSetting the public id from photo then we result from the service when we uploaded
           Task<DeletionResult> DeletePhotoAsync(string publicId);

    }
}