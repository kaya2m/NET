using NET.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Financial
{
    public class UpdatePaymentDto
    {
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment date is required")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Payment type is required")]
        public PaymentType PaymentType { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters")]
        public string? PaymentMethod { get; set; }

        [StringLength(100, ErrorMessage = "Bank reference cannot exceed 100 characters")]
        public string? BankReference { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Late fee must be positive")]
        public decimal? LateFee { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }
    }
}