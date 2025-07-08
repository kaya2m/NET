using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Tenant
{
    public class TenantUsageDto
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public int BuildingsUsed { get; set; }
        public int MaxBuildings { get; set; }
        public decimal BuildingUsagePercentage { get; set; }
        public int UnitsUsed { get; set; }
        public int MaxUnits { get; set; }
        public decimal UnitUsagePercentage { get; set; }
        public int UsersUsed { get; set; }
        public int MaxUsers { get; set; }
        public decimal UserUsagePercentage { get; set; }
        public decimal StorageUsedMB { get; set; }
        public decimal MaxStorageMB { get; set; }
        public decimal StorageUsagePercentage { get; set; }
        public bool IsWithinLimits { get; set; }
        public bool IsNearingLimits { get; set; }
        public List<string> LimitWarnings { get; set; } = new();
        public List<string> LimitExceeded { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }
}
