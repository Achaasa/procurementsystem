using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using procurementsystem.models.User;

namespace procurementsystem.IService
{
    public interface IUser
    {
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(CreateUserDto newUser);
        Task<bool>  UpdateUserAsync(Guid userId, UpdateUserDto updatedUser);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm);
        Task<UserDto> GetByEmailAsync(string email);
    }
}