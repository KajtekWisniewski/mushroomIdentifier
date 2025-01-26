using AutoMapper;
using mushroomAPI.DTOs;
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
        }
    }
    

}
