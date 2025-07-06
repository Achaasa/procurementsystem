using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace procurementsystem.models.Auth
{
    public class ResetPasswordDto
    {
        public string token { get; set; } = string.Empty; // Token for password reset
        public string newPassword { get; set; } = string.Empty; // New password to be set
    }
}