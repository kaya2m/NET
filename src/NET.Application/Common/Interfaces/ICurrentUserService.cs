using NET.Domain.Enums;
using System;

namespace NET.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        UserRoleEnum? Role { get; }
        Guid? TenantId { get; }
        Guid? BuildingId { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(UserRoleEnum role);
        bool HasPermission(string permission);
    }
}