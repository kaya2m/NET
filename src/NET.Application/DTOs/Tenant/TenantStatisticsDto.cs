using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Tenant
{
    public class TenantStatisticsDto
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public int TotalBuildings { get; set; }
        public int ActiveBuildings { get; set; }
        public int TotalUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public int VacantUnits { get; set; }
        public decimal OccupancyRate { get; set; }
        public int TotalResidents { get; set; }
        public int ActiveResidents { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public decimal NetIncome { get; set; }
        public int PendingMaintenanceRequests { get; set; }
        public int OverduePayments { get; set; }
        public decimal OverdueAmount { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public int DaysUntilExpiry { get; set; }
    }
}
