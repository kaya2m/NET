using NET.Domain.Entities.Common;
using NET.Domain.Enums;
using System;

namespace NET.Domain.Entities
{
    public class MaintenanceRequest : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public Guid BuildingId { get; set; }
        public Guid? BuildingUnitId { get; set; }
        public Guid? ResidentId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Open;
        public Priority Priority { get; set; } = Priority.Medium;
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public string? WorkDescription { get; set; }
        public string? PhotoUrls { get; set; }
        public string? Notes { get; set; }
        public string? CompletionNotes { get; set; }
        public bool RequiresApproval { get; set; } = false;
        public bool IsApproved { get; set; } = false;
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        // Navigation properties
        public virtual Building Building { get; set; } = null!;
        public virtual BuildingUnit? BuildingUnit { get; set; }
        public virtual Resident? Resident { get; set; }
        public virtual User? AssignedToUser { get; set; }
        public virtual User? ApprovedByUser { get; set; }
    }
}