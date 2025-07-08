using NET.Application.Common.Models;
using NET.Application.DTOs.Maintenance;
using NET.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NET.Application.Services
{
    public interface IMaintenanceService
    {
        Task<Result<MaintenanceRequestDto>> GetByIdAsync(Guid id);
        Task<Result<PagedResult<MaintenanceRequestDto>>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<Result<List<MaintenanceRequestDto>>> GetByBuildingAsync(Guid buildingId);
        Task<Result<List<MaintenanceRequestDto>>> GetByUnitAsync(Guid unitId);
        Task<Result<List<MaintenanceRequestDto>>> GetByResidentAsync(Guid residentId);
        Task<Result<List<MaintenanceRequestDto>>> GetByStatusAsync(MaintenanceStatus status);
        Task<Result<List<MaintenanceRequestDto>>> GetByPriorityAsync(Priority priority);
        Task<Result<List<MaintenanceRequestDto>>> GetAssignedToUserAsync(Guid userId);
        //Task<Result<MaintenanceRequestDto>> CreateAsync(CreateMaintenanceRequestDto dto);
        //Task<Result<MaintenanceRequestDto>> UpdateAsync(Guid id, UpdateMaintenanceRequestDto dto);
        Task<Result> DeleteAsync(Guid id);

        // Status Management
        Task<Result> AssignToUserAsync(Guid requestId, Guid userId);
        Task<Result> StartWorkAsync(Guid requestId);
        Task<Result> CompleteWorkAsync(Guid requestId, string completionNotes, decimal? actualCost = null);
        Task<Result> CancelRequestAsync(Guid requestId, string reason);
        Task<Result> PutOnHoldAsync(Guid requestId, string reason);
        Task<Result> RejectRequestAsync(Guid requestId, string reason);

        // Approval
        Task<Result> ApproveRequestAsync(Guid requestId);
        Task<Result> RequestApprovalAsync(Guid requestId);

        // Reports
        Task<Result<MaintenanceStatisticsDto>> GetMaintenanceStatisticsAsync(DateTime fromDate, DateTime toDate);
        Task<Result<List<MaintenanceRequestDto>>> GetOverdueRequestsAsync();
        Task<Result<List<MaintenanceRequestDto>>> GetHighPriorityRequestsAsync();
    }
}