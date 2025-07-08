using NET.Application.Common.Models;
using NET.Application.DTOs.Resident;

namespace NET.Application.Services
{
    public interface IResidentService
    {
        Task<Result<ResidentDto>> GetByIdAsync(Guid id);
        Task<Result<PagedResult<ResidentDto>>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<Result<List<ResidentDto>>> GetActiveResidentsAsync();
        Task<Result<List<ResidentDto>>> GetResidentsByBuildingAsync(Guid buildingId);
        Task<Result<List<ResidentDto>>> GetResidentsByUnitAsync(Guid unitId);
        Task<Result<ResidentDto>> CreateAsync(CreateResidentDto dto);
        //Task<Result<ResidentDto>> UpdateAsync(Guid id, UpdateResidentDto dto);
        Task<Result> DeleteAsync(Guid id);
        Task<Result> ActivateAsync(Guid id);
        Task<Result> DeactivateAsync(Guid id);

        // Unit Assignment
        Task<Result> AssignToUnitAsync(Guid residentId, Guid unitId, bool isOwner = false, bool isPrimary = true);
        Task<Result> RemoveFromUnitAsync(Guid residentId, Guid unitId);
        Task<Result> MoveToUnitAsync(Guid residentId, Guid fromUnitId, Guid toUnitId);

        // Search
        Task<Result<List<ResidentDto>>> SearchResidentsAsync(string searchTerm);
        Task<Result<List<ResidentDto>>> GetResidentsWithoutUnitsAsync();
    }
}