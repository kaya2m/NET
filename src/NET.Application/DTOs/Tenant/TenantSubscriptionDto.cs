using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Tenant
{
    public class TenantSubscriptionDto
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public int MaxBuildings { get; set; }
        public int MaxUnits { get; set; }
        public decimal MonthlyFee { get; set; }
        public string SubscriptionType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsExpired { get; set; }
        public bool IsNearExpiry { get; set; }
        public int DaysUntilExpiry { get; set; }
        public bool AutoRenew { get; set; }
    }

    public class UpdateSubscriptionDto
    {
        [Required(ErrorMessage = "Subscription end date is required")]
        public DateTime SubscriptionEndDate { get; set; }

        [Range(1, 100, ErrorMessage = "Max buildings must be between 1 and 100")]
        public int MaxBuildings { get; set; }

        [Range(1, 10000, ErrorMessage = "Max units must be between 1 and 10000")]
        public int MaxUnits { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Monthly fee must be positive")]
        public decimal MonthlyFee { get; set; }

        [StringLength(50, ErrorMessage = "Subscription type cannot exceed 50 characters")]
        public string SubscriptionType { get; set; } = string.Empty;

        public bool AutoRenew { get; set; } = false;
    }
}
