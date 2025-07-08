using NET.Application.DTOs.User;
using NET.Domain.Enums;
using System;

namespace NET.Application.DTOs.Authentication
{
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserInfoDto User { get; set; } = new();
    }

    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRoleEnum Role { get; set; }
        public Guid TenantId { get; set; }
        public Guid? BuildingId { get; set; }
    }
}