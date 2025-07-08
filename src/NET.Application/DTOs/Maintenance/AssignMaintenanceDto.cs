using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Maintenance
{
    public class AssignMaintenanceDto
    {
        [Required(ErrorMessage = "Assigned user ID is required")]
        public Guid AssignedToUserId { get; set; }

        public DateTime? ScheduledDate { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        public bool NotifyUser { get; set; } = true;
    }
}
