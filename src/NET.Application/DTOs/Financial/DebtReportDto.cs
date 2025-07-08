using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Financial
{
    public class DebtReportDto
    {
        public List<ResidentDebtInfo> ResidentDebts { get; set; } = new();
        public decimal TotalDebt { get; set; }
        public int DebtorCount { get; set; }
        public decimal AverageDebt { get; set; }
        public int CriticalDebts { get; set; } // Over 90 days
        public int WarningDebts { get; set; } // 30-90 days
        public int NormalDebts { get; set; } // Under 30 days
        public DateTime GeneratedAt { get; set; }
        public decimal CollectionEfficiency { get; set; } // Percentage of debts recovered
    }

    public class ResidentDebtInfo
    {
        public Guid ResidentId { get; set; }
        public string ResidentName { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;
        public string BuildingName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal DebtAmount { get; set; }
        public int OverdueDays { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public int OverdueInvoiceCount { get; set; }
        public string DebtLevel { get; set; } = string.Empty; // Critical, Warning, Normal
        public decimal OldestDebtAmount { get; set; }
        public DateTime? OldestDebtDate { get; set; }
    }

}
