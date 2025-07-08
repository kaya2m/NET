using NET.Domain.Entities.Common;
using NET.Domain.Enums;
using System;
using System.Collections.Generic;

namespace NET.Domain.Entities
{
    public class Role : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string? Permissions { get; set; } // JSON string of permissions

        // Navigation properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}