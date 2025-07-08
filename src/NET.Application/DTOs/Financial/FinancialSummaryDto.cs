using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.Financial
{
    public class FinancialSummaryDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
        public decimal PendingPayments { get; set; }
        public decimal OverduePayments { get; set; }
        public decimal PaidPayments { get; set; }
        public int TotalTransactions { get; set; }
        public int PendingInvoices { get; set; }
        public int OverdueInvoices { get; set; }
        public decimal AveragePaymentAmount { get; set; }
        public decimal CollectionRate { get; set; } // Percentage of invoices paid
        public List<PaymentTypeSummary> PaymentsByType { get; set; } = new();
        public List<ExpenseCategorySummary> ExpensesByCategory { get; set; } = new();
    }

    public class PaymentTypeSummary
    {
        public string PaymentType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ExpenseCategorySummary
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}
