//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NET.Application.Common.Interfaces;
//using NET.Application.DTOs.Financial;
//using NET.Application.Services;
//using NET.Domain.Enums;
//using System;
//using System.Threading.Tasks;

//namespace NET.API.Controllers
//{
//    [Route("api/[controller]")]
//    [Authorize]
//    public class FinancialController : BaseController
//    {
//        private readonly IFinancialService _financialService;

//        public FinancialController(
//            IFinancialService financialService,
//            ICurrentUserService currentUserService)
//            : base(currentUserService)
//        {
//            _financialService = financialService;
//        }

//        #region Invoices

//        /// <summary>
//        /// Get all invoices with pagination
//        /// </summary>
//        [HttpGet("invoices")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetInvoices([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] InvoiceStatus? status = null)
//        {
//            var result = await _financialService.GetAllInvoicesAsync(page, pageSize, status);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get invoice by ID
//        /// </summary>
//        [HttpGet("invoices/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Resident")]
//        public async Task<IActionResult> GetInvoice(Guid id)
//        {
//            var result = await _financialService.GetInvoiceByIdAsync(id);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get invoices by resident
//        /// </summary>
//        [HttpGet("invoices/resident/{residentId}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Resident")]
//        public async Task<IActionResult> GetInvoicesByResident(Guid residentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            // Check if resident can view their own invoices
//            if (IsInRole("Resident"))
//            {
//                // Additional logic to verify resident access
//            }

//            var result = await _financialService.GetInvoicesByResidentAsync(residentId, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get invoices by building unit
//        /// </summary>
//        [HttpGet("invoices/unit/{unitId}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetInvoicesByUnit(Guid unitId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _financialService.GetInvoicesByUnitAsync(unitId, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get overdue invoices
//        /// </summary>
//        [HttpGet("invoices/overdue")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetOverdueInvoices([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _financialService.GetOverdueInvoicesAsync(page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Create a new invoice
//        /// </summary>
//        [HttpPost("invoices")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _financialService.CreateInvoiceAsync(dto);

//            if (result.IsSuccess)
//            {
//                return CreatedAtAction(nameof(GetInvoice), new { id = result.Data.Id }, new
//                {
//                    Success = true,
//                    Data = result.Data,
//                    Message = result.Message
//                });
//            }

//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Update invoice
//        /// </summary>
//        [HttpPut("invoices/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> UpdateInvoice(Guid id, [FromBody] UpdateInvoiceDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _financialService.UpdateInvoiceAsync(id, dto);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Delete invoice
//        /// </summary>
//        [HttpDelete("invoices/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin")]
//        public async Task<IActionResult> DeleteInvoice(Guid id)
//        {
//            var result = await _financialService.DeleteInvoiceAsync(id);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Mark invoice as paid
//        /// </summary>
//        [HttpPost("invoices/{id}/mark-paid")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> MarkInvoiceAsPaid(Guid id, [FromBody] MarkInvoicePaidDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _financialService.MarkInvoiceAsPaidAsync(id, dto.PaidAmount, dto.PaymentDate, dto.Notes);
//            return HandleResult(result);
//        }

//        #endregion

//        #region Payments

//        /// <summary>
//        /// Get all payments with pagination
//        /// </summary>
//        [HttpGet("payments")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetPayments([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] PaymentStatus? status = null)
//        {
//            var result = await _financialService.GetAllPaymentsAsync(page, pageSize, status);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get payment by ID
//        /// </summary>
//        [HttpGet("payments/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Resident")]
//        public async Task<IActionResult> GetPayment(Guid id)
//        {
//            var result = await _financialService.GetPaymentByIdAsync(id);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get payments by resident
//        /// </summary>
//        [HttpGet("payments/resident/{residentId}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Resident")]
//        public async Task<IActionResult> GetPaymentsByResident(Guid residentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            // Check if resident can view their own payments
//            if (IsInRole("Resident"))
//            {
//                // Additional logic to verify resident access
//            }

//            var result = await _financialService.GetPaymentsByResidentAsync(residentId, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get payments by date range
//        /// </summary>
//        [HttpGet("payments/by-date")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetPaymentsByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _financialService.GetPaymentsByDateRangeAsync(fromDate, toDate, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Create a new payment
//        /// </summary>
//        [HttpPost("payments")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _financialService.CreatePaymentAsync(dto);

//            if (result.IsSuccess)
//            {
//                return CreatedAtAction(nameof(GetPayment), new { id = result.Data.Id }, new
//                {
//                    Success = true,
//                    Data = result.Data,
//                    Message = result.Message
//                });
//            }

//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Update payment
//        /// </summary>
//        [HttpPut("payments/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> UpdatePayment(Guid id, [FromBody] UpdatePaymentDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _financialService.UpdatePaymentAsync(id, dto);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Delete payment
//        /// </summary>
//        [HttpDelete("payments/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin")]
//        public async Task<IActionResult> DeletePayment(Guid id)
//        {
//            var result = await _financialService.DeletePaymentAsync(id);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Confirm payment
//        /// </summary>
//        [HttpPost("payments/{id}/confirm")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> ConfirmPayment(Guid id)
//        {
//            var result = await _financialService.ConfirmPaymentAsync(id);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Cancel payment
//        /// </summary>
//        [HttpPost("payments/{id}/cancel")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> CancelPayment(Guid id, [FromBody] CancelPaymentDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _financialService.CancelPaymentAsync(id, dto.Reason);
//            return HandleResult(result);
//        }

