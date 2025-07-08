using NET.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace NET.Application.DTOs.Financial
{
    public class CreateInvoiceDto
    {
        [Required(ErrorMessage = "Building unit ID is required")]
        public Guid BuildingUnitId { get; set; }

        public Guid? ResidentId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tax amount must be positive")]
        public decimal? TaxAmount { get; set; }

        [Required(ErrorMessage = "Invoice date is required")]
        public DateTime InvoiceDate { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Payment type is required")]
        public PaymentType PaymentType { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }
    }
}