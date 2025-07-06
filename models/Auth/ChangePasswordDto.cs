using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace procurementsystem.models.Auth
{
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }= string.Empty;
        public string NewPassword { get; set; }= string.Empty;
        public string ConfirmNewPassword { get; set; }= string.Empty; // optional, for validation
    }
}
