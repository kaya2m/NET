using NET.Domain.Entities.Common;
using NET.Domain.Enums;
using System;
using System.Collections.Generic;

namespace NET.Domain.Entities
{
    public class User : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public Guid? BuildingId { get; set; }
        public Guid? ResidentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public UserRoleEnum Role { get; set; } = UserRoleEnum.Resident;
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; } = false;
        public DateTime? LastLoginDate { get; set; }
        public string? PhotoUrl { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool TwoFactorEnabled { get; set; } = false;
        public string? SecurityStamp { get; set; }
        public int AccessFailedCount { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; } = true;

        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual Building? Building { get; set; }
        public virtual Resident? Resident { get; set; }
        public virtual ICollection<MaintenanceRequest> AssignedMaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public virtual ICollection<MaintenanceRequest> ApprovedMaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public virtual ICollection<Expense> ApprovedExpenses { get; set; } = new List<Expense>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}