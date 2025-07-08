using NET.Domain.Enums;

namespace NET.Application.DTOs.Maintenance
{
    public class MaintenanceRequestDto
    {
        public Guid Id { get; set; }
        public Guid BuildingId { get; set; }
        public string BuildingName { get; set; } = string.Empty;
        public Guid? BuildingUnitId { get; set; }
        public string? UnitNumber { get; set; }
        public Guid? ResidentId { get; set; }
        public string? ResidentName { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public MaintenanceStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public Priority Priority { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public string? WorkDescription { get; set; }
        public string? PhotoUrls { get; set; }
        public string? Notes { get; set; }
        public string? CompletionNotes { get; set; }
        public bool RequiresApproval { get; set; }
        public bool IsApproved { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}