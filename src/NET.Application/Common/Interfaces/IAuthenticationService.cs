using NET.Application.Common.Models;
using NET.Application.DTOs.Authentication;
using System;
using System.Threading.Tasks;

namespace NET.Application.Common.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<AuthenticationResult>> AuthenticateAsync(string email, string password);
        Task<Result<AuthenticationResult>> RefreshTokenAsync(string token, string refreshToken);
        Task<Result> RevokeTokenAsync(string refreshToken);
        Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<Result> ResetPasswordAsync(string email);
        Task<Result> ConfirmPasswordResetAsync(string email, string token, string newPassword);
        Task<Result> ConfirmEmailAsync(Guid userId, string token);
        bool ValidateToken(string token);
    }
}