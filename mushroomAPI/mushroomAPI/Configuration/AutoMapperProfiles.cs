using AutoMapper;
using mushroomAPI.DTOs.Forum;
using mushroomAPI.DTOs.Mushroom.Coordinates;
using mushroomAPI.DTOs.Mushroom.Entries;
using mushroomAPI.DTOs.Mushroom.Location;
using mushroomAPI.DTOs.Mushroom.Predictions;
using mushroomAPI.DTOs.User;
using mushroomAPI.Entities;


namespace mushroomAPI.Configuration
{
    public class AutoMapperProfiles : Profile { 
        public AutoMapperProfiles() { 
            CreateMap<Mushroom, MushroomDTO>();
            CreateMap<Mushroom, UpsertMushroomDTO>();
            CreateMap<User, UserDTO>();
            CreateMap<User, UserRecognitionsDTO>();
            CreateMap<User, UserProfileDTO>();
            CreateMap<Recognition, RecognitionDTO>();
            CreateMap<Coordinates, CoordinatesDTO>();
            CreateMap<Coordinates, LocationDTO>();
            CreateMap<ForumPost, ForumPostDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.MushroomName, opt => opt.MapFrom(src => src.Mushroom.Name));
        }
    }
    

}
