using NET.Domain.Entities.Common;
using NET.Domain.Enums;
using System;

namespace NET.Domain.Entities
{
    public class Expense : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public Guid BuildingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public ExpenseCategory Category { get; set; }
        public string? Vendor { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? ReceiptPath { get; set; }
        public bool IsRecurring { get; set; } = false;
        public string? RecurringPattern { get; set; }
        public bool IsApproved { get; set; } = false;
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Building Building { get; set; } = null!;
        public virtual User? ApprovedByUser { get; set; }
    }
}