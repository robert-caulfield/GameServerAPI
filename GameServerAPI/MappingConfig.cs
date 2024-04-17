using GameServerAPI.Models;
using AutoMapper;
using GameServerAPI.Models.DTO.Auth;
using GameServerAPI.Controllers;
using GameServerAPI.Models.DTO.GameServer;
using GameServerAPI.Models.Token;

namespace GameServerAPI
{
    /// <summary>
    /// Configures object mapping for AutoMapper
    /// </summary>
    public class MappingConfig : Profile
    {
        /// <summary>
        /// Initializes instance of MappingConfig class.
        /// </summary>
        public MappingConfig() {
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();

            CreateMap<GameServer, GameServerRegisterDTO>().ReverseMap();
            CreateMap<GameServer, GameServerDetailsDTO>().ReverseMap();

            // Populate properties to GameServerDetailsDTO
            CreateMap<GameServerCache, GameServerDetailsDTO>();

            // Allows for easy deep copys
            CreateMap<GameServerCache, GameServerCache>();

            // Map registration dto to application user
            // Automatic mapping for normalized email, converting email to upper
            CreateMap<RegistrationRequestDTO, ApplicationUser>()
                .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()));
        }
    }
}
