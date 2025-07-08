using NET.Domain.Enums;

namespace NET.Application.DTOs.Financial
{
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public Guid BuildingId { get; set; }
        public string BuildingName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public ExpenseCategory Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Vendor { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? ReceiptPath { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurringPattern { get; set; }
        public bool IsApproved { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}