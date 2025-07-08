//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NET.Application.Common.Interfaces;
//using NET.Application.Services;
//using System;
//using System.Threading.Tasks;

//namespace NET.API.Controllers
//{
//    [Route("api/[controller]")]
//    [Authorize]
//    public class ReportsController : BaseController
//    {
//        private readonly IFinancialService _financialService;
//        private readonly IMaintenanceService _maintenanceService;
//        private readonly IBuildingService _buildingService;
//        private readonly IResidentService _residentService;
//        private readonly IUserService _userService;

//        public ReportsController(
//            IFinancialService financialService,
//            IMaintenanceService maintenanceService,
//            IBuildingService buildingService,
//            IResidentService residentService,
//            IUserService userService,
//            ICurrentUserService currentUserService)
//            : base(currentUserService)
//        {
//            _financialService = financialService;
//            _maintenanceService = maintenanceService;
//            _buildingService = buildingService;
//            _residentService = residentService;
//            _userService = userService;
//        }

//        #region Financial Reports

//        /// <summary>
//        /// Get financial summary report
//        /// </summary>
//        [HttpGet("financial/summary")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetFinancialSummaryReport([FromQuery] Guid? buildingId = null, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
//        {
//            var result = await _financialService.GetFinancialSummaryAsync(buildingId, fromDate, toDate);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get monthly financial report
//        /// </summary>
//        [HttpGet("financial/monthly")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetMonthlyFinancialReport([FromQuery] int year, [FromQuery] int month, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _financialService.GetMonthlyFinancialReportAsync(year, month, buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get yearly financial report
//        /// </summary>
//        [HttpGet("financial/yearly")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetYearlyFinancialReport([FromQuery] int year, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _financialService.GetYearlyFinancialReportAsync(year, buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get debt report
//        /// </summary>
//        [HttpGet("financial/debt")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetDebtReport([FromQuery] Guid? buildingId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _financialService.GetDebtReportAsync(buildingId, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get income vs expenses report
//        /// </summary>
//        [HttpGet("financial/income-vs-expenses")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetIncomeVsExpensesReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _financialService.GetIncomeVsExpensesReportAsync(fromDate, toDate, buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get payment collection report
//        /// </summary>
//        [HttpGet("financial/payment-collection")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetPaymentCollectionReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _financialService.GetPaymentCollectionReportAsync(fromDate, toDate, buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get expense breakdown report
//        /// </summary>
//        [HttpGet("financial/expense-breakdown")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetExpenseBreakdownReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _financialService.GetExpenseBreakdownReportAsync(fromDate, toDate, buildingId);
//            return HandleResult(result);
//        }

//        #endregion

//        #region Maintenance Reports

//        /// <summary>
//        /// Get maintenance statistics report
//        /// </summary>
//        [HttpGet("maintenance/statistics")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetMaintenanceStatisticsReport([FromQuery] Guid? buildingId = null, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
//        {
//            var result = await _maintenanceService.GetMaintenanceStatisticsAsync(buildingId, fromDate, toDate);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get maintenance cost analysis report
//        /// </summary>
//        [HttpGet("maintenance/cost-analysis")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetMaintenanceCostAnalysisReport([FromQuery] Guid? buildingId = null, [FromQuery] int months = 12)
//        {
//            var result = await _maintenanceService.GetMaintenanceCostAnalysisAsync(buildingId, months);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get maintenance performance metrics report
//        /// </summary>
//        [HttpGet("maintenance/performance")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetMaintenancePerformanceReport([FromQuery] Guid? buildingId = null, [FromQuery] Guid? assignedUserId = null, [FromQuery] int months = 6)
//        {
//            var result = await _maintenanceService.GetMaintenancePerformanceMetricsAsync(buildingId, assignedUserId, months);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get maintenance trends report
//        /// </summary>
//        [HttpGet("maintenance/trends")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetMaintenanceTrendsReport([FromQuery] Guid? buildingId = null, [FromQuery] int months = 12)
//        {
//            var result = await _maintenanceService.GetMaintenanceTrendsReportAsync(buildingId, months);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get maintenance by category report
//        /// </summary>
//        [HttpGet("maintenance/by-category")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetMaintenanceByCategoryReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _maintenanceService.GetMaintenanceByCategoryReportAsync(fromDate, toDate, buildingId);
//            return HandleResult(result);
//        }

//        #endregion

//        #region Building Reports

