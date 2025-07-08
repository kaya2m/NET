using NET.Domain.Entities.Common;
using NET.Domain.Enums;
using System;
using System.Collections.Generic;

namespace NET.Domain.Entities
{
    public class BuildingUnit : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public Guid BuildingId { get; set; }
        public string UnitNumber { get; set; } = string.Empty;
        public int FloorNumber { get; set; }
        public UnitType UnitType { get; set; }
        public decimal? AreaSqm { get; set; }
        public bool IsOccupied { get; set; } = false;
        public decimal? MonthlyDues { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public bool HasBalcony { get; set; } = false;
        public string? Description { get; set; }
        public decimal? RentAmount { get; set; }
        public DateTime? LastOccupiedDate { get; set; }

        // Navigation properties
        public virtual Building Building { get; set; } = null!;
        public virtual ICollection<ResidentUnit> ResidentUnits { get; set; } = new List<ResidentUnit>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    }
}