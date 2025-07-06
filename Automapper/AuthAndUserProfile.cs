using AutoMapper;
using procurementsystem.Entities;
using procurementsystem.models.Auth;
using procurementsystem.models.User;
using procurementsystem.enums;

namespace procurementsystem.Automapper
{
   
    public class AuthAndUserProfile : Profile
{
    public AuthAndUserProfile()
    {
        #region Auth Mappings

        // Mapping for LoginDto to Entity User (for authentication)
        CreateMap<LoginDto, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

        // Mapping for ResetPasswordDto to User (for password reset)
        CreateMap<ResetPasswordDto, User>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.newPassword));

        // Mapping for AuthResponseDto to User (response after successful login)
        CreateMap<User, AuthResponseDto>()
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => "Generated JWT Token"))  // Custom mapping for JWT Token, assume you have logic to generate the token
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(1)))  // Custom expiry time
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.IsAuthenticated, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Message, opt => opt.Ignore());

        // Mapping for ChangePasswordDto (No mapping to Entity since it's an update action)
        CreateMap<ChangePasswordDto, User>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.NewPassword));

        #endregion

        #region User Mappings

        // Mapping for CreateUserDto to User (for creating a new user)
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password)) // Password should be hashed outside AutoMapper
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.imageUrl, opt => opt.MapFrom(src => src.imageUrl))
            .ForMember(dest => dest.imageKey, opt => opt.MapFrom(src => src.imageKey))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Mapping for UpdateUserDto to User (for updating an existing user)
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.imageUrl, opt => opt.MapFrom(src => src.imageUrl))
            .ForMember(dest => dest.imageKey, opt => opt.MapFrom(src => src.imageKey))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin));

        // Mapping for User to UserDto (for sending user data to the front-end)
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin))
            .ForMember(dest => dest.imageUrl, opt => opt.MapFrom(src => src.imageUrl));

        #endregion
    }
}

}
