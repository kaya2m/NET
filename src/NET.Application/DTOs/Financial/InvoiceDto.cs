using NET.Domain.Enums;

namespace NET.Application.DTOs.Financial
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public Guid BuildingUnitId { get; set; }
        public string UnitNumber { get; set; } = string.Empty;
        public string BuildingName { get; set; } = string.Empty;
        public Guid? ResidentId { get; set; }
        public string? ResidentName { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public PaymentType PaymentType { get; set; }
        public string PaymentTypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? PdfPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsOverdue => Status != InvoiceStatus.Paid && DueDate < DateTime.Now;
        public decimal RemainingAmount => TotalAmount - (PaidAmount ?? 0);
        public int DaysOverdue => IsOverdue ? (DateTime.Now - DueDate).Days : 0;
    }
}