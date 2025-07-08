using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Maintenance
{
    public class MaintenanceStatisticsDto
    {
        public int TotalRequests { get; set; }
        public int OpenRequests { get; set; }
        public int InProgressRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int CancelledRequests { get; set; }
        public int OnHoldRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int HighPriorityRequests { get; set; }
        public int CriticalRequests { get; set; }
        public int EmergencyRequests { get; set; }
        public decimal AverageCompletionTimeHours { get; set; }
        public decimal TotalCost { get; set; }
        public decimal AverageCost { get; set; }
        public decimal PendingApprovalCost { get; set; }
        public int RequestsRequiringApproval { get; set; }
        public int OverdueRequests { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
