using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET.Application.Common.Interfaces;
using NET.Application.DTOs.Resident;
using NET.Application.Services;
using System;
using System.Threading.Tasks;

namespace NET.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ResidentsController : BaseController
    {
        private readonly IResidentService _residentService;

        public ResidentsController(
            IResidentService residentService,
            ICurrentUserService currentUserService)
            : base(currentUserService)
        {
            _residentService = residentService;
        }

        /// <summary>
        /// Get all residents with pagination
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> GetResidents([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _residentService.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get resident by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager,Resident")]
        public async Task<IActionResult> GetResident(Guid id)
        {
            var result = await _residentService.GetByIdAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Get residents by building
        /// </summary>
        [HttpGet("by-building/{buildingId}")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> GetResidentsByBuilding(Guid buildingId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _residentService.GetResidentsByBuildingAsync(buildingId);
            return Ok(result);
        }

        /// <summary>
        /// Get residents by unit
        /// </summary>
        [HttpGet("by-unit/{unitId}")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> GetResidentsByUnit(Guid unitId)
        {
            var result = await _residentService.GetResidentsByUnitAsync(unitId);
            return HandleResult(result);
        }

        /// <summary>
        /// Get active residents
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> GetActiveResidents([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _residentService.GetActiveResidentsAsync();
            return Ok(result);
        }

        ///// <summary>
        ///// Get current resident's profile (for residents who are logged in as users)
        ///// </summary>
        //[HttpGet("profile")]
        //[Authorize(Roles = "Resident")]
        //public async Task<IActionResult> GetMyProfile()
        //{
        //    var userId = GetCurrentUserId();
        //    var result = await _residentService.GetResidentByUserIdAsync(userId);
        //    return HandleResult(result);
        //}

        /// <summary>
        /// Create a new resident
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> CreateResident([FromBody] CreateResidentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _residentService.CreateAsync(dto);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetResident), new { id = result.Data.Id }, new
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return HandleResult(result);
        }

        ///// <summary>
        ///// Update resident
        ///// </summary>
        //[HttpPut("{id}")]
        //[Authorize(Roles = "SuperAdmin,Admin,Manager")]
        //public async Task<IActionResult> UpdateResident(Guid id, [FromBody] UpdateResidentDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var result = await _residentService.UpdateAsync(id, dto);
        //    return HandleResult(result);
        //}

        ///// <summary>
        ///// Update current resident's profile
        ///// </summary>
        //[HttpPut("profile")]
        //[Authorize(Roles = "Resident")]
        //public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateResidentDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var userId = GetCurrentUserId();
        //    var residentResult = await _residentService.GetResidentByUserIdAsync(userId);
            
        //    if (!residentResult.IsSuccess)
        //        return HandleResult(residentResult);

        //    var result = await _residentService.UpdateAsync(residentResult.Data.Id, dto);
        //    return HandleResult(result);
        //}

        /// <summary>
        /// Delete resident (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteResident(Guid id)
        {
            var result = await _residentService.DeleteAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Activate resident
        /// </summary>
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> ActivateResident(Guid id)
        {
            var result = await _residentService.ActivateAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Deactivate resident
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> DeactivateResident(Guid id)
        {
            var result = await _residentService.DeactivateAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Search residents
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        public async Task<IActionResult> SearchResidents([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { Success = false, Message = "Search query is required" });

            var result = await _residentService.SearchResidentsAsync(query);
            return Ok(result);
        }

        ///// <summary>
        ///// Get resident statistics
        ///// </summary>
        //[HttpGet("statistics")]
        //[Authorize(Roles = "SuperAdmin,Admin,Manager")]
        //public async Task<IActionResult> GetResidentStatistics()
        //{
        //    var result = await _residentService.GetResidentStatisticsAsync();
        //    return HandleResult(result);
        //}
    }
}

// Additional DTOs that might be missing
namespace NET.Application.DTOs.Resident
{
    public class AssignUnitDto
    {
        public Guid UnitId { get; set; }
        public bool IsPrimaryResident { get; set; } = true;
        public DateTime MoveInDate { get; set; } = DateTime.UtcNow;
        public decimal DepositAmount { get; set; } = 0;
        public decimal RentAmount { get; set; } = 0;
    }

    public class RemoveFromUnitDto
    {
        public DateTime MoveOutDate { get; set; } = DateTime.UtcNow;
    }

    public class UpdateEmergencyContactDto
    {
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
    }
}