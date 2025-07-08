using AutoMapper;
using NET.Application.Common.Interfaces;
using NET.Application.Common.Models;
using NET.Application.DTOs.Building;
using NET.Domain.Entities;
using NET.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NET.Application.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantService _tenantService;
        private readonly ICurrentUserService _currentUserService;

        public BuildingService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITenantService tenantService,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantService = tenantService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<BuildingDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var building = await _unitOfWork.Buildings.GetFirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
                if (building == null)
                {
                    return Result<BuildingDto>.Failure("Building not found");
                }

                var buildingDto = _mapper.Map<BuildingDto>(building);

                // Calculate statistics
                var units = await _unitOfWork.BuildingUnits.GetAsync(u => u.BuildingId == id && !u.IsDeleted);
                buildingDto.OccupiedUnits = units.Count(u => u.IsOccupied);
                buildingDto.VacantUnits = units.Count(u => !u.IsOccupied);
                buildingDto.OccupancyRate = buildingDto.TotalUnits > 0 ?
                    (decimal)buildingDto.OccupiedUnits / buildingDto.TotalUnits * 100 : 0;

                return Result<BuildingDto>.Success(buildingDto);
            }
            catch (Exception ex)
            {
                return Result<BuildingDto>.Failure($"Error retrieving building: {ex.Message}");
            }
        }

        public async Task<Result<PagedResult<BuildingDto>>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var buildings = await _unitOfWork.Buildings.GetPagedAsync(
                    page, pageSize,
                    b => !b.IsDeleted);

                var buildingDtos = _mapper.Map<IEnumerable<BuildingDto>>(buildings.Data);

                var pagedResult = new PagedResult<BuildingDto>(
                    buildingDtos, page, pageSize, buildings.TotalCount);

                return Result<PagedResult<BuildingDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return Result<PagedResult<BuildingDto>>.Failure($"Error retrieving buildings: {ex.Message}");
            }
        }

        public async Task<Result<BuildingDto>> CreateAsync(CreateBuildingDto dto)
        {
            try
            {
                var building = _mapper.Map<Building>(dto);
                building.TenantId = _tenantService.GetCurrentTenantId();
                building.CreatedBy = _currentUserService.UserName;

                var createdBuilding = await _unitOfWork.Buildings.AddAsync(building);
                await _unitOfWork.SaveChangesAsync();

                var buildingDto = _mapper.Map<BuildingDto>(createdBuilding);
                return Result<BuildingDto>.Success(buildingDto, "Building created successfully");
            }
            catch (Exception ex)
            {
                return Result<BuildingDto>.Failure($"Error creating building: {ex.Message}");
            }
        }

        public async Task<Result<BuildingDto>> UpdateAsync(Guid id, UpdateBuildingDto dto)
        {
            try
            {
                var building = await _unitOfWork.Buildings.GetByIdAsync(id);
                if (building == null || building.IsDeleted)
                {
                    return Result<BuildingDto>.Failure("Building not found");
                }

                _mapper.Map(dto, building);
                building.UpdatedAt = DateTime.UtcNow;
                building.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Buildings.UpdateAsync(building);
                await _unitOfWork.SaveChangesAsync();

                var buildingDto = _mapper.Map<BuildingDto>(building);
                return Result<BuildingDto>.Success(buildingDto, "Building updated successfully");
            }
            catch (Exception ex)
            {
                return Result<BuildingDto>.Failure($"Error updating building: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            try
            {
                var building = await _unitOfWork.Buildings.GetByIdAsync(id);
                if (building == null || building.IsDeleted)
                {
                    return Result.Failure("Building not found");
                }

                // Check if building has units
                var hasUnits = await _unitOfWork.BuildingUnits.ExistsAsync(u => u.BuildingId == id && !u.IsDeleted);
                if (hasUnits)
                {
                    return Result.Failure("Cannot delete building with existing units");
                }

                building.IsDeleted = true;
                building.UpdatedAt = DateTime.UtcNow;
                building.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Buildings.UpdateAsync(building);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Building deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error deleting building: {ex.Message}");
            }
        }

        public async Task<Result<BuildingUnitDto>> CreateUnitAsync(CreateBuildingUnitDto dto)
        {
            try
            {
                // Validate building exists
                var buildingExists = await _unitOfWork.Buildings.ExistsAsync(b => b.Id == dto.BuildingId && !b.IsDeleted);
                if (!buildingExists)
                {
                    return Result<BuildingUnitDto>.Failure("Building not found");
                }

                // Check if unit number already exists in building
                var unitExists = await _unitOfWork.BuildingUnits.ExistsAsync(
                    u => u.BuildingId == dto.BuildingId && u.UnitNumber == dto.UnitNumber && !u.IsDeleted);
                if (unitExists)
                {
                    return Result<BuildingUnitDto>.Failure("Unit number already exists in this building");
                }

                var unit = _mapper.Map<BuildingUnit>(dto);
                unit.TenantId = _tenantService.GetCurrentTenantId();
                unit.CreatedBy = _currentUserService.UserName;

                var createdUnit = await _unitOfWork.BuildingUnits.AddAsync(unit);
                await _unitOfWork.SaveChangesAsync();

                var unitDto = _mapper.Map<BuildingUnitDto>(createdUnit);
                return Result<BuildingUnitDto>.Success(unitDto, "Unit created successfully");
            }
            catch (Exception ex)
            {
                return Result<BuildingUnitDto>.Failure($"Error creating unit: {ex.Message}");
            }
        }

        // Additional methods implementation...
        public async Task<Result<List<BuildingDto>>> GetActiveBuildingsAsync()
        {
            try
            {
                var buildings = await _unitOfWork.Buildings.GetAsync(b => b.IsActive && !b.IsDeleted);
                var buildingDtos = _mapper.Map<List<BuildingDto>>(buildings);
                return Result<List<BuildingDto>>.Success(buildingDtos);
            }
            catch (Exception ex)
            {
                return Result<List<BuildingDto>>.Failure($"Error retrieving active buildings: {ex.Message}");
            }
        }

        public async Task<Result> ActivateAsync(Guid id)
        {
            try
            {
                var building = await _unitOfWork.Buildings.GetByIdAsync(id);
                if (building == null || building.IsDeleted)
                {
                    return Result.Failure("Building not found");
                }

                building.IsActive = true;
                building.UpdatedAt = DateTime.UtcNow;
                building.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Buildings.UpdateAsync(building);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Building activated successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error activating building: {ex.Message}");
            }
        }

        public async Task<Result> DeactivateAsync(Guid id)
        {
            try
            {
                var building = await _unitOfWork.Buildings.GetByIdAsync(id);
                if (building == null || building.IsDeleted)
                {
                    return Result.Failure("Building not found");
                }

                building.IsActive = false;
                building.UpdatedAt = DateTime.UtcNow;
                building.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Buildings.UpdateAsync(building);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Building deactivated successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error deactivating building: {ex.Message}");
            }
        }

        public async Task<Result<PagedResult<BuildingUnitDto>>> GetBuildingUnitsAsync(Guid buildingId, int page = 1, int pageSize = 10)
        {
            try
            {
                var units = await _unitOfWork.BuildingUnits.GetPagedAsync(
                    page, pageSize,
                    u => u.BuildingId == buildingId && !u.IsDeleted);

                var unitDtos = _mapper.Map<IEnumerable<BuildingUnitDto>>(units.Data);
                var pagedResult = new PagedResult<BuildingUnitDto>(unitDtos, page, pageSize, units.TotalCount);

                return Result<PagedResult<BuildingUnitDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return Result<PagedResult<BuildingUnitDto>>.Failure($"Error retrieving building units: {ex.Message}");
            }
        }

        public async Task<Result<BuildingUnitDto>> GetUnitByIdAsync(Guid unitId)
        {
            try
            {
                var unit = await _unitOfWork.BuildingUnits.GetByIdAsync(unitId);
                if (unit == null || unit.IsDeleted)
                {
                    return Result<BuildingUnitDto>.Failure("Unit not found");
                }

                var unitDto = _mapper.Map<BuildingUnitDto>(unit);
                return Result<BuildingUnitDto>.Success(unitDto);
            }
            catch (Exception ex)
            {
                return Result<BuildingUnitDto>.Failure($"Error retrieving unit: {ex.Message}");
            }
        }

        public async Task<Result<BuildingUnitDto>> UpdateUnitAsync(Guid unitId, CreateBuildingUnitDto dto)
        {
            try
            {
                var unit = await _unitOfWork.BuildingUnits.GetByIdAsync(unitId);
                if (unit == null || unit.IsDeleted)
                {
                    return Result<BuildingUnitDto>.Failure("Unit not found");
                }

                _mapper.Map(dto, unit);
                unit.UpdatedAt = DateTime.UtcNow;
                unit.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.BuildingUnits.UpdateAsync(unit);
                await _unitOfWork.SaveChangesAsync();

                var unitDto = _mapper.Map<BuildingUnitDto>(unit);
                return Result<BuildingUnitDto>.Success(unitDto, "Unit updated successfully");
            }
            catch (Exception ex)
            {
                return Result<BuildingUnitDto>.Failure($"Error updating unit: {ex.Message}");
            }
        }

        public async Task<Result> DeleteUnitAsync(Guid unitId)
        {
            try
            {
                var unit = await _unitOfWork.BuildingUnits.GetByIdAsync(unitId);
                if (unit == null || unit.IsDeleted)
                {
                    return Result.Failure("Unit not found");
                }

                // Check if unit has residents
                var hasResidents = await _unitOfWork.ResidentUnits.ExistsAsync(ru => ru.BuildingUnitId == unitId && ru.IsActive);
                if (hasResidents)
                {
                    return Result.Failure("Cannot delete unit with active residents");
                }

                unit.IsDeleted = true;
                unit.UpdatedAt = DateTime.UtcNow;
                unit.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.BuildingUnits.UpdateAsync(unit);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Unit deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error deleting unit: {ex.Message}");
            }
        }

        public async Task<Result<BuildingStatisticsDto>> GetBuildingStatisticsAsync(Guid buildingId)
        {
            try
            {
                var building = await _unitOfWork.Buildings.GetByIdAsync(buildingId);
                if (building == null || building.IsDeleted)
                {
                    return Result<BuildingStatisticsDto>.Failure("Building not found");
                }

                var units = await _unitOfWork.BuildingUnits.GetAsync(u => u.BuildingId == buildingId && !u.IsDeleted);
                var occupiedUnits = units.Count(u => u.IsOccupied);
                var vacantUnits = units.Count(u => !u.IsOccupied);

                var maintenanceRequests = await _unitOfWork.MaintenanceRequests.CountAsync(
                    mr => mr.BuildingId == buildingId && mr.Status == Domain.Enums.MaintenanceStatus.Open);

                var monthlyExpenses = await _unitOfWork.Expenses.GetAsync(
                    e => e.BuildingId == buildingId &&
                         e.ExpenseDate >= DateTime.Now.AddDays(-30) &&
                         !e.IsDeleted);

                var statistics = new BuildingStatisticsDto
                {
                    TotalUnits = building.TotalUnits,
                    OccupiedUnits = occupiedUnits,
                    VacantUnits = vacantUnits,
                    OccupancyRate = building.TotalUnits > 0 ? (decimal)occupiedUnits / building.TotalUnits * 100 : 0,
                    TotalMonthlyDues = units.Sum(u => u.MonthlyDues ?? 0),
                    PendingMaintenanceRequests = maintenanceRequests,
                    MonthlyExpenses = monthlyExpenses.Sum(e => e.Amount)
                };

                return Result<BuildingStatisticsDto>.Success(statistics);
            }
            catch (Exception ex)
            {
                return Result<BuildingStatisticsDto>.Failure($"Error retrieving building statistics: {ex.Message}");
            }
        }

        public async Task<Result<List<BuildingDto>>> GetBuildingsWithVacantUnitsAsync()
        {
            try
            {
                var buildings = await _unitOfWork.Buildings.GetAsync(b => b.IsActive && !b.IsDeleted);
                var buildingsWithVacantUnits = new List<BuildingDto>();

                foreach (var building in buildings)
                {
                    var units = await _unitOfWork.BuildingUnits.GetAsync(u => u.BuildingId == building.Id && !u.IsDeleted);
                    if (units.Any(u => !u.IsOccupied))
                    {
                        var buildingDto = _mapper.Map<BuildingDto>(building);
                        buildingDto.VacantUnits = units.Count(u => !u.IsOccupied);
                        buildingsWithVacantUnits.Add(buildingDto);
                    }
                }

                return Result<List<BuildingDto>>.Success(buildingsWithVacantUnits);
            }
            catch (Exception ex)
            {
                return Result<List<BuildingDto>>.Failure($"Error retrieving buildings with vacant units: {ex.Message}");
            }
        }
    }
}