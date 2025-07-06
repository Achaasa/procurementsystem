using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using procurementsystem.IService;
using procurementsystem.models.Auth;
using procurementsystem.models.User;

namespace procurementsystem.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Auth : ControllerBase
    {
        private readonly IUser _userService;
        private readonly IAuth _authService;
        private readonly ICloudinaryService _cloudinaryService;
        public Auth(IUser userService, IAuth authService, ICloudinaryService cloudinaryService)
        {
            _authService = authService;
            _userService = userService;
            _cloudinaryService = cloudinaryService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromForm] CreateUserDto registerDto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(registerDto);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); // HTTP 409 Conflict
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginRequest)
        {
            var resonse = await _authService.LoginAsync(loginRequest);
            if (resonse.IsAuthenticated == false)
            {
                return Unauthorized("Invalid email or password");
            }
            return Ok(resonse);
        }
        [HttpPut("update/{userId}")]
        public async Task<ActionResult<UserDto>> UpdateUser(Guid userId, [FromForm] UpdateUserDto updateUserDto)
        {
            try
            {
                if (updateUserDto.Image != null)
                {
                    var (imageUrl, imageKey) = _cloudinaryService.UploadImage(updateUserDto.Image, "users");
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        return BadRequest("Image upload failed.");
                    }

                    // Set the image URL and image key in the DTO
                    updateUserDto.imageUrl = imageUrl;
                    updateUserDto.imageKey = imageKey;
                }
                var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);
                if (!updatedUser)
                {
                    return NotFound("User not found");
                }
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize]
        [HttpPut("change-password/{userId}")]
        public async Task<ActionResult<bool>> ChangePassword(Guid userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
                if (!result)
                {
                    return NotFound("User not found or password change failed");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(email);

                return Ok("Password reset token sent to your email.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var token = resetPasswordDto.token.ToString();
            var newPassword = resetPasswordDto.newPassword.ToString();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("Token and new password are required.");
            }
            try
            {
                var result = await _authService.ConfirmResetPasswordAsync(resetPasswordDto);
                if (result)
                {
                    return Ok("Password has been reset successfully.");
                }
                else
                {
                    return BadRequest("Password reset failed.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers([FromQuery] string searchTerm)
        {
            try
            {
                var users = await _userService.SearchUsersAsync(searchTerm);
                if (users == null || !users.Any())
                {
                    return NotFound("No users found matching the search term.");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("users/all")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                if (users == null || !users.Any())
                {
                    return NotFound("No users found.");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("user/email/{email}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPut("delete/{userId}")]
        public async Task<ActionResult<bool>> DeleteUser(Guid userId)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(userId);
                if (!result)
                {
                    return NotFound("User not found or deletion failed.");
                }
                return Ok("User deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}