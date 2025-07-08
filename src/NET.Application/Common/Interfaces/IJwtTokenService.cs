using NET.Application.DTOs.Authentication;
using NET.Domain.Entities;
using NET.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace NET.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        // Token Generation
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        TokenInfo GenerateTokenPair(User user);

        // Token Validation
        bool ValidateToken(string token);
        ClaimsPrincipal? GetPrincipalFromToken(string token, bool validateLifetime = true);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

        // Token Information
        bool IsTokenExpired(string token);
        DateTime GetTokenExpiration(string token);
        string? GetClaimValue(string token, string claimType);

        // User Information from Token
        Guid? GetUserIdFromToken(string token);
        string? GetEmailFromToken(string token);
        UserRoleEnum? GetRoleFromToken(string token);
        Guid? GetTenantIdFromToken(string token);
        Guid? GetBuildingIdFromToken(string token);
        IEnumerable<Claim> GetAllClaims(string token);

        // Security Tokens
        string GenerateEmailConfirmationToken();
        string GeneratePasswordResetToken();
        string GenerateTwoFactorToken();
    }
}