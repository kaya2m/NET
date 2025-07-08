using NET.Application.Common.Models;
using NET.Application.DTOs.Financial;
using NET.Domain.Enums;

public interface IFinancialService
{
    Task<Result<PaymentDto>> GetPaymentByIdAsync(Guid id);
    Task<Result<PagedResult<PaymentDto>>> GetAllPaymentsAsync(int page = 1, int pageSize = 10);
    Task<Result<List<PaymentDto>>> GetPaymentsByResidentAsync(Guid residentId);
    Task<Result<List<PaymentDto>>> GetPaymentsByUnitAsync(Guid unitId);
    Task<Result<List<PaymentDto>>> GetPaymentsByBuildingAsync(Guid buildingId);
    Task<Result<List<PaymentDto>>> GetPaymentsByTypeAsync(PaymentType paymentType);
    Task<Result<List<PaymentDto>>> GetPaymentsByStatusAsync(PaymentStatus status);
    Task<Result<List<PaymentDto>>> GetPaymentsByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<Result<PaymentDto>> CreatePaymentAsync(CreatePaymentDto dto);
    Task<Result<PaymentDto>> UpdatePaymentAsync(Guid id, CreatePaymentDto dto);
    Task<Result> DeletePaymentAsync(Guid id);
    Task<Result> MarkPaymentAsPaidAsync(Guid id);
    Task<Result> MarkPaymentAsOverdueAsync(Guid id);
    Task<Result> CancelPaymentAsync(Guid id, string reason);

    // Invoices
    Task<Result<InvoiceDto>> GetInvoiceByIdAsync(Guid id);
    Task<Result<PagedResult<InvoiceDto>>> GetAllInvoicesAsync(int page = 1, int pageSize = 10);
    Task<Result<List<InvoiceDto>>> GetInvoicesByUnitAsync(Guid unitId);
    Task<Result<List<InvoiceDto>>> GetInvoicesByResidentAsync(Guid residentId);
    Task<Result<List<InvoiceDto>>> GetInvoicesByBuildingAsync(Guid buildingId);
    Task<Result<List<InvoiceDto>>> GetOverdueInvoicesAsync();
    Task<Result<List<InvoiceDto>>> GetPendingInvoicesAsync();
    Task<Result<InvoiceDto>> CreateInvoiceAsync(CreateInvoiceDto dto);
    Task<Result<InvoiceDto>> UpdateInvoiceAsync(Guid id, UpdateInvoiceDto dto);
    Task<Result> DeleteInvoiceAsync(Guid id);
    Task<Result> SendInvoiceAsync(Guid id);
    Task<Result> MarkInvoiceAsPaidAsync(Guid id, decimal paidAmount);
    Task<Result<byte[]>> GenerateInvoicePdfAsync(Guid id);
    Task<Result> BulkCreateInvoicesAsync(List<CreateInvoiceDto> invoices);

    // Expenses
    Task<Result<ExpenseDto>> GetExpenseByIdAsync(Guid id);
    Task<Result<PagedResult<ExpenseDto>>> GetAllExpensesAsync(int page = 1, int pageSize = 10);
    Task<Result<List<ExpenseDto>>> GetExpensesByBuildingAsync(Guid buildingId);
    Task<Result<List<ExpenseDto>>> GetExpensesByCategoryAsync(ExpenseCategory category);
    Task<Result<List<ExpenseDto>>> GetExpensesByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<Result<List<ExpenseDto>>> GetPendingApprovalsAsync();
    Task<Result<ExpenseDto>> CreateExpenseAsync(CreateExpenseDto dto);
    Task<Result<ExpenseDto>> UpdateExpenseAsync(Guid id, CreateExpenseDto dto);
    Task<Result> DeleteExpenseAsync(Guid id);
    Task<Result> ApproveExpenseAsync(Guid id);
    Task<Result> RejectExpenseAsync(Guid id, string reason);
    Task<Result> BulkApproveExpensesAsync(List<Guid> expenseIds);

    // Financial Reports
    Task<Result<FinancialSummaryDto>> GetFinancialSummaryAsync(DateTime fromDate, DateTime toDate);
    Task<Result<MonthlyFinancialReportDto>> GetMonthlyReportAsync(int year, int month);
    Task<Result<List<PaymentDto>>> GetPaymentReportAsync(DateTime fromDate, DateTime toDate);
    Task<Result<List<ExpenseDto>>> GetExpenseReportAsync(DateTime fromDate, DateTime toDate);
    Task<Result<DebtReportDto>> GetDebtReportAsync();
    Task<Result<List<InvoiceDto>>> GetCollectionReportAsync(DateTime fromDate, DateTime toDate);

    // Dashboard Data
    Task<Result<decimal>> GetTotalIncomeAsync(DateTime fromDate, DateTime toDate);
    Task<Result<decimal>> GetTotalExpensesAsync(DateTime fromDate, DateTime toDate);
    Task<Result<decimal>> GetPendingPaymentsAmountAsync();
    Task<Result<decimal>> GetOverduePaymentsAmountAsync();
    Task<Result<int>> GetOverdueInvoiceCountAsync();
}