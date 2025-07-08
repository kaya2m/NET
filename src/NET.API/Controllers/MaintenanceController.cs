//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NET.Application.Common.Interfaces;
//using NET.Application.DTOs.Maintenance;
//using NET.Application.Services;
//using NET.Domain.Enums;
//using System;
//using System.Threading.Tasks;

//namespace NET.API.Controllers
//{
//    [Route("api/[controller]")]
//    [Authorize]
//    public class MaintenanceController : BaseController
//    {
//        private readonly IMaintenanceService _maintenanceService;

//        public MaintenanceController(
//            IMaintenanceService maintenanceService,
//            ICurrentUserService currentUserService)
//            : base(currentUserService)
//        {
//            _maintenanceService = maintenanceService;
//        }

//        /// <summary>
//        /// Get all maintenance requests with pagination
//        /// </summary>
//        [HttpGet("requests")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> GetMaintenanceRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] MaintenanceStatus? status = null, [FromQuery] Priority? priority = null)
//        {
//            var result = await _maintenanceService.GetAllMaintenanceRequestsAsync(page, pageSize, status, priority);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get maintenance request by ID
//        /// </summary>
//        [HttpGet("requests/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance,Resident")]
//        public async Task<IActionResult> GetMaintenanceRequest(Guid id)
//        {
//            var result = await _maintenanceService.GetMaintenanceRequestByIdAsync(id);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get maintenance requests by building
//        /// </summary>
//        [HttpGet("requests/building/{buildingId}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> GetMaintenanceRequestsByBuilding(Guid buildingId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _maintenanceService.GetMaintenanceRequestsByBuildingAsync(buildingId, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get maintenance requests by resident
//        /// </summary>
//        [HttpGet("requests/resident/{residentId}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance,Resident")]
//        public async Task<IActionResult> GetMaintenanceRequestsByResident(Guid residentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            // Check if resident can view their own requests
//            if (IsInRole("Resident"))
//            {
//                // Additional logic to verify resident access
//            }

//            var result = await _maintenanceService.GetMaintenanceRequestsByResidentAsync(residentId, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get maintenance requests assigned to current user
//        /// </summary>
//        [HttpGet("requests/my-assignments")]
//        [Authorize(Roles = "Maintenance")]
//        public async Task<IActionResult> GetMyAssignedRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var userId = GetCurrentUserId();
//            var result = await _maintenanceService.GetMaintenanceRequestsByAssignedUserAsync(userId, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get current resident's maintenance requests
//        /// </summary>
//        [HttpGet("requests/my-requests")]
//        [Authorize(Roles = "Resident")]
//        public async Task<IActionResult> GetMyMaintenanceRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var userId = GetCurrentUserId();
//            var result = await _maintenanceService.GetMaintenanceRequestsByResidentUserAsync(userId, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get pending maintenance requests
//        /// </summary>
//        [HttpGet("requests/pending")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> GetPendingMaintenanceRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _maintenanceService.GetPendingMaintenanceRequestsAsync(page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get high priority maintenance requests
//        /// </summary>
//        [HttpGet("requests/high-priority")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> GetHighPriorityMaintenanceRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _maintenanceService.GetHighPriorityMaintenanceRequestsAsync(page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Create a new maintenance request
//        /// </summary>
//        [HttpPost("requests")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Resident")]
//        public async Task<IActionResult> CreateMaintenanceRequest([FromBody] CreateMaintenanceRequestDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _maintenanceService.CreateMaintenanceRequestAsync(dto);

//            if (result.IsSuccess)
//            {
//                return CreatedAtAction(nameof(GetMaintenanceRequest), new { id = result.Data.Id }, new
//                {
//                    Success = true,
//                    Data = result.Data,
//                    Message = result.Message
//                });
//            }

