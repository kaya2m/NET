using NET.Domain.Entities.Common;
using System;
using System.Collections.Generic;

namespace NET.Domain.Entities
{
    public class Building : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int TotalUnits { get; set; }
        public int? ConstructionYear { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
        public decimal? TotalArea { get; set; }
        public int TotalFloors { get; set; }
        public bool HasElevator { get; set; } = false;
        public bool HasParking { get; set; } = false;
        public bool HasGarden { get; set; } = false;
        public bool HasPool { get; set; } = false;
        public bool HasGym { get; set; } = false;
        public string? ManagerName { get; set; }
        public string? ManagerPhone { get; set; }
        public string? ManagerEmail { get; set; }

        // Navigation properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual ICollection<BuildingUnit> BuildingUnits { get; set; } = new List<BuildingUnit>();
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}