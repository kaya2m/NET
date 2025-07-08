using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Building
{
    public class BuildingStatisticsDto
    {
        public int TotalUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public int VacantUnits { get; set; }
        public decimal OccupancyRate { get; set; }
        public decimal TotalMonthlyDues { get; set; }
        public int PendingMaintenanceRequests { get; set; }
        public decimal MonthlyExpenses { get; set; }
    }
}
