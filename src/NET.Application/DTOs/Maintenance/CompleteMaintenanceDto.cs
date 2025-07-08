using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Maintenance
{
    public class CompleteMaintenanceDto
    {
        [Required(ErrorMessage = "Completion notes are required")]
        [StringLength(1000, ErrorMessage = "Completion notes cannot exceed 1000 characters")]
        public string CompletionNotes { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Actual cost must be positive")]
        public decimal? ActualCost { get; set; }

        [StringLength(1000, ErrorMessage = "Work description cannot exceed 1000 characters")]
        public string? WorkDescription { get; set; }

        public DateTime? CompletedDate { get; set; }

        public bool NotifyResident { get; set; } = true;
    }
}