//        /// <summary>
//        /// Get building statistics report
//        /// </summary>
//        [HttpGet("buildings/statistics")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetBuildingStatisticsReport()
//        {
//            var result = await _buildingService.GetBuildingStatisticsReportAsync();
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get building occupancy report
//        /// </summary>
//        [HttpGet("buildings/occupancy")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetBuildingOccupancyReport([FromQuery] Guid? buildingId = null)
//        {
//            var result = await _buildingService.GetBuildingOccupancyReportAsync(buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get building revenue report
//        /// </summary>
//        [HttpGet("buildings/revenue")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetBuildingRevenueReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _buildingService.GetBuildingRevenueReportAsync(fromDate, toDate, buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get unit performance report
//        /// </summary>
//        [HttpGet("buildings/unit-performance")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetUnitPerformanceReport([FromQuery] Guid buildingId, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
//        {
//            var result = await _buildingService.GetUnitPerformanceReportAsync(buildingId, fromDate, toDate);
//            return HandleResult(result);
//        }

//        #endregion

//        #region Resident Reports

//        /// <summary>
//        /// Get resident statistics report
//        /// </summary>
//        [HttpGet("residents/statistics")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetResidentStatisticsReport()
//        {
//            var result = await _residentService.GetResidentStatisticsAsync();
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get resident turnover report
//        /// </summary>
//        [HttpGet("residents/turnover")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetResidentTurnoverReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _residentService.GetResidentTurnoverReportAsync(fromDate, toDate, buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get resident payment behavior report
//        /// </summary>
//        [HttpGet("residents/payment-behavior")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetResidentPaymentBehaviorReport([FromQuery] Guid? buildingId = null, [FromQuery] int months = 12)
//        {
//            var result = await _residentService.GetResidentPaymentBehaviorReportAsync(buildingId, months);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get lease expiration report
//        /// </summary>
//        [HttpGet("residents/lease-expiration")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetLeaseExpirationReport([FromQuery] int daysAhead = 90, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _residentService.GetLeaseExpirationReportAsync(daysAhead, buildingId);
//            return HandleResult(result);
//        }

//        #endregion

//        #region User Reports

//        /// <summary>
//        /// Get user statistics report
//        /// </summary>
//        [HttpGet("users/statistics")]
//        [Authorize(Roles = "SuperAdmin,Admin")]
//        public async Task<IActionResult> GetUserStatisticsReport()
//        {
//            var result = await _userService.GetUserStatisticsAsync();
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get user activity report
//        /// </summary>
//        [HttpGet("users/activity")]
//        [Authorize(Roles = "SuperAdmin,Admin")]
//        public async Task<IActionResult> GetUserActivityReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _userService.GetUserActivityReportAsync(fromDate, toDate, buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get user login report
//        /// </summary>
//        [HttpGet("users/login-history")]
//        [Authorize(Roles = "SuperAdmin,Admin")]
//        public async Task<IActionResult> GetUserLoginReport([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _userService.GetUserLoginReportAsync(fromDate, toDate, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        #endregion

//        #region Dashboard Reports

//        /// <summary>
//        /// Get executive dashboard data
//        /// </summary>
//        [HttpGet("dashboard/executive")]
//        [Authorize(Roles = "SuperAdmin,Admin")]
//        public async Task<IActionResult> GetExecutiveDashboard([FromQuery] Guid? buildingId = null)
//        {
//            var financialDashboard = await _financialService.GetFinancialDashboardAsync(buildingId);
//            var maintenanceDashboard = await _maintenanceService.GetMaintenanceDashboardAsync(buildingId);
//            var buildingStatistics = await _buildingService.GetBuildingStatisticsAsync();
//            var residentStatistics = await _residentService.GetResidentStatisticsAsync();

//            var executiveDashboard = new
//            {
//                Financial = financialDashboard.Data,
//                Maintenance = maintenanceDashboard.Data,
//                Buildings = buildingStatistics.Data,
//                Residents = residentStatistics.Data,
//                GeneratedAt = DateTime.UtcNow
//            };

//            return Ok(new
//            {
//                Success = true,
//                Data = executiveDashboard,
//                Message = "Executive dashboard data retrieved successfully"
//            });
//        }

//        /// <summary>
//        /// Get operational dashboard data
//        /// </summary>
//        [HttpGet("dashboard/operational")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetOperationalDashboard([FromQuery] Guid? buildingId = null)
//        {
//            var maintenancePending = await _maintenanceService.GetPendingMaintenanceRequestsAsync(1, 10);
//            var overdueInvoices = await _financialService.GetOverdueInvoicesAsync(1, 10);
//            var recentPayments = await _financialService.GetRecentPaymentsAsync(1, 10);
//            var highPriorityMaintenance = await _maintenanceService.GetHighPriorityMaintenanceRequestsAsync(1, 10);

//            var operationalDashboard = new
//            {
//                PendingMaintenance = maintenancePending.Data,
//                OverdueInvoices = overdueInvoices.Data,
//                RecentPayments = recentPayments.Data,
//                HighPriorityMaintenance = highPriorityMaintenance.Data,
//                GeneratedAt = DateTime.UtcNow
//            };

