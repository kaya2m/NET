using NET.Application.Common.Models;
using NET.Application.DTOs.User;
using NET.Domain.Entities;
using NET.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NET.Application.Services
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetByIdAsync(Guid id);
        Task<Result<UserDto>> GetByEmailAsync(string email);
        Task<Result<PagedResult<UserDto>>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<Result<List<UserDto>>> GetByRoleAsync(UserRoleEnum role);
        Task<Result<List<UserDto>>> GetByBuildingAsync(Guid buildingId);
        Task<Result<UserDto>> CreateAsync(CreateUserDto dto);
        Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserDto dto);
        Task<Result> DeleteAsync(Guid id);
        Task<Result> ActivateAsync(Guid id);
        Task<Result> DeactivateAsync(Guid id);

        // Password Management
        Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<Result> ResetPasswordAsync(Guid userId, string newPassword);
        Task<Result> SendPasswordResetEmailAsync(string email);

        // Authentication
        Task<Result<UserDto>> AuthenticateAsync(string email, string password);
        Task<Result> ConfirmEmailAsync(Guid userId, string token);
        Task<Result> ResendEmailConfirmationAsync(string email);

        // Role Management
        Task<Result> AssignRoleAsync(Guid userId, UserRoleEnum role);
        Task<Result> RemoveRoleAsync(Guid userId, UserRoleEnum role);
        Task<Result<List<UserRoleEnum>>> GetUserRolesAsync(Guid userId);

        // Two Factor Authentication
        Task<Result> EnableTwoFactorAsync(Guid userId);
        Task<Result> DisableTwoFactorAsync(Guid userId);

        // User Statistics
        Task<Result<UserStatisticsDto>> GetUserStatisticsAsync();
        Task<Result<List<UserDto>>> GetActiveUsersAsync();
        Task<Result<List<UserDto>>> GetInactiveUsersAsync();
    }
}
public class UserStatisticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int AdminUsers { get; set; }
    public int ResidentUsers { get; set; }
    public int MaintenanceUsers { get; set; }
    public int TodayLogins { get; set; }
    public DateTime LastLogin { get; set; }
}