using System.Threading.Tasks;
using API.Helpers;
using API.interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {

        private readonly Cloudinary r_Cloudinary;

        //IOptions - I want to access to config file for to take the details of CloudinarySettings
        public PhotoService(IOptions<CloudinarySettings> i_ConfigOfCloudinary)
        {

            //Account - Initializes a new instance of the Account class. Parameterized constructor. cloudinary class
                //Importent to kipp save about order of details that wirte in config
            var cloudinaryAccount = new Account(

                    i_ConfigOfCloudinary.Value.CloudName,
                    i_ConfigOfCloudinary.Value.ApiKey,
                    i_ConfigOfCloudinary.Value.ApiSecret
            );

            r_Cloudinary = new Cloudinary(cloudinaryAccount);
        }


        //Upload photo to Cloudinary
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            
            var uploadResult = new ImageUploadResult();

            if(file.Length > 0)//Checking if have something in file
            {
                
                //The file is not empty and we want to use 
                //using for despose after the finish it
                //and we open the pipe with stream and after that we using ").
                using var stream = file.OpenReadStream();

                //ImageUploadParams - Initializes a new instance of the ImageUploadParams class.
                var uploadParams = new ImageUploadParams
                {

                    File = new FileDescription(file.FileName, stream),//Pass the file name and the stream
                    Transformation = new Transformation()//Creete the photo as photo square, and focus on face
                                .Height(500).Width(500).Crop("fill").Gravity("face")

                };

                //Upload the file to cloudinary
                uploadResult = await r_Cloudinary.UploadAsync(uploadParams);

            }

                return uploadResult;

        }

        //Delete photo from Cloudinary
        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            
            var deletePhotoParams = new DeletionParams(publicId);

            var resultDeletion =  await r_Cloudinary.DestroyAsync(deletePhotoParams);

            return resultDeletion;

        }
    }
}