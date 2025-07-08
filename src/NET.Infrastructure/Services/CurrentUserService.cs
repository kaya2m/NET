using Microsoft.AspNetCore.Http;
using NET.Application.Common.Interfaces;
using NET.Domain.Entities;
using NET.Domain.Enums;
using System;
using System.Security.Claims;

namespace NET.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
                return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
            }
        }

        public string? UserName
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            }
        }

        public string? Email
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            }
        }

        public UserRoleEnum? Role
        {
            get
            {
                var roleClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);
                return roleClaim != null && Enum.TryParse<UserRoleEnum>(roleClaim.Value, out var role) ? role : null;
            }
        }

        public Guid? TenantId
        {
            get
            {
                var tenantClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id");
                return tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var tenantId) ? tenantId : null;
            }
        }

        public Guid? BuildingId
        {
            get
            {
                var buildingClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("building_id");
                return buildingClaim != null && Guid.TryParse(buildingClaim.Value, out var buildingId) ? buildingId : null;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            }
        }

        public bool IsInRole(UserRoleEnum role)
        {
            return Role == role;
        }

        public bool HasPermission(string permission)
        {
            // Check if user has specific permission
            var permissionClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("permission");
            return permissionClaim?.Value?.Contains(permission) ?? false;
        }
    }
}