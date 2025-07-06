using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using procurementsystem.enums;

namespace procurementsystem.models.User
{
    public class UpdateUserDto
    {

        public string? Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.USER; // Default role is User
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;
        public IFormFile? Image { get; set; } // The image file uploaded by the user
        public string? imageKey { get; set; }  // Cloudinary's public ID (optional)
        public string? imageUrl { get; set; }  // URL of the uploaded image from Cloudinary
    }
}