using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET.Application.Common.Interfaces;
using NET.Application.DTOs.Building;
using NET.Application.Services;
using NET.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace NET.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BuildingsController : BaseController
    {
        private readonly IBuildingService _buildingService;

        public BuildingsController(
            IBuildingService buildingService,
            ICurrentUserService currentUserService)
            : base(currentUserService)
        {
            _buildingService = buildingService;
        }

        /// <summary>
        /// Get all buildings with pagination
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,TenantAdmin,BuildingManager")]
        public async Task<IActionResult> GetBuildings([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _buildingService.GetAllAsync(page, pageSize);
            return HandlePagedResult(result);
        }

        /// <summary>
        /// Get building by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin,BuildingManager,Resident")]
        public async Task<IActionResult> GetBuilding(Guid id)
        {
            var result = await _buildingService.GetByIdAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Get all active buildings
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin,BuildingManager")]
        public async Task<IActionResult> GetActiveBuildings()
        {
            var result = await _buildingService.GetActiveBuildingsAsync();
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new building
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,TenantAdmin")]
        public async Task<IActionResult> CreateBuilding([FromBody] CreateBuildingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _buildingService.CreateAsync(dto);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetBuilding), new { id = result.Data.Id }, new
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return HandleResult(result);
        }

        /// <summary>
        /// Update building
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin,BuildingManager")]
        public async Task<IActionResult> UpdateBuilding(Guid id, [FromBody] UpdateBuildingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _buildingService.UpdateAsync(id, dto);
            return HandleResult(result);
        }

        /// <summary>
        /// Delete building
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin")]
        public async Task<IActionResult> DeleteBuilding(Guid id)
        {
            var result = await _buildingService.DeleteAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Activate building
        /// </summary>
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin")]
        public async Task<IActionResult> ActivateBuilding(Guid id)
        {
            var result = await _buildingService.ActivateAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Deactivate building
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin")]
        public async Task<IActionResult> DeactivateBuilding(Guid id)
        {
            var result = await _buildingService.DeactivateAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Get building statistics
        /// </summary>
        [HttpGet("{id}/statistics")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin,BuildingManager")]
        public async Task<IActionResult> GetBuildingStatistics(Guid id)
        {
            var result = await _buildingService.GetBuildingStatisticsAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Get buildings with vacant units
        /// </summary>
        [HttpGet("with-vacant-units")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin,BuildingManager")]
        public async Task<IActionResult> GetBuildingsWithVacantUnits()
        {
            var result = await _buildingService.GetBuildingsWithVacantUnitsAsync();
            return HandleResult(result);
        }

        #region Building Units

        /// <summary>
        /// Get building units
        /// </summary>
        [HttpGet("{buildingId}/units")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin,BuildingManager,Resident")]
        public async Task<IActionResult> GetBuildingUnits(Guid buildingId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _buildingService.GetBuildingUnitsAsync(buildingId, page, pageSize);
            return HandlePagedResult(result);
        }

        /// <summary>
        /// Get unit by ID
        /// </summary>
        [HttpGet("units/{unitId}")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin,BuildingManager,Resident")]
        public async Task<IActionResult> GetUnit(Guid unitId)
        {
            var result = await _buildingService.GetUnitByIdAsync(unitId);
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new unit
        /// </summary>
        [HttpPost("{buildingId}/units")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin,BuildingManager")]
        public async Task<IActionResult> CreateUnit(Guid buildingId, [FromBody] CreateBuildingUnitDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Ensure building ID matches route parameter
            dto.BuildingId = buildingId;

            var result = await _buildingService.CreateUnitAsync(dto);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetUnit), new { unitId = result.Data.Id }, new
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return HandleResult(result);
        }

        /// <summary>
        /// Update unit
        /// </summary>
        [HttpPut("units/{unitId}")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin,BuildingManager")]
        public async Task<IActionResult> UpdateUnit(Guid unitId, [FromBody] CreateBuildingUnitDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _buildingService.UpdateUnitAsync(unitId, dto);
            return HandleResult(result);
        }

        /// <summary>
        /// Delete unit
        /// </summary>
        [HttpDelete("units/{unitId}")]
        [Authorize(Roles = "SuperAdmin,TenantAdmin")]
        public async Task<IActionResult> DeleteUnit(Guid unitId)
        {
            var result = await _buildingService.DeleteUnitAsync(unitId);
            return HandleResult(result);
        }

        #endregion
    }
}