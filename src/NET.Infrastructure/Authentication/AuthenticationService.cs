using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NET.Application.Common.Interfaces;
using NET.Application.Common.Models;
using NET.Application.DTOs.Authentication;
using NET.Domain.Entities;
using NET.Domain.Enums;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IAuthenticationService = NET.Application.Common.Interfaces.IAuthenticationService;

namespace NET.Infrastructure.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly ITenantService _tenantService;
        private readonly IEmailService _emailService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthenticationService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<AuthenticationService> logger,
            ITenantService tenantService,
            IEmailService emailService,
            IJwtTokenService jwtTokenService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
            _tenantService = tenantService;
            _emailService = emailService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result<AuthenticationResult>> AuthenticateAsync(string email, string password)
        {
            try
            {
                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u =>
                    u.Email == email && !u.IsDeleted);

                if (user == null)
                {
                    _logger.LogWarning("Authentication failed: User not found for email {Email}", email);
                    return Result<AuthenticationResult>.Failure("Invalid email or password");
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Authentication failed: User account is inactive for email {Email}", email);
                    return Result<AuthenticationResult>.Failure("Account is inactive");
                }

                if (!VerifyPassword(password, user.PasswordHash))
                {
                    _logger.LogWarning("Authentication failed: Invalid password for email {Email}", email);
                    return Result<AuthenticationResult>.Failure("Invalid email or password");
                }

                if (user.EmailConfirmed == false)
                {
                    return Result<AuthenticationResult>.Failure("Email not confirmed");
                }

                user.LastLoginDate = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();

                var tokenInfo = _jwtTokenService.GenerateTokenPair(user);

                user.RefreshToken = tokenInfo.RefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _unitOfWork.SaveChangesAsync();

                var result = new AuthenticationResult
                {
                    IsSuccess = true,
                    Token = tokenInfo.AccessToken,
                    RefreshToken = tokenInfo.RefreshToken,
                    ExpiresAt = tokenInfo.ExpiresAt,
                    User = new UserInfoDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role,
                        TenantId = user.TenantId,
                        BuildingId = user.BuildingId
                    }
                };

                _logger.LogInformation("User {Email} authenticated successfully", email);
                return Result<AuthenticationResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for email {Email}", email);
                return Result<AuthenticationResult>.Failure("Authentication failed");
            }
        }

        public async Task<Result<AuthenticationResult>> RefreshTokenAsync(string token, string refreshToken)
        {
            try
            {
                var principal = _jwtTokenService.GetPrincipalFromExpiredToken(token);
                if (principal == null)
                {
                    return Result<AuthenticationResult>.Failure("Invalid token");
                }

                var email = _jwtTokenService.GetEmailFromToken(token);
                if (string.IsNullOrEmpty(email))
                {
                    return Result<AuthenticationResult>.Failure("Invalid token");
                }

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u =>
                    u.Email == email && !u.IsDeleted);

                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return Result<AuthenticationResult>.Failure("Invalid refresh token");
                }

                var tokenInfo = _jwtTokenService.GenerateTokenPair(user);

                user.RefreshToken = tokenInfo.RefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _unitOfWork.SaveChangesAsync();

                var result = new AuthenticationResult
                {
                    IsSuccess = true,
                    Token = tokenInfo.AccessToken,
                    RefreshToken = tokenInfo.RefreshToken,
                    ExpiresAt = tokenInfo.ExpiresAt,
                    User = new UserInfoDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role,
                        TenantId = user.TenantId,
                        BuildingId = user.BuildingId
                    }
                };

                return Result<AuthenticationResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return Result<AuthenticationResult>.Failure("Token refresh failed");
            }
        }

        public async Task<Result> RevokeTokenAsync(string refreshToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u =>
                    u.RefreshToken == refreshToken && !u.IsDeleted);

                if (user == null)
                {
                    return Result.Failure("Invalid refresh token");
                }

                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token revocation");
                return Result.Failure("Token revocation failed");
            }
        }

        public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u =>
                    u.Id == userId && !u.IsDeleted);

                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                if (!VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return Result.Failure("Current password is incorrect");
                }

                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Password changed successfully for user {UserId}", userId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return Result.Failure("Password change failed");
            }
        }

        public async Task<Result> ResetPasswordAsync(string email)
        {
            try
            {
                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u =>
                    u.Email == email && !u.IsDeleted);

                if (user == null)
                {
                    // Don't reveal if user exists or not
                    return Result.Success();
                }

                var resetToken = _jwtTokenService.GeneratePasswordResetToken();
                await _unitOfWork.SaveChangesAsync();

                await _emailService.SendPasswordResetEmailAsync(email, resetToken);

                _logger.LogInformation("Password reset email sent to {Email}", email);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email to {Email}", email);
                return Result.Failure("Password reset failed");
            }
        }

        public async Task<Result> ConfirmPasswordResetAsync(string email, string token, string newPassword)
        {
            try
            {
                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u =>
                    u.Email == email && !u.IsDeleted);

                if (user == null)
                {
                    return Result.Failure("Invalid or expired reset token");
                }

                user.PasswordHash = HashPassword(newPassword);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Password reset successfully for user {Email}", email);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming password reset for {Email}", email);
                return Result.Failure("Password reset confirmation failed");
            }
        }

        public async Task<Result> ConfirmEmailAsync(Guid userId, string token)
        {
            try
            {
                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u =>
                    u.Id == userId && !u.IsDeleted);

                if (user == null)
                {
                    return Result.Failure("Invalid confirmation token");
                }

                user.EmailConfirmed = true;
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Email confirmed successfully for user {UserId}", userId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email for user {UserId}", userId);
                return Result.Failure("Email confirmation failed");
            }
        }

        public bool ValidateToken(string token)
        {
            return _jwtTokenService.ValidateToken(token);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}       