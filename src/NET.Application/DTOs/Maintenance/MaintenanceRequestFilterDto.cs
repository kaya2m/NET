using NET.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Maintenance
{
    public class MaintenanceRequestFilterDto
    {
        public Guid? BuildingId { get; set; }
        public Guid? BuildingUnitId { get; set; }
        public Guid? ResidentId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public MaintenanceStatus? Status { get; set; }
        public Priority? Priority { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? RequiresApproval { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsOverdue { get; set; }
        public string? SearchText { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
    }
}
