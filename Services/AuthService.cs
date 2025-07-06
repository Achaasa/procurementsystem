using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using procurementsystem.Data;
using procurementsystem.IService;
using procurementsystem.models.Auth;
using procurementsystem.models.User;
using procurementsystem.utils;

namespace procurementsystem.Services
{
    public class AuthService : IAuth
    {
        private readonly IConfiguration _config;
        private readonly JwtSettings _jwtSettings;
        private readonly IUser _userService;
        private readonly IEmailService _emailService;
        private readonly ApplicationDBContext _context;

        private readonly IMapper _mapper;

        public AuthService(IConfiguration config, IOptions<JwtSettings> jwtOptions, IUser UserService, IEmailService emailService, IMapper mapper, ApplicationDBContext context)
        {
            _config = config;
            _jwtSettings = jwtOptions.Value;
            _userService = UserService;
            _emailService = emailService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Use the database context instead of the static list
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower() && !u.delFlag);

            if (user == null || string.IsNullOrEmpty(user.Password) || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return new AuthResponseDto
                {
                    IsAuthenticated = false,
                    Username = loginDto.Email,
                    Message = "Invalid email or password."
                };
            }

            var token = GenerateJwtToken(user.Id, user.Role.ToString());

            var response = _mapper.Map<AuthResponseDto>(user);
            response.Token = token;
            response.ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
            response.IsAuthenticated = true;
            response.Message = "Login successful";

            return response;
        }

        public Task LogoutAsync(Guid userId)
        {
            // Optional if you're not tracking token sessions server-side
            throw new NotImplementedException();
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users
                                      .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // Check old password matches
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.Password))
            {
                throw new BadHttpRequestException("Current password does not match.");
            }

            // Hash new password and store it
            user.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

            // Update user properties directly
            user.UpdatedAt = DateTime.UtcNow;
            user.LastLogin = DateTime.UtcNow; // Update last login time
                                              // If needed, update other fields, e.g., imageKey, imageUrl
            user.imageKey = user.imageKey;
            user.imageUrl = user.imageUrl;

            // Update the user in the database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // No need to map to UpdateUserDto, simply return true
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && !u.delFlag);

            if (user == null)
                return false;

            // 🔐 Generate secure token (GUID or JWT)
            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.TokenExpiry = DateTime.UtcNow.AddMinutes(15); // Token valid for 15 minutes

            // Update the user in the database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // 🔗 Build reset link (your frontend should handle it)
            string resetLink = $"https://yourfrontend.com/reset-password?token={token}";

            // 📧 Send reset link via email
            await _emailService.SendEmailAsync(user.Email, "Password Reset Request",
                $"Click the link to reset your password:<br><a href=\"{resetLink}\">{resetLink}</a><br>This link will expire in 15 minutes.");

            return true;
        }

        public Task<bool> VerifyTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<bool> ConfirmResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => 
                    u.PasswordResetToken == dto.token &&
                    u.TokenExpiry.HasValue &&
                    u.TokenExpiry > DateTime.UtcNow &&
                    !u.delFlag);

            if (user == null)
                return false;

            user.Password = Bcrypt.HashPassword(dto.newPassword);
            user.PasswordResetToken = null;
            user.TokenExpiry = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        private string GenerateJwtToken(Guid userId, string role)
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), // Set token expiration time
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,

                SigningCredentials = creds
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
