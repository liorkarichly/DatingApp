using System;
using System.Linq;
using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {

            //Destination, Options -> go to collaction photos and take the main url --> insert to photoUrl in memberDTOs
            CreateMap<AppUser, MemberDTOs>()
            .ForMember(
                destination => destination.PhotoUrl
                , options => options.MapFrom(
                              source => source.Photos.FirstOrDefault(
                                                        mainPhoto => mainPhoto.IsMain).Url)
            )
            .ForMember(destination => destination.Age, 
                    optional => optional.MapFrom(source => source.DateOfBirth.CalculatorAge()));//Which property that we want to affect 

            CreateMap<Photo, PhotoDTOs>();
            CreateMap<MemberUpdateDTOs, AppUser>();
            CreateMap<RegisterDTOs, AppUser>();

            /*The MessageDTOs is so big and i want that maper help me to pass messages but i want to give some 
              configuration because there's a couple of properties or one property that
              we cannot get automatically to do for us, and that's for the use of it.  */
            CreateMap<Message, MessageDTOs>()
                                    .ForMember(destination => destination.SenderPhotoUrl,//Send a photo,URL
                                                 options => options.MapFrom(//Specify the source where we're mapping
                                                     source => source.Sender.Photos.FirstOrDefault(//From source.
                                                         user => user.IsMain//Take url of specific member
                                                    ).Url
                                    ))
                                    .ForMember(destination => destination.RecipientPhotoUrl,//Send a photo,URL
                                                 options => options.MapFrom(//Specify the source where we're mapping
                                                     source => source.Recipient.Photos.FirstOrDefault(//From source.
                                                         user => user.IsMain//Take url of specific member
                                                    ).Url
                                    ));
          
        }

    }

}