using NET.Domain.Entities.Common;
using System;

namespace NET.Domain.Entities
{
    public class UserRole : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? ExpiryDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}