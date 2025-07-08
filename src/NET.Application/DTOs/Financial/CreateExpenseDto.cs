using NET.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace NET.Application.DTOs.Financial
{
    public class CreateExpenseDto
    {
        [Required(ErrorMessage = "Building ID is required")]
        public Guid BuildingId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Expense date is required")]
        public DateTime ExpenseDate { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public ExpenseCategory Category { get; set; }

        [StringLength(255, ErrorMessage = "Vendor cannot exceed 255 characters")]
        public string? Vendor { get; set; }

        [StringLength(100, ErrorMessage = "Invoice number cannot exceed 100 characters")]
        public string? InvoiceNumber { get; set; }

        public bool IsRecurring { get; set; } = false;

        [StringLength(100, ErrorMessage = "Recurring pattern cannot exceed 100 characters")]
        public string? RecurringPattern { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }
    }
}