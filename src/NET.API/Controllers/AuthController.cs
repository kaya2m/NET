using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET.Application.Common.Interfaces;
using NET.Application.DTOs.Authentication;
using NET.Application.DTOs.User;
using System.Threading.Tasks;

namespace NET.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthenticationService authenticationService,
            IJwtTokenService jwtTokenService,
            ILogger<AuthController> logger)
        {
            _authenticationService = authenticationService;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        /// <summary>
        /// User login
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authenticationService.AuthenticateAsync(loginDto.Email, loginDto.Password);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);
                    return Ok(new
                    {
                        Success = true,
                        Data = result.Data,
                        Message = "Login successful"
                    });
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault() ?? "Login failed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", loginDto.Email);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred during login"
                });
            }
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authenticationService.RefreshTokenAsync(
                    refreshTokenDto.AccessToken, 
                    refreshTokenDto.RefreshToken);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        Success = true,
                        Data = result.Data,
                        Message = "Token refreshed successfully"
                    });
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault() ?? "Token refresh failed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred during token refresh"
                });
            }
        }

        /// <summary>
        /// Revoke refresh token (Logout)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RevokeTokenDto revokeTokenDto)
        {
            try
            {
                var result = await _authenticationService.RevokeTokenAsync(revokeTokenDto.RefreshToken);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Logged out successfully"
                    });
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault() ?? "Logout failed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred during logout"
                });
            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get current user ID from token
                var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("nameid");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid user token"
                    });
                }

                var result = await _authenticationService.ChangePasswordAsync(
                    userId, 
                    changePasswordDto.CurrentPassword, 
                    changePasswordDto.NewPassword);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Password changed successfully"
                    });
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault() ?? "Password change failed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred during password change"
                });
            }
        }

        /// <summary>
        /// Request password reset
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authenticationService.ResetPasswordAsync(forgotPasswordDto.Email);

                // Always return success for security reasons (don't reveal if email exists)
                return Ok(new
                {
                    Success = true,
                    Message = "If the email exists, a password reset link has been sent"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset request for {Email}", forgotPasswordDto.Email);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred during password reset request"
                });
            }
        }

        /// <summary>
        /// Reset password with token
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authenticationService.ConfirmPasswordResetAsync(
                    resetPasswordDto.Email,
                    resetPasswordDto.Token,
                    resetPasswordDto.NewPassword);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Password reset successfully"
                    });
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault() ?? "Password reset failed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for {Email}", resetPasswordDto.Email);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred during password reset"
                });
            }
        }

        /// <summary>
        /// Confirm email address
        /// </summary>
        [HttpPost("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmEmailDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authenticationService.ConfirmEmailAsync(
                    confirmEmailDto.UserId,
                    confirmEmailDto.Token);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Email confirmed successfully"
                    });
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault() ?? "Email confirmation failed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email confirmation for user {UserId}", confirmEmailDto.UserId);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred during email confirmation"
                });
            }
        }

        /// <summary>
        /// Validate token
        /// </summary>
        [HttpPost("validate-token")]
        [AllowAnonymous]
        public IActionResult ValidateToken([FromBody] ValidateTokenDto validateTokenDto)
        {
            try
            {
                var isValid = _authenticationService.ValidateToken(validateTokenDto.Token);

                if (isValid)
                {
                    var claims = _jwtTokenService.GetAllClaims(validateTokenDto.Token);
                    var expirationDate = _jwtTokenService.GetTokenExpiration(validateTokenDto.Token);

                    return Ok(new
                    {
                        Success = true,
                        IsValid = true,
                        ExpiresAt = expirationDate,
                        Claims = claims.Select(c => new { c.Type, c.Value })
                    });
                }

                return Ok(new
                {
                    Success = true,
                    IsValid = false,
                    Message = "Token is invalid or expired"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token validation");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred during token validation"
                });
            }
        }

        /// <summary>
        /// Get current user info from token
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("nameid");
                var emailClaim = User.FindFirst("email");
                var nameClaim = User.FindFirst("name");
                var roleClaim = User.FindFirst("role");
                var tenantIdClaim = User.FindFirst("tenant_id");
                var buildingIdClaim = User.FindFirst("building_id");

                if (userIdClaim == null)
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Invalid token"
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        UserId = userIdClaim.Value,
                        Email = emailClaim?.Value,
                        Name = nameClaim?.Value,
                        Role = roleClaim?.Value,
                        TenantId = tenantIdClaim?.Value,
                        BuildingId = buildingIdClaim?.Value,
                        Claims = User.Claims.Select(c => new { c.Type, c.Value })
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user info");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while getting user info"
                });
            }
        }
    }
}

// DTOs that might be missing
namespace NET.Application.DTOs.Authentication
{
    public class RefreshTokenDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RevokeTokenDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ConfirmEmailDto
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
    }

    public class ValidateTokenDto
    {
        public string Token { get; set; } = string.Empty;
    }
}