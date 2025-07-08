using AutoMapper;
using NET.Application.Common.Interfaces;
using NET.Application.Common.Models;
using NET.Application.DTOs.Financial;
using NET.Domain.Entities;
using NET.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NET.Application.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantService _tenantService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;

        public FinancialService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITenantService tenantService,
            ICurrentUserService currentUserService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantService = tenantService;
            _currentUserService = currentUserService;
            _emailService = emailService;
        }

        #region Payment Methods

        public async Task<Result<PaymentDto>> GetPaymentByIdAsync(Guid id)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetFirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
                if (payment == null)
                {
                    return Result<PaymentDto>.Failure("Payment not found");
                }

                var paymentDto = _mapper.Map<PaymentDto>(payment);
                return Result<PaymentDto>.Success(paymentDto);
            }
            catch (Exception ex)
            {
                return Result<PaymentDto>.Failure($"Error retrieving payment: {ex.Message}");
            }
        }

        public async Task<Result<PagedResult<PaymentDto>>> GetAllPaymentsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetPagedAsync(page, pageSize, p => !p.IsDeleted);
                var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments.Data);
                var pagedResult = new PagedResult<PaymentDto>(paymentDtos, page, pageSize, payments.TotalCount);

                return Result<PagedResult<PaymentDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return Result<PagedResult<PaymentDto>>.Failure($"Error retrieving payments: {ex.Message}");
            }
        }

        public async Task<Result<PaymentDto>> CreatePaymentAsync(CreatePaymentDto dto)
        {
            try
            {
                // Validate resident exists
                var residentExists = await _unitOfWork.Residents.ExistsAsync(r => r.Id == dto.ResidentId && !r.IsDeleted);
                if (!residentExists)
                {
                    return Result<PaymentDto>.Failure("Resident not found");
                }

                // Validate unit if provided
                if (dto.BuildingUnitId.HasValue)
                {
                    var unitExists = await _unitOfWork.BuildingUnits.ExistsAsync(u => u.Id == dto.BuildingUnitId && !u.IsDeleted);
                    if (!unitExists)
                    {
                        return Result<PaymentDto>.Failure("Building unit not found");
                    }
                }

                var payment = _mapper.Map<Payment>(dto);
                payment.TenantId = _tenantService.GetCurrentTenantId();
                payment.CreatedBy = _currentUserService.UserName;

                // Generate reference number if not provided
                if (string.IsNullOrEmpty(payment.ReferenceNumber))
                {
                    payment.ReferenceNumber = await GeneratePaymentReferenceAsync();
                }

                var createdPayment = await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                var paymentDto = _mapper.Map<PaymentDto>(createdPayment);
                return Result<PaymentDto>.Success(paymentDto, "Payment created successfully");
            }
            catch (Exception ex)
            {
                return Result<PaymentDto>.Failure($"Error creating payment: {ex.Message}");
            }
        }

        public async Task<Result> MarkPaymentAsPaidAsync(Guid id)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetByIdAsync(id);
                if (payment == null || payment.IsDeleted)
                {
                    return Result.Failure("Payment not found");
                }

                payment.Status = PaymentStatus.Paid;
                payment.PaymentDate = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;
                payment.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Payments.UpdateAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Payment marked as paid successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error marking payment as paid: {ex.Message}");
            }
        }

        public async Task<Result<List<PaymentDto>>> GetPaymentsByResidentAsync(Guid residentId)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAsync(p => p.ResidentId == residentId && !p.IsDeleted);
                var paymentDtos = _mapper.Map<List<PaymentDto>>(payments);
                return Result<List<PaymentDto>>.Success(paymentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<PaymentDto>>.Failure($"Error retrieving payments by resident: {ex.Message}");
            }
        }

        #endregion

        #region Invoice Methods

        public async Task<Result<InvoiceDto>> GetInvoiceByIdAsync(Guid id)
        {
            try
            {
                var invoice = await _unitOfWork.Invoices.GetFirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
                if (invoice == null)
                {
                    return Result<InvoiceDto>.Failure("Invoice not found");
                }

                var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
                return Result<InvoiceDto>.Success(invoiceDto);
            }
            catch (Exception ex)
            {
                return Result<InvoiceDto>.Failure($"Error retrieving invoice: {ex.Message}");
            }
        }

        public async Task<Result<InvoiceDto>> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            try
            {
                // Validate unit exists
                var unitExists = await _unitOfWork.BuildingUnits.ExistsAsync(u => u.Id == dto.BuildingUnitId && !u.IsDeleted);
                if (!unitExists)
                {
                    return Result<InvoiceDto>.Failure("Building unit not found");
                }

                var invoice = _mapper.Map<Invoice>(dto);
                invoice.TenantId = _tenantService.GetCurrentTenantId();
                invoice.CreatedBy = _currentUserService.UserName;
                invoice.InvoiceNumber = await GenerateInvoiceNumberAsync();

                var createdInvoice = await _unitOfWork.Invoices.AddAsync(invoice);
                await _unitOfWork.SaveChangesAsync();

                var invoiceDto = _mapper.Map<InvoiceDto>(createdInvoice);
                return Result<InvoiceDto>.Success(invoiceDto, "Invoice created successfully");
            }
            catch (Exception ex)
            {
                return Result<InvoiceDto>.Failure($"Error creating invoice: {ex.Message}");
            }
        }

        public async Task<Result<List<InvoiceDto>>> GetOverdueInvoicesAsync()
        {
            try
            {
                var invoices = await _unitOfWork.Invoices.GetAsync(i =>
                    i.DueDate < DateTime.UtcNow &&
                    i.Status != InvoiceStatus.Paid &&
                    i.Status != InvoiceStatus.Cancelled &&
                    !i.IsDeleted);

                var invoiceDtos = _mapper.Map<List<InvoiceDto>>(invoices);
                return Result<List<InvoiceDto>>.Success(invoiceDtos);
            }
            catch (Exception ex)
            {
                return Result<List<InvoiceDto>>.Failure($"Error retrieving overdue invoices: {ex.Message}");
            }
        }

        #endregion

        #region Expense Methods

        public async Task<Result<ExpenseDto>> GetExpenseByIdAsync(Guid id)
        {
            try
            {
                var expense = await _unitOfWork.Expenses.GetFirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
                if (expense == null)
                {
                    return Result<ExpenseDto>.Failure("Expense not found");
                }

                var expenseDto = _mapper.Map<ExpenseDto>(expense);
                return Result<ExpenseDto>.Success(expenseDto);
            }
            catch (Exception ex)
            {
                return Result<ExpenseDto>.Failure($"Error retrieving expense: {ex.Message}");
            }
        }

        public async Task<Result<ExpenseDto>> CreateExpenseAsync(CreateExpenseDto dto)
        {
            try
            {
                // Validate building exists
                var buildingExists = await _unitOfWork.Buildings.ExistsAsync(b => b.Id == dto.BuildingId && !b.IsDeleted);
                if (!buildingExists)
                {
                    return Result<ExpenseDto>.Failure("Building not found");
                }

                var expense = _mapper.Map<Expense>(dto);
                expense.TenantId = _tenantService.GetCurrentTenantId();
                expense.CreatedBy = _currentUserService.UserName;

                var createdExpense = await _unitOfWork.Expenses.AddAsync(expense);
                await _unitOfWork.SaveChangesAsync();

                var expenseDto = _mapper.Map<ExpenseDto>(createdExpense);
                return Result<ExpenseDto>.Success(expenseDto, "Expense created successfully");
            }
            catch (Exception ex)
            {
                return Result<ExpenseDto>.Failure($"Error creating expense: {ex.Message}");
            }
        }

        public async Task<Result> ApproveExpenseAsync(Guid id)
        {
            try
            {
                var expense = await _unitOfWork.Expenses.GetByIdAsync(id);
                if (expense == null || expense.IsDeleted)
                {
                    return Result.Failure("Expense not found");
                }

                expense.IsApproved = true;
                expense.ApprovedBy = _currentUserService.UserId;
                expense.ApprovedDate = DateTime.UtcNow;
                expense.UpdatedAt = DateTime.UtcNow;
                expense.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Expenses.UpdateAsync(expense);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Expense approved successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error approving expense: {ex.Message}");
            }
        }

        #endregion

        #region Financial Reports

        public async Task<Result<FinancialSummaryDto>> GetFinancialSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAsync(p =>
                    p.PaymentDate >= fromDate &&
                    p.PaymentDate <= toDate &&
                    p.Status == PaymentStatus.Paid &&
                    !p.IsDeleted);

                var expenses = await _unitOfWork.Expenses.GetAsync(e =>
                    e.ExpenseDate >= fromDate &&
                    e.ExpenseDate <= toDate &&
                    e.IsApproved &&
                    !e.IsDeleted);

                var pendingPayments = await _unitOfWork.Payments.GetAsync(p =>
                    p.Status == PaymentStatus.Pending && !p.IsDeleted);

                var overduePayments = await _unitOfWork.Payments.GetAsync(p =>
                    p.Status == PaymentStatus.Overdue && !p.IsDeleted);

                var totalIncome = payments.Sum(p => p.Amount);
                var totalExpenses = expenses.Sum(e => e.Amount);

                var summary = new FinancialSummaryDto
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    TotalIncome = totalIncome,
                    TotalExpenses = totalExpenses,
                    NetIncome = totalIncome - totalExpenses,
                    PendingPayments = pendingPayments.Sum(p => p.Amount),
                    OverduePayments = overduePayments.Sum(p => p.Amount),
                    PaidPayments = totalIncome,
                    TotalTransactions = payments.Count() + expenses.Count(),
                    AveragePaymentAmount = payments.Any() ? payments.Average(p => p.Amount) : 0,
                    PaymentsByType = payments.GroupBy(p => p.PaymentType)
                        .Select(g => new PaymentTypeSummary
                        {
                            PaymentType = g.Key.ToString(),
                            Amount = g.Sum(p => p.Amount),
                            Count = g.Count(),
                            Percentage = totalIncome > 0 ? (g.Sum(p => p.Amount) / totalIncome) * 100 : 0
                        }).ToList(),
                    ExpensesByCategory = expenses.GroupBy(e => e.Category)
                        .Select(g => new ExpenseCategorySummary
                        {
                            Category = g.Key.ToString(),
                            Amount = g.Sum(e => e.Amount),
                            Count = g.Count(),
                            Percentage = totalExpenses > 0 ? (g.Sum(e => e.Amount) / totalExpenses) * 100 : 0
                        }).ToList()
                };

                return Result<FinancialSummaryDto>.Success(summary);
            }
            catch (Exception ex)
            {
                return Result<FinancialSummaryDto>.Failure($"Error generating financial summary: {ex.Message}");
            }
        }

        public async Task<Result<DebtReportDto>> GetDebtReportAsync()
        {
            try
            {
                var overdueInvoices = await _unitOfWork.Invoices.GetAsync(i =>
                    i.DueDate < DateTime.UtcNow &&
                    i.Status != InvoiceStatus.Paid &&
                    i.Status != InvoiceStatus.Cancelled &&
                    !i.IsDeleted);

                var residentDebts = overdueInvoices
                    .GroupBy(i => new { i.ResidentId, i.BuildingUnit.UnitNumber, i.BuildingUnit.Building.Name })
                    .Select(g => new ResidentDebtInfo
                    {
                        ResidentId = g.Key.ResidentId ?? Guid.Empty,
                        ResidentName = g.First().Resident?.FirstName + " " + g.First().Resident?.LastName ?? "Unknown",
                        UnitNumber = g.Key.UnitNumber,
                        BuildingName = g.Key.Name,
                        Email = g.First().Resident?.Email ?? "",
                        Phone = g.First().Resident?.Phone ?? "",
                        DebtAmount = g.Sum(i => i.TotalAmount - (i.PaidAmount ?? 0)),
                        OverdueDays = (DateTime.UtcNow - g.Max(i => i.DueDate)).Days,
                        OverdueInvoiceCount = g.Count(),
                        DebtLevel = (DateTime.UtcNow - g.Max(i => i.DueDate)).Days > 90 ? "Critical" :
                                   (DateTime.UtcNow - g.Max(i => i.DueDate)).Days > 30 ? "Warning" : "Normal"
                    }).ToList();

                var totalDebt = residentDebts.Sum(r => r.DebtAmount);

                var report = new DebtReportDto
                {
                    ResidentDebts = residentDebts,
                    TotalDebt = totalDebt,
                    DebtorCount = residentDebts.Count,
                    AverageDebt = residentDebts.Any() ? residentDebts.Average(r => r.DebtAmount) : 0,
                    CriticalDebts = residentDebts.Count(r => r.DebtLevel == "Critical"),
                    WarningDebts = residentDebts.Count(r => r.DebtLevel == "Warning"),
                    GeneratedAt = DateTime.UtcNow
                };

                return Result<DebtReportDto>.Success(report);
            }
            catch (Exception ex)
            {
                return Result<DebtReportDto>.Failure($"Error generating debt report: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        private async Task<string> GeneratePaymentReferenceAsync()
        {
            var lastPayment = await _unitOfWork.Payments.GetAsync(p => !p.IsDeleted);
            var count = lastPayment.Count() + 1;
            return $"PAY-{DateTime.UtcNow:yyyyMM}-{count:D6}";
        }

        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var lastInvoice = await _unitOfWork.Invoices.GetAsync(i => !i.IsDeleted);
            var count = lastInvoice.Count() + 1;
            return $"INV-{DateTime.UtcNow:yyyyMM}-{count:D6}";
        }

        #endregion

        // Implement remaining methods with similar patterns...
        public async Task<Result<PagedResult<InvoiceDto>>> GetAllInvoicesAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var invoices = await _unitOfWork.Invoices.GetPagedAsync(page, pageSize, i => !i.IsDeleted);
                var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices.Data);
                var pagedResult = new PagedResult<InvoiceDto>(invoiceDtos, page, pageSize, invoices.TotalCount);

                return Result<PagedResult<InvoiceDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return Result<PagedResult<InvoiceDto>>.Failure($"Error retrieving invoices: {ex.Message}");
            }
        }

        public async Task<Result<PagedResult<ExpenseDto>>> GetAllExpensesAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var expenses = await _unitOfWork.Expenses.GetPagedAsync(page, pageSize, e => !e.IsDeleted);
                var expenseDtos = _mapper.Map<IEnumerable<ExpenseDto>>(expenses.Data);
                var pagedResult = new PagedResult<ExpenseDto>(expenseDtos, page, pageSize, expenses.TotalCount);

                return Result<PagedResult<ExpenseDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return Result<PagedResult<ExpenseDto>>.Failure($"Error retrieving expenses: {ex.Message}");
            }
        }

        // Add remaining method implementations...
        public async Task<Result<List<PaymentDto>>> GetPaymentsByUnitAsync(Guid unitId)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAsync(p => p.BuildingUnitId == unitId && !p.IsDeleted);
                var paymentDtos = _mapper.Map<List<PaymentDto>>(payments);
                return Result<List<PaymentDto>>.Success(paymentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<PaymentDto>>.Failure($"Error retrieving payments by unit: {ex.Message}");
            }
        }

        public async Task<Result<List<PaymentDto>>> GetPaymentsByBuildingAsync(Guid buildingId)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAsync(p => p.BuildingUnit.BuildingId == buildingId && !p.IsDeleted);
                var paymentDtos = _mapper.Map<List<PaymentDto>>(payments);
                return Result<List<PaymentDto>>.Success(paymentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<PaymentDto>>.Failure($"Error retrieving payments by building: {ex.Message}");
            }
        }

        public async Task<Result<List<PaymentDto>>> GetPaymentsByTypeAsync(PaymentType paymentType)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAsync(p => p.PaymentType == paymentType && !p.IsDeleted);
                var paymentDtos = _mapper.Map<List<PaymentDto>>(payments);
                return Result<List<PaymentDto>>.Success(paymentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<PaymentDto>>.Failure($"Error retrieving payments by type: {ex.Message}");
            }
        }

        public async Task<Result<List<PaymentDto>>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAsync(p => p.Status == status && !p.IsDeleted);
                var paymentDtos = _mapper.Map<List<PaymentDto>>(payments);
                return Result<List<PaymentDto>>.Success(paymentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<PaymentDto>>.Failure($"Error retrieving payments by status: {ex.Message}");
            }
        }

        public async Task<Result<List<PaymentDto>>> GetPaymentsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAsync(p =>
                    p.PaymentDate >= fromDate && p.PaymentDate <= toDate && !p.IsDeleted);
                var paymentDtos = _mapper.Map<List<PaymentDto>>(payments);
                return Result<List<PaymentDto>>.Success(paymentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<PaymentDto>>.Failure($"Error retrieving payments by date range: {ex.Message}");
            }
        }

        // Continue implementing remaining methods...
        public async Task<Result<PaymentDto>> UpdatePaymentAsync(Guid id, CreatePaymentDto dto)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetByIdAsync(id);
                if (payment == null || payment.IsDeleted)
                {
                    return Result<PaymentDto>.Failure("Payment not found");
                }

                _mapper.Map(dto, payment);
                payment.UpdatedAt = DateTime.UtcNow;
                payment.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Payments.UpdateAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                var paymentDto = _mapper.Map<PaymentDto>(payment);
                return Result<PaymentDto>.Success(paymentDto, "Payment updated successfully");
            }
            catch (Exception ex)
            {
                return Result<PaymentDto>.Failure($"Error updating payment: {ex.Message}");
            }
        }

        public async Task<Result> DeletePaymentAsync(Guid id)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetByIdAsync(id);
                if (payment == null || payment.IsDeleted)
                {
                    return Result.Failure("Payment not found");
                }

                payment.IsDeleted = true;
                payment.UpdatedAt = DateTime.UtcNow;
                payment.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Payments.UpdateAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Payment deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error deleting payment: {ex.Message}");
            }
        }

        public async Task<Result> MarkPaymentAsOverdueAsync(Guid id)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetByIdAsync(id);
                if (payment == null || payment.IsDeleted)
                {
                    return Result.Failure("Payment not found");
                }

                payment.Status = PaymentStatus.Overdue;
                payment.UpdatedAt = DateTime.UtcNow;
                payment.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Payments.UpdateAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Payment marked as overdue successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error marking payment as overdue: {ex.Message}");
            }
        }

        public async Task<Result> CancelPaymentAsync(Guid id, string reason)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetByIdAsync(id);
                if (payment == null || payment.IsDeleted)
                {
                    return Result.Failure("Payment not found");
                }

                payment.Status = PaymentStatus.Cancelled;
                payment.Notes = $"{payment.Notes} | Cancelled: {reason}";
                payment.UpdatedAt = DateTime.UtcNow;
                payment.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Payments.UpdateAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Payment cancelled successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error cancelling payment: {ex.Message}");
            }
        }

        // Continue with remaining method implementations following the same pattern...
        // For brevity, I'll implement a few more key methods

        public async Task<Result<List<InvoiceDto>>> GetInvoicesByUnitAsync(Guid unitId)
        {
            try
            {
                var invoices = await _unitOfWork.Invoices.GetAsync(i => i.BuildingUnitId == unitId && !i.IsDeleted);
                var invoiceDtos = _mapper.Map<List<InvoiceDto>>(invoices);
                return Result<List<InvoiceDto>>.Success(invoiceDtos);
            }
            catch (Exception ex)
            {
                return Result<List<InvoiceDto>>.Failure($"Error retrieving invoices by unit: {ex.Message}");
            }
        }

        public async Task<Result<List<InvoiceDto>>> GetInvoicesByResidentAsync(Guid residentId)
        {
            try
            {
                var invoices = await _unitOfWork.Invoices.GetAsync(i => i.ResidentId == residentId && !i.IsDeleted);
                var invoiceDtos = _mapper.Map<List<InvoiceDto>>(invoices);
                return Result<List<InvoiceDto>>.Success(invoiceDtos);
            }
            catch (Exception ex)
            {
                return Result<List<InvoiceDto>>.Failure($"Error retrieving invoices by resident: {ex.Message}");
            }
        }

        public async Task<Result<List<InvoiceDto>>> GetInvoicesByBuildingAsync(Guid buildingId)
        {
            try
            {
                var invoices = await _unitOfWork.Invoices.GetAsync(i => i.BuildingUnit.BuildingId == buildingId && !i.IsDeleted);
                var invoiceDtos = _mapper.Map<List<InvoiceDto>>(invoices);
                return Result<List<InvoiceDto>>.Success(invoiceDtos);
            }
            catch (Exception ex)
            {
                return Result<List<InvoiceDto>>.Failure($"Error retrieving invoices by building: {ex.Message}");
            }
        }

        public async Task<Result<List<InvoiceDto>>> GetPendingInvoicesAsync()
        {
            try
            {
                var invoices = await _unitOfWork.Invoices.GetAsync(i =>
                    (i.Status == InvoiceStatus.Draft || i.Status == InvoiceStatus.Sent) && !i.IsDeleted);
                var invoiceDtos = _mapper.Map<List<InvoiceDto>>(invoices);
                return Result<List<InvoiceDto>>.Success(invoiceDtos);
            }
            catch (Exception ex)
            {
                return Result<List<InvoiceDto>>.Failure($"Error retrieving pending invoices: {ex.Message}");
            }
        }

        public Task<Result<InvoiceDto>> UpdateInvoiceAsync(Guid id, UpdateInvoiceDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteInvoiceAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> SendInvoiceAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> MarkInvoiceAsPaidAsync(Guid id, decimal paidAmount)
        {
            throw new NotImplementedException();
        }

        public Task<Result<byte[]>> GenerateInvoicePdfAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> BulkCreateInvoicesAsync(List<CreateInvoiceDto> invoices)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<ExpenseDto>>> GetExpensesByBuildingAsync(Guid buildingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<ExpenseDto>>> GetExpensesByCategoryAsync(ExpenseCategory category)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<ExpenseDto>>> GetExpensesByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<ExpenseDto>>> GetPendingApprovalsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<ExpenseDto>> UpdateExpenseAsync(Guid id, CreateExpenseDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteExpenseAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RejectExpenseAsync(Guid id, string reason)
        {
            throw new NotImplementedException();
        }

        public Task<Result> BulkApproveExpensesAsync(List<Guid> expenseIds)
        {
            throw new NotImplementedException();
        }

        public Task<Result<MonthlyFinancialReportDto>> GetMonthlyReportAsync(int year, int month)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<PaymentDto>>> GetPaymentReportAsync(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<ExpenseDto>>> GetExpenseReportAsync(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<InvoiceDto>>> GetCollectionReportAsync(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public Task<Result<decimal>> GetTotalIncomeAsync(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public Task<Result<decimal>> GetTotalExpensesAsync(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public Task<Result<decimal>> GetPendingPaymentsAmountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<decimal>> GetOverduePaymentsAmountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<int>> GetOverdueInvoiceCountAsync()
        {
            throw new NotImplementedException();
        }
    }
}