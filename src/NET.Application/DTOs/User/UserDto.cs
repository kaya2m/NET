using NET.Domain.Enums;
using System;

namespace NET.Application.DTOs.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid? BuildingId { get; set; }
        public string? BuildingName { get; set; }
        public Guid? ResidentId { get; set; }
        public string? ResidentName { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public UserRoleEnum Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? PhotoUrl { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}