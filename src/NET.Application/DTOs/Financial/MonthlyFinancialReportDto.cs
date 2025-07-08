using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Financial
{
    public class MonthlyFinancialReportDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
        public decimal CollectionRate { get; set; } // Percentage of payments collected
        public int TotalInvoices { get; set; }
        public int PaidInvoices { get; set; }
        public int PendingInvoices { get; set; }
        public int OverdueInvoices { get; set; }
        public decimal AverageInvoiceAmount { get; set; }
        public decimal TotalLateFees { get; set; }
        public List<MonthlyPaymentBreakdown> PaymentBreakdown { get; set; } = new();
        public List<MonthlyExpenseBreakdown> ExpenseBreakdown { get; set; } = new();
        public decimal PreviousMonthIncome { get; set; }
        public decimal IncomeGrowthPercentage { get; set; }
    }

    public class MonthlyPaymentBreakdown
    {
        public string PaymentType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class MonthlyExpenseBreakdown
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}
