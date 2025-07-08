using NET.Domain.Entities.Common;
using NET.Domain.Enums;
using System;
using System.Collections.Generic;

namespace NET.Domain.Entities
{
    public class Invoice : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public Guid BuildingUnitId { get; set; }
        public Guid? ResidentId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
        public PaymentType PaymentType { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? PdfPath { get; set; }

        // Navigation properties
        public virtual BuildingUnit BuildingUnit { get; set; } = null!;
        public virtual Resident? Resident { get; set; }
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}