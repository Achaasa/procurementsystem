using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using procurementsystem.enums;

namespace procurementsystem.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.USER; // Default role is User
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; } = DateTime.UtcNow;
        public string? imageKey { get; set; }  // Optional image key for user profile picture
        public string? imageUrl { get; set; }  // Optional image url for user profile picture
        public string? PasswordResetToken { get; set; } // Token for password reset
        public DateTime? TokenExpiry { get; set; } // Expiry time
        public bool delFlag { get; set; } = false; // Flag to indicate if the user is deleted
        public ICollection<ProcurementItem> CreatedProcurementItems { get; set; } = new List<ProcurementItem>();
        public ICollection<ProcurementItem> UpdatedProcurementItems { get; set; } = new List<ProcurementItem>();

        // New Navigation Property to access all the history updates this user has done.
        public ICollection<ProcurementHistory> UpdatedProcurementHistories { get; set; } = new List<ProcurementHistory>();
    }
}
