using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NET.Application.Common.Interfaces;
using NET.Application.DTOs.Authentication;
using NET.Domain.Entities;
using NET.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NET.Infrastructure.Authentication
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtTokenService> _logger;

        public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateAccessToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = GetSigningKey();

                var claims = BuildUserClaims(user);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
                    Issuer = _configuration["JwtSettings:Issuer"],
                    Audience = _configuration["JwtSettings:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogDebug("Access token generated for user {UserId}", user.Id);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating access token for user {UserId}", user.Id);
                throw;
            }
        }

        public string GenerateRefreshToken()
        {
            try
            {
                var randomNumber = new byte[64];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating refresh token");
                throw;
            }
        }

        public TokenInfo GenerateTokenPair(User user)
        {
            try
            {
                var accessToken = GenerateAccessToken(user);
                var refreshToken = GenerateRefreshToken();
                var expiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes());

                return new TokenInfo
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    TokenType = "Bearer"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token pair for user {UserId}", user.Id);
                throw;
            }
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = GetSigningKey();

                var validationParameters = GetTokenValidationParameters(key);

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Token validation failed");
                return false;
            }
        }

        public ClaimsPrincipal? GetPrincipalFromToken(string token, bool validateLifetime = true)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = GetSigningKey();

                var validationParameters = GetTokenValidationParameters(key, validateLifetime);

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token algorithm");
                }

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error getting principal from token");
                return null;
            }
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            return GetPrincipalFromToken(token, validateLifetime: false);
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error checking token expiration");
                return true; 
            }
        }

        public DateTime GetTokenExpiration(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                return jwtToken.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error getting token expiration");
                return DateTime.MinValue;
            }
        }

        public string? GetClaimValue(string token, string claimType)
        {
            try
            {
                var principal = GetPrincipalFromToken(token, validateLifetime: false);
                return principal?.FindFirst(claimType)?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error getting claim value from token");
                return null;
            }
        }

        public Guid? GetUserIdFromToken(string token)
        {
            var userIdString = GetClaimValue(token, ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }

        public string? GetEmailFromToken(string token)
        {
            return GetClaimValue(token, ClaimTypes.Email);
        }

        public UserRoleEnum? GetRoleFromToken(string token)
        {
            var roleString = GetClaimValue(token, ClaimTypes.Role);
            return Enum.TryParse<UserRoleEnum>(roleString, out var role) ? role : null;
        }

        public Guid? GetTenantIdFromToken(string token)
        {
            var tenantIdString = GetClaimValue(token, "tenant_id");
            return Guid.TryParse(tenantIdString, out var tenantId) ? tenantId : null;
        }

        public Guid? GetBuildingIdFromToken(string token)
        {
            var buildingIdString = GetClaimValue(token, "building_id");
            return Guid.TryParse(buildingIdString, out var buildingId) ? buildingId : null;
        }

        public IEnumerable<Claim> GetAllClaims(string token)
        {
            try
            {
                var principal = GetPrincipalFromToken(token, validateLifetime: false);
                return principal?.Claims ?? Enumerable.Empty<Claim>();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error getting all claims from token");
                return Enumerable.Empty<Claim>();
            }
        }

        public string GenerateEmailConfirmationToken()
        {
            try
            {
                var randomNumber = new byte[32];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating email confirmation token");
                throw;
            }
        }

        public string GeneratePasswordResetToken()
        {
            try
            {
                var randomNumber = new byte[32];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating password reset token");
                throw;
            }
        }

        public string GenerateTwoFactorToken()
        {
            try
            {
                var random = new Random();
                return random.Next(100000, 999999).ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating two-factor token");
                throw;
            }
        }

        private List<Claim> BuildUserClaims(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString()),
                new("tenant_id", user.TenantId.ToString()),
                new("jti", Guid.NewGuid().ToString()),
                new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            if (user.BuildingId.HasValue)
            {
                claims.Add(new Claim("building_id", user.BuildingId.Value.ToString()));
            }
            // Add custom claims based on user role
            switch (user.Role)
            {
                case UserRoleEnum.SuperAdmin:
                    claims.Add(new Claim("permission", "all"));
                    break;
                case UserRoleEnum.Admin:
                    claims.Add(new Claim("permission", "building_admin"));
                    break;
                case UserRoleEnum.Manager:
                    claims.Add(new Claim("permission", "building_manager"));
                    break;
                case UserRoleEnum.Maintenance:
                    claims.Add(new Claim("permission", "maintenance"));
                    break;
                case UserRoleEnum.Resident:
                    claims.Add(new Claim("permission", "resident"));
                    break;
            }

            return claims;
        }

        private byte[] GetSigningKey()
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is not configured");
            }

            return Encoding.UTF8.GetBytes(secretKey);
        }

        private TokenValidationParameters GetTokenValidationParameters(byte[] key, bool validateLifetime = true)
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        }

        private int GetTokenExpirationMinutes()
        {
            return int.TryParse(_configuration["JwtSettings:ExpirationInMinutes"], out var minutes) ? minutes : 60;
        }
    }
}