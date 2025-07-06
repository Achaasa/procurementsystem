using System;
using System.Threading.Tasks;
using procurementsystem.models.Auth;

namespace procurementsystem.IService
{
    public interface IAuth
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task LogoutAsync(Guid userId); // optional depending on how you manage sessions

        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
        Task<bool> ResetPasswordAsync(string email); // sends email with reset link/token
        public Task<bool> ConfirmResetPasswordAsync(ResetPasswordDto dto);
        Task<bool> VerifyTokenAsync(string token); // optional, for token validation (e.g. JWT)
    }
}