//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Update maintenance request
//        /// </summary>
//        [HttpPut("requests/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> UpdateMaintenanceRequest(Guid id, [FromBody] UpdateMaintenanceRequestDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _maintenanceService.UpdateMaintenanceRequestAsync(id, dto);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Delete maintenance request
//        /// </summary>
//        [HttpDelete("requests/{id}")]
//        [Authorize(Roles = "SuperAdmin,Admin")]
//        public async Task<IActionResult> DeleteMaintenanceRequest(Guid id)
//        {
//            var result = await _maintenanceService.DeleteMaintenanceRequestAsync(id);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Assign maintenance request to user
//        /// </summary>
//        [HttpPost("requests/{id}/assign")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> AssignMaintenanceRequest(Guid id, [FromBody] AssignMaintenanceDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _maintenanceService.AssignMaintenanceRequestAsync(id, dto.AssignedToUserId, dto.ScheduledDate, dto.Notes);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Start working on maintenance request
//        /// </summary>
//        [HttpPost("requests/{id}/start")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> StartMaintenanceRequest(Guid id, [FromBody] StartMaintenanceDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _maintenanceService.StartMaintenanceRequestAsync(id, dto.Notes);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Complete maintenance request
//        /// </summary>
//        [HttpPost("requests/{id}/complete")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> CompleteMaintenanceRequest(Guid id, [FromBody] CompleteMaintenanceDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _maintenanceService.CompleteMaintenanceRequestAsync(id, dto.CompletionNotes, dto.ActualCost, dto.CompletedDate);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Cancel maintenance request
//        /// </summary>
//        [HttpPost("requests/{id}/cancel")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> CancelMaintenanceRequest(Guid id, [FromBody] CancelMaintenanceDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _maintenanceService.CancelMaintenanceRequestAsync(id, dto.CancellationReason);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Update maintenance request priority
//        /// </summary>
//        [HttpPost("requests/{id}/priority")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> UpdateMaintenancePriority(Guid id, [FromBody] UpdatePriorityDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _maintenanceService.UpdateMaintenancePriorityAsync(id, dto.Priority, dto.Reason);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Add photos to maintenance request
//        /// </summary>
//        [HttpPost("requests/{id}/photos")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance,Resident")]
//        public async Task<IActionResult> AddPhotosToMaintenanceRequest(Guid id, [FromBody] AddPhotosDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _maintenanceService.AddPhotosToMaintenanceRequestAsync(id, dto.PhotoUrls);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get maintenance request history
//        /// </summary>
//        [HttpGet("requests/{id}/history")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> GetMaintenanceRequestHistory(Guid id)
//        {
//            var result = await _maintenanceService.GetMaintenanceRequestHistoryAsync(id);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Search maintenance requests
//        /// </summary>
//        [HttpGet("requests/search")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> SearchMaintenanceRequests([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            if (string.IsNullOrWhiteSpace(query))
//                return BadRequest(new { Success = false, Message = "Search query is required" });

//            var result = await _maintenanceService.SearchMaintenanceRequestsAsync(query, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Filter maintenance requests
//        /// </summary>
//        [HttpPost("requests/filter")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> FilterMaintenanceRequests([FromBody] MaintenanceRequestFilterDto filter)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _maintenanceService.FilterMaintenanceRequestsAsync(filter);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get maintenance statistics
//        /// </summary>
//        [HttpGet("statistics")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetMaintenanceStatistics([FromQuery] Guid? buildingId = null, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
//        {
//            var result = await _maintenanceService.GetMaintenanceStatisticsAsync(buildingId, fromDate, toDate);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get maintenance dashboard data
//        /// </summary>
//        [HttpGet("dashboard")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> GetMaintenanceDashboard([FromQuery] Guid? buildingId = null)
//        {
//            var result = await _maintenanceService.GetMaintenanceDashboardAsync(buildingId);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get overdue maintenance requests
//        /// </summary>
//        [HttpGet("requests/overdue")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> GetOverdueMaintenanceRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _maintenanceService.GetOverdueMaintenanceRequestsAsync(page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Get maintenance requests by category
//        /// </summary>
//        [HttpGet("requests/by-category/{category}")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager,Maintenance")]
//        public async Task<IActionResult> GetMaintenanceRequestsByCategory(string category, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        {
//            var result = await _maintenanceService.GetMaintenanceRequestsByCategoryAsync(category, page, pageSize);
//            return HandlePagedResult(result);
//        }

//        /// <summary>
//        /// Export maintenance requests to Excel
//        /// </summary>
//        [HttpGet("requests/export")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> ExportMaintenanceRequests([FromQuery] string? format = "excel", [FromQuery] Guid? buildingId = null, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
//        {
//            var result = await _maintenanceService.ExportMaintenanceRequestsAsync(format, buildingId, fromDate, toDate);
            
//            if (result.IsSuccess)
//            {
//                var contentType = format?.ToLower() switch
//                {
//                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                    "csv" => "text/csv",
//                    _ => "application/octet-stream"
//                };

//                var fileName = $"maintenance_requests_{DateTime.Now:yyyyMMdd}.{format?.ToLower() ?? "excel"}";
//                return File(result.Data, contentType, fileName);
//            }

//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get maintenance cost analysis
//        /// </summary>
//        [HttpGet("cost-analysis")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetMaintenanceCostAnalysis([FromQuery] Guid? buildingId = null, [FromQuery] int months = 12)
//        {
//            var result = await _maintenanceService.GetMaintenanceCostAnalysisAsync(buildingId, months);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Get maintenance performance metrics
//        /// </summary>
//        [HttpGet("performance-metrics")]
//        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
//        public async Task<IActionResult> GetMaintenancePerformanceMetrics([FromQuery] Guid? buildingId = null, [FromQuery] Guid? assignedUserId = null, [FromQuery] int months = 6)
//        {
//            var result = await _maintenanceService.GetMaintenancePerformanceMetricsAsync(buildingId, assignedUserId, months);
//            return HandleResult(result);
//        }

//        /// <summary>
//        /// Approve maintenance request (for high-value requests)
//        /// </summary>
//        [HttpPost("requests/{id}/approve")]
//        [Authorize(Roles = "SuperAdmin,Admin")]
//        public async Task<IActionResult> ApproveMaintenanceRequest(Guid id, [FromBody] ApproveMaintenanceDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var userId = GetCurrentUserId();
//            var result = await _maintenanceService.ApproveMaintenanceRequestAsync(id, userId, dto.ApprovalNotes);
//            return HandleResult(result);
//        }
//    }
//}