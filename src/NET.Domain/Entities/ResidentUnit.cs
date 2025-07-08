using NET.Domain.Entities.Common;
using System;

namespace NET.Domain.Entities
{
    public class ResidentUnit : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public Guid ResidentId { get; set; }
        public Guid BuildingUnitId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsOwner { get; set; } = false;
        public bool IsPrimaryResident { get; set; } = true;
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Resident Resident { get; set; } = null!;
        public virtual BuildingUnit BuildingUnit { get; set; } = null!;
    }
}