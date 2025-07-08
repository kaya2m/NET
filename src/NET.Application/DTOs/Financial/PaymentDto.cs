using NET.Domain.Enums;

namespace NET.Application.DTOs.Financial
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid ResidentId { get; set; }
        public string ResidentName { get; set; } = string.Empty;
        public Guid? BuildingUnitId { get; set; }
        public string? UnitNumber { get; set; }
        public string? BuildingName { get; set; }
        public Guid? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentType PaymentType { get; set; }
        public string PaymentTypeName { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public string? BankReference { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? LateFee { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}