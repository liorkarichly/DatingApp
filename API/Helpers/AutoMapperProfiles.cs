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
          
        }

    }

}