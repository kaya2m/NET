using NET.Domain.Entities.Common;
using System;
using System.Collections.Generic;

namespace NET.Domain.Entities
{
    public class Resident : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
        public string? IdentityNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? Occupation { get; set; }
        public string? Notes { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsOwner { get; set; } = false;
        public DateTime? MoveInDate { get; set; }
        public DateTime? MoveOutDate { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        public virtual ICollection<ResidentUnit> ResidentUnits { get; set; } = new List<ResidentUnit>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    }
}