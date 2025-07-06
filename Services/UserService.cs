using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using procurementsystem.Data;
using procurementsystem.Entities;
using procurementsystem.IService;
using procurementsystem.models.User;
using procurementsystem.utils;

namespace procurementsystem.Services
{
    public class UserService : IUser
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        public UserService(ICloudinaryService cloudinaryService, ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }
        public async Task<UserDto> CreateUserAsync(CreateUserDto userRequest)
        {
            var existingUser = await _context.Users
                              .FirstOrDefaultAsync(u => 
                                  u.Email.ToLower() == userRequest.Email.ToLower());

            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            var hashedPassword = Bcrypt.HashPassword(userRequest.Password);
            userRequest.Password = hashedPassword;
            var newUser = _mapper.Map<User>(userRequest);
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.UpdatedAt = DateTime.UtcNow;
            newUser.delFlag = false;
            
            // Handle image upload if provided
            if (userRequest.Image != null)
            {
                try
                {
                    var (imageUrl, imageKey) = _cloudinaryService.UploadImage(userRequest.Image, "users");
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        newUser.imageUrl = imageUrl;
                        newUser.imageKey = imageKey;
                    }
                    else
                    {
                        // Log or throw an exception if upload fails
                        throw new InvalidOperationException("Image upload failed. Please check your Cloudinary configuration.");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Image upload failed: {ex.Message}");
                }
            }
            
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Use AutoMapper to map User to UserDto
            var userDto = _mapper.Map<UserDto>(newUser);
            return userDto;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return false;  // Return false if user is not found
            }

            user.delFlag = true; // Mark the user as deleted
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Where(u => !u.delFlag)  // Exclude deleted users
                .ToListAsync();
                
            if (users == null || !users.Any())
            {
                throw new KeyNotFoundException("No users found.");
            }
            
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return userDtos;
        }

        public async Task<UserDto> GetByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => 
                u.Email.ToLower() == email.ToLower() && !u.delFlag);
                
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId && !u.delFlag);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                throw new ArgumentException("Search term cannot be null or empty.");
            }

            var users = await _context.Users
                .Where(u => !u.delFlag && (u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm)))
                .ToListAsync();

            if (users == null || !users.Any())
            {
                throw new KeyNotFoundException("No users found matching the search term.");
            }

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return userDtos;
        }

        public async Task<bool> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;


            if (!string.IsNullOrWhiteSpace(updateUserDto.Name))
                user.Name = updateUserDto.Name;

            if (!string.IsNullOrWhiteSpace(updateUserDto.Email))
                user.Email = updateUserDto.Email;

            if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);

            user.Role = updateUserDto.Role;
            user.UpdatedAt = DateTime.UtcNow;

            if (updateUserDto.Image != null)
            {
                if (!string.IsNullOrEmpty(user.imageKey))
                {
                    _cloudinaryService.DeleteImage(user.imageKey);
                }

                var (imageUrl, imageKey) = _cloudinaryService.UploadImage(updateUserDto.Image, "users");
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    user.imageUrl = imageUrl;
                    user.imageKey = imageKey;
                }
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Use AutoMapper to map User to UserDto if you want to return the DTO
            var updatedUserDto = _mapper.Map<UserDto>(user);
            return updatedUserDto != null;


        }

    }
}