using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET.Application.Common.Interfaces;
using NET.Application.DTOs.User;
using NET.Application.Services;
using NET.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace NET.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(
            IUserService userService,
            ICurrentUserService currentUserService)
            : base(currentUserService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _userService.GetAllAsync(page, pageSize);
            return HandlePagedResult(result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            var result = await _userService.GetByIdAsync(userId);
            return HandleResult(result);
        }

        ///// <summary>
        ///// Get users by role
        ///// </summary>
        //[HttpGet("by-role/{role}")]
        //[Authorize(Roles = "SuperAdmin,Admin,Manager")]
        //public async Task<IActionResult> GetUsersByRole(UserRoleEnum role, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        //{
        //    var result = await _userService.GetUsersByRoleAsync(role, page, pageSize);
        //    return HandlePagedResult(result);
        //}

        ///// <summary>
        ///// Get users by building
        ///// </summary>
        //[HttpGet("by-building/{buildingId}")]
        //[Authorize(Roles = "SuperAdmin,Admin,Manager")]
        //public async Task<IActionResult> GetUsersByBuilding(Guid buildingId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        //{
        //    var result = await _userService.GetUsersByBuildingAsync(buildingId, page, pageSize);
        //    return HandlePagedResult(result);
        //}

        /// <summary>
        /// Get active users
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> GetActiveUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userService.GetActiveUsersAsync();
            return Ok(result);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.CreateAsync(dto);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetUser), new { id = result.Data.Id }, new
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return HandleResult(result);
        }

        /// <summary>
        /// Update user
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateAsync(id, dto);
            return HandleResult(result);
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var result = await _userService.UpdateAsync(userId, dto);
            return HandleResult(result);
        }

        /// <summary>
        /// Delete user (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Activate user
        /// </summary>
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ActivateUser(Guid id)
        {
            var result = await _userService.ActivateAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Deactivate user
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeactivateUser(Guid id)
        {
            var result = await _userService.DeactivateAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Reset user password
        /// </summary>
        [HttpPost("{id}/reset-password")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ResetUserPassword(Guid id, [FromBody] ResetUserPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ResetPasswordAsync(id, dto.NewPassword);
            return HandleResult(result);
        }

        /// <summary>
        /// Assign role to user
        /// </summary>
        [HttpPost("{id}/roles")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AssignRole(Guid id, [FromBody] AssignRoleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.AssignRoleAsync(id, dto.Role);
            return HandleResult(result);
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        [HttpDelete("{id}/roles/{role}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> RemoveRole(Guid id, UserRoleEnum role)
        {
            var result = await _userService.RemoveRoleAsync(id, role);
            return HandleResult(result);
        }

        /// <summary>
        /// Assign user to building
        /// </summary>
        //[HttpPost("{id}/assign-building")]
        //[Authorize(Roles = "SuperAdmin,Admin")]
        //public async Task<IActionResult> AssignToBuilding(Guid id, [FromBody] AssignBuildingDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var result = await _userService.AssignToBuildingAsync(id, dto.BuildingId);
        //    return HandleResult(result);
        //}

        ///// <summary>
        ///// Remove user from building
        ///// </summary>
        //[HttpPost("{id}/remove-building")]
        //[Authorize(Roles = "SuperAdmin,Admin")]
        //public async Task<IActionResult> RemoveFromBuilding(Guid id)
        //{
        //    var result = await _userService.RemoveFromBuildingAsync(id);
        //    return HandleResult(result);
        //}

        ///// <summary>
        ///// Lock user account
        ///// </summary>
        //[HttpPost("{id}/lock")]
        //[Authorize(Roles = "SuperAdmin,Admin")]
        //public async Task<IActionResult> LockUser(Guid id, [FromBody] LockUserDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var result = await _userService.LockUserAsync(id, dto.LockoutEnd, dto.Reason);
        //    return HandleResult(result);
        //}

        ///// <summary>
        ///// Unlock user account
        ///// </summary>
        //[HttpPost("{id}/unlock")]
        //[Authorize(Roles = "SuperAdmin,Admin")]
        //public async Task<IActionResult> UnlockUser(Guid id)
        //{
        //    var result = await _userService.UnlockUserAsync(id);
        //    return HandleResult(result);
        //}

        /// <summary>
        /// Get user statistics
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> GetUserStatistics()
        {
            var result = await _userService.GetUserStatisticsAsync();
            return HandleResult(result);
        }

        ///// <summary>
        ///// Search users
        ///// </summary>
        //[HttpGet("search")]
        //[Authorize(Roles = "SuperAdmin,Admin,Manager")]
        //public async Task<IActionResult> SearchUsers([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        //{
        //    if (string.IsNullOrWhiteSpace(query))
        //        return BadRequest(new { Success = false, Message = "Search query is required" });

        //    var result = await _userService.SearchUsersAsync(query, page, pageSize);
        //    return HandlePagedResult(result);
        //}

        ///// <summary>
        ///// Get user login history
        ///// </summary>
        //[HttpGet("{id}/login-history")]
        //[Authorize(Roles = "SuperAdmin,Admin")]
        //public async Task<IActionResult> GetUserLoginHistory(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        //{
        //    var result = await _userService.GetUserLoginHistoryAsync(id, page, pageSize);
        //    return HandlePagedResult(result);
        //}

        ///// <summary>
        ///// Update user photo
        ///// </summary>
        //[HttpPost("{id}/photo")]
        //[Authorize]
        //public async Task<IActionResult> UpdateUserPhoto(Guid id, [FromBody] UpdatePhotoDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    // Check if user can update this photo (own profile or admin)
        //    var currentUserId = GetCurrentUserId();
        //    if (id != currentUserId && !IsInRole("SuperAdmin") && !IsInRole("Admin"))
        //    {
        //        return Forbid();
        //    }

        //    var result = await _userService.UpdatePhotoAsync(id, dto.PhotoUrl);
        //    return HandleResult(result);
        //}

        /// <summary>
        /// Enable two-factor authentication
        /// </summary>
        [HttpPost("{id}/enable-2fa")]
        [Authorize]
        public async Task<IActionResult> EnableTwoFactorAuth(Guid id)
        {
            // Check if user can enable 2FA for this account (own profile or admin)
            var currentUserId = GetCurrentUserId();
            if (id != currentUserId && !IsInRole("SuperAdmin") && !IsInRole("Admin"))
            {
                return Forbid();
            }

            var result = await _userService.EnableTwoFactorAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Disable two-factor authentication
        /// </summary>
        [HttpPost("{id}/disable-2fa")]
        [Authorize]
        public async Task<IActionResult> DisableTwoFactorAuth(Guid id)
        {
            // Check if user can disable 2FA for this account (own profile or admin)
            var currentUserId = GetCurrentUserId();
            if (id != currentUserId && !IsInRole("SuperAdmin") && !IsInRole("Admin"))
            {
                return Forbid();
            }

            var result = await _userService.DisableTwoFactorAsync(id);
            return HandleResult(result);
        }
    }
}

// Additional DTOs that might be missing
namespace NET.Application.DTOs.User
{
    public class ResetUserPasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    public class AssignRoleDto
    {
        public UserRoleEnum Role { get; set; }
    }

    public class AssignBuildingDto
    {
        public Guid BuildingId { get; set; }
    }

    public class LockUserDto
    {
        public DateTime? LockoutEnd { get; set; }
        public string? Reason { get; set; }
    }

    public class UpdatePhotoDto
    {
        public string PhotoUrl { get; set; } = string.Empty;
    }
}