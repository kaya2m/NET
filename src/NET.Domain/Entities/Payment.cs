
using NET.Domain.Entities.Common;
using NET.Domain.Enums;
using System;

namespace NET.Domain.Entities
{
    public class Payment : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public Guid ResidentId { get; set; }
        public Guid? BuildingUnitId { get; set; }
        public Guid? InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public string? BankReference { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? LateFee { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Resident Resident { get; set; } = null!;
        public virtual BuildingUnit? BuildingUnit { get; set; }
        public virtual Invoice? Invoice { get; set; }
    }
}