//        #endregion

//        #region Expenses

//        /// <summary>
//        /// Get all expenses with pagination
//        /// </summary>
//        [HttpGet("expenses")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetExpenses([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] ExpenseCategory? category = null)
//        {
//            var result = await _financialService.GetAllExpensesAsync(page, pageSize, category);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get expense by ID
//        /// </summary>
//        [HttpGet("expenses/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetExpense(Guid id)
//        {
//            var result = await _financialService.GetExpenseByIdAsync(id);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get expenses by building
//        /// </summary>
//        [HttpGet("expenses/building/{buildingId}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetExpensesByBuilding(Guid buildingId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _financialService.GetExpensesByBuildingAsync(buildingId, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get expenses by date range
//        /// </summary>
//        [HttpGet("expenses/by-date")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetExpensesByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _financialService.GetExpensesByDateRangeAsync(fromDate, toDate, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Create a new expense
//        /// </summary>
//        [HttpPost("expenses")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _financialService.CreateExpenseAsync(dto);

//            if (result.IsSuccess)
//            {
//                return CreatedAtAction(nameof(GetExpense), new { id = result.Data.Id }, new
//                {
//                    Success = true,
//                    Data = result.Data,
//                    Message = result.Message
//                });
//            }

//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Update expense
//        /// </summary>
//        [HttpPut("expenses/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _financialService.UpdateExpenseAsync(id, dto);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Delete expense
//        /// </summary>
//        [HttpDelete("expenses/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin")]
//        public async Task<IActionResult> DeleteExpense(Guid id)
//        {
//            var result = await _financialService.DeleteExpenseAsync(id);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Approve expense
//        /// </summary>
//        [HttpPost("expenses/{id}/approve")]
//        [Authorize(Roles = "SuperAdmin,Admin")]
//        public async Task<IActionResult> ApproveExpense(Guid id, [FromBody] ApproveExpenseDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var userId = GetCurrentUserId();
//            var result = await _financialService.ApproveExpenseAsync(id, userId, dto.Notes);
//            return HandleResult(result);
//        }

//        #endregion

//        #region Reports

//        /// <summary>
//        /// Get financial summary
//        /// </summary>
//        [HttpGet("summary")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetFinancialSummary([FromQuery] Guid? buildingId = null, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
//        {
//            var result = await _financialService.GetFinancialSummaryAsync(buildingId, fromDate, toDate);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get monthly financial report
//        /// </summary>
//        [HttpGet("monthly-report")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetMonthlyFinancialReport([FromQuery] int year, [FromQuery] int month, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _financialService.GetMonthlyFinancialReportAsync(year, month, buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get debt report
//        /// </summary>
//        [HttpGet("debt-report")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetDebtReport([FromQuery] Guid? buildingId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _financialService.GetDebtReportAsync(buildingId, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get income vs expenses report
//        /// </summary>
//        [HttpGet("income-vs-expenses")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetIncomeVsExpensesReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _financialService.GetIncomeVsExpensesReportAsync(fromDate, toDate, buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Export financial data to Excel
//        /// </summary>
//        [HttpGet("export")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> ExportFinancialData([FromQuery] string type, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _financialService.ExportFinancialDataAsync(type, fromDate, toDate, buildingId);
            
//            if (result.IsSuccess)
//            {
//                var contentType = type.ToLower() switch
//                {
//                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                    "csv" => "text/csv",
//                    _ => "application/octet-stream"
//                };

//                var fileName = $"financial_data_{DateTime.Now:yyyyMMdd}.{type.ToLower()}";
//                return File(result.Data, contentType, fileName);
//            }

//            return HandleResult(result);
//        }

//        #endregion

//        #region Dashboard

//        /// <summary>
//        /// Get financial dashboard data
//        /// </summary>
//        [HttpGet("dashboard")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetFinancialDashboard([FromQuery] Guid? buildingId = null)
//        {
//            var result = await _financialService.GetFinancialDashboardAsync(buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get payment statistics
//        /// </summary>
//        [HttpGet("payment-statistics")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetPaymentStatistics([FromQuery] Guid? buildingId = null, [FromQuery] int months = 12)
//        {
//            var result = await _financialService.GetPaymentStatisticsAsync(buildingId, months);
//            return HandleResult(result);
//        }

//        #endregion
//    }
//}

//// Additional DTOs that might be missing
//namespace NET.Application.DTOs.Financial
//{
//    public class MarkInvoicePaidDto
//    {
//        public decimal PaidAmount { get; set; }
//        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
//        public string? Notes { get; set; }
//    }

//    public class CancelPaymentDto
//    {
//        public string Reason { get; set; } = string.Empty;
//    }

//    public class ApproveExpenseDto
//    {
//        public string? Notes { get; set; }
//    }
//}