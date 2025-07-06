using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using procurementsystem.enums;

namespace procurementsystem.models.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserRole Role { get; set; } = UserRole.USER;
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty; // Optional: helpful for display
        public bool IsAuthenticated { get; set; } 
        public string? Message { get; set; } = string.Empty;
    }
}
