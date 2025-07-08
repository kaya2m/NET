using NET.Application.Common.Models;
using NET.Application.DTOs.Building;

public interface IBuildingService
{
    Task<Result<BuildingDto>> GetByIdAsync(Guid id);
    Task<Result<PagedResult<BuildingDto>>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<Result<List<BuildingDto>>> GetActiveBuildingsAsync();
    Task<Result<BuildingDto>> CreateAsync(CreateBuildingDto dto);
    Task<Result<BuildingDto>> UpdateAsync(Guid id, UpdateBuildingDto dto);
    Task<Result> DeleteAsync(Guid id);
    Task<Result> ActivateAsync(Guid id);
    Task<Result> DeactivateAsync(Guid id);

    // Building Units
    Task<Result<PagedResult<BuildingUnitDto>>> GetBuildingUnitsAsync(Guid buildingId, int page = 1, int pageSize = 10);
    Task<Result<BuildingUnitDto>> GetUnitByIdAsync(Guid unitId);
    Task<Result<BuildingUnitDto>> CreateUnitAsync(CreateBuildingUnitDto dto);
    Task<Result<BuildingUnitDto>> UpdateUnitAsync(Guid unitId, CreateBuildingUnitDto dto);
    Task<Result> DeleteUnitAsync(Guid unitId);

    // Statistics
    Task<Result<BuildingStatisticsDto>> GetBuildingStatisticsAsync(Guid buildingId);
    Task<Result<List<BuildingDto>>> GetBuildingsWithVacantUnitsAsync();
}