//            return Ok(new
//            {
//                Success = true,
//                Data = operationalDashboard,
//                Message = "Operational dashboard data retrieved successfully"
//            });
//        }

//        #endregion

//        #region Export Reports

//        /// <summary>
//        /// Export comprehensive building report
//        /// </summary>
//        [HttpGet("export/building-report")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> ExportBuildingReport([FromQuery] Guid buildingId, [FromQuery] string format = "pdf", [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
//        {
//            var result = await _buildingService.ExportBuildingReportAsync(buildingId, format, fromDate, toDate);
            
//            if (result.IsSuccess)
//            {
//                var contentType = format.ToLower() switch
//                {
//                    "pdf" => "application/pdf",
//                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                    "csv" => "text/csv",
//                    _ => "application/octet-stream"
//                };

//                var fileName = $"building_report_{buildingId}_{DateTime.Now:yyyyMMdd}.{format.ToLower()}";
//                return File(result.Data, contentType, fileName);
//            }

//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Export financial report
//        /// </summary>
//        [HttpGet("export/financial-report")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> ExportFinancialReport([FromQuery] string format = "excel", [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null, [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _financialService.ExportFinancialDataAsync(format, fromDate, toDate, buildingId);
            
//            if (result.IsSuccess)
//            {
//                var contentType = format.ToLower() switch
//                {
//                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                    "csv" => "text/csv",
//                    "pdf" => "application/pdf",
//                    _ => "application/octet-stream"
//                };

//                var fileName = $"financial_report_{DateTime.Now:yyyyMMdd}.{format.ToLower()}";
//                return File(result.Data, contentType, fileName);
//            }

//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Export maintenance report
//        /// </summary>
//        [HttpGet("export/maintenance-report")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> ExportMaintenanceReport([FromQuery] string format = "excel", [FromQuery] Guid? buildingId = null, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
//        {
//            var result = await _maintenanceService.ExportMaintenanceRequestsAsync(format, buildingId, fromDate, toDate);
            
//            if (result.IsSuccess)
//            {
//                var contentType = format.ToLower() switch
//                {
//                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                    "csv" => "text/csv",
//                    "pdf" => "application/pdf",
//                    _ => "application/octet-stream"
//                };

//                var fileName = $"maintenance_report_{DateTime.Now:yyyyMMdd}.{format.ToLower()}";
//                return File(result.Data, contentType, fileName);
//            }

//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Export resident report
//        /// </summary>
//        [HttpGet("export/resident-report")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> ExportResidentReport([FromQuery] string format = "excel", [FromQuery] Guid? buildingId = null)
//        {
//            var result = await _residentService.ExportResidentsAsync(buildingId);
            
//            if (result.IsSuccess)
//            {
//                var contentType = format.ToLower() switch
//                {
//                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                    "csv" => "text/csv",
//                    _ => "text/csv"
//                };

//                var fileName = $"resident_report_{DateTime.Now:yyyyMMdd}.{(format.ToLower() == "excel" ? "xlsx" : "csv")}";
//                return File(result.Data, contentType, fileName);
//            }

//            return HandleResult(result);
//        }

//        #endregion

//        #region Custom Reports

//        /// <summary>
//        /// Generate custom report based on filters
//        /// </summary>
//        //[HttpPost("custom")]
//        //[Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        //public async Task<IActionResult> GenerateCustomReport([FromBody] CustomReportRequestDto request)
//        //{
//        //    if (!ModelState.IsValid)
//        //        return BadRequest(ModelState);

//        //    var result = await GenerateCustomReportAsync(request);
//        //    return HandleResult(result);
//        //}

//        //private async Task<NET.Application.Common.Models.Result<object>> GenerateCustomReportAsync(CustomReportRequestDto request)
//        //{
//        //    // This would implement custom report generation logic
//        //    // For now, return a placeholder
//        //    await Task.Delay(1);
//        //    return NET.Application.Common.Models.Result<object>.Success(new
//        //    {
//        //        ReportType = request.ReportType,
//        //        GeneratedAt = DateTime.UtcNow,
//        //        Message = "Custom report generation not yet implemented"
//        //    });
//        //}

//        #endregion
//    }
//}

//// Custom report DTO
//namespace NET.Application.DTOs.Reports
//{
//    public class CustomReportRequestDto
//    {
//        public string ReportType { get; set; } = string.Empty;
//        public Guid? BuildingId { get; set; }
//        public DateTime? FromDate { get; set; }
//        public DateTime? ToDate { get; set; }
//        public List<string> Metrics { get; set; } = new();
//        public List<string> Groupings { get; set; } = new();
//        public Dictionary<string, object> Filters { get; set; } = new();
//    }
//}