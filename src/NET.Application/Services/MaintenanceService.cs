//using AutoMapper;
//using NET.Application.Common.Interfaces;
//using NET.Application.Common.Models;
//using NET.Application.DTOs.Maintenance;
//using NET.Domain.Entities;
//using NET.Domain.Enums;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace NET.Application.Services
//{
//    public class MaintenanceService : IMaintenanceService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//        private readonly ICurrentUserService _currentUserService;

//        public MaintenanceService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//            _currentUserService = currentUserService;
//        }

//        public async Task<Result<MaintenanceRequestDto>> GetByIdAsync(Guid id)
//        {
//            try
//            {
//                var request = await _unitOfWork.Repository<MaintenanceRequest>().GetByIdAsync(id);
//                if (request == null)
//                {
//                    return Result<MaintenanceRequestDto>.Failure("Maintenance request not found");
//                }

//                var dto = _mapper.Map<MaintenanceRequestDto>(request);
//                return Result<MaintenanceRequestDto>.Success(dto);
//            }
//            catch (Exception ex)
//            {
//                return Result<MaintenanceRequestDto>.Failure($"Error retrieving maintenance request: {ex.Message}");
//            }
//        }

//        public async Task<Result<PagedResult<MaintenanceRequestDto>>> GetAllAsync(int page = 1, int pageSize = 10)
//        {
//            try
//            {
//                var requests = await _unitOfWork.Repository<MaintenanceRequest>().GetAllAsync();
//                var totalCount = requests.Count();
//                var pagedRequests = requests.Skip((page - 1) * pageSize).Take(pageSize).ToList();

//                var dtos = _mapper.Map<List<MaintenanceRequestDto>>(pagedRequests);
//                var pagedResult = new PagedResult<MaintenanceRequestDto>
//                {
//                    Items = dtos,
//                    PageNumber = page,
//                    PageSize = pageSize,
//                    TotalCount = totalCount,
//                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
//                };

//                return Result<PagedResult<MaintenanceRequestDto>>.Success(pagedResult);
//            }
//            catch (Exception ex)
//            {
//                return Result<PagedResult<MaintenanceRequestDto>>.Failure($"Error retrieving maintenance requests: {ex.Message}");
//            }
//        }

//        public async Task<Result<List<MaintenanceRequestDto>>> GetByBuildingAsync(Guid buildingId)
//        {
//            try
//            {
//                var requests = await _unitOfWork.Repository<MaintenanceRequest>()
//                    .FindAsync(r => r.BuildingUnit.BuildingId == buildingId);

//                var dtos = _mapper.Map<List<MaintenanceRequestDto>>(requests);
//                return Result<List<MaintenanceRequestDto>>.Success(dtos);
//            }
//            catch (Exception ex)
//            {
//                return Result<List<MaintenanceRequestDto>>.Failure($"Error retrieving maintenance requests: {ex.Message}");
//            }
//        }

//        public async Task<Result<List<MaintenanceRequestDto>>> GetByUnitAsync(Guid unitId)
//        {
//            try
//            {
//                var requests = await _unitOfWork.Repository<MaintenanceRequest>()
//                    .FindAsync(r => r.UnitId == unitId);

//                var dtos = _mapper.Map<List<MaintenanceRequestDto>>(requests);
//                return Result<List<MaintenanceRequestDto>>.Success(dtos);
//            }
//            catch (Exception ex)
//            {
//                return Result<List<MaintenanceRequestDto>>.Failure($"Error retrieving maintenance requests: {ex.Message}");
//            }
//        }

//        public async Task<Result<List<MaintenanceRequestDto>>> GetByResidentAsync(Guid residentId)
//        {
//            try
//            {
//                var requests = await _unitOfWork.Repository<MaintenanceRequest>()
//                    .FindAsync(r => r.RequestedById == residentId);

//                var dtos = _mapper.Map<List<MaintenanceRequestDto>>(requests);
//                return Result<List<MaintenanceRequestDto>>.Success(dtos);
//            }
//            catch (Exception ex)
//            {
//                return Result<List<MaintenanceRequestDto>>.Failure($"Error retrieving maintenance requests: {ex.Message}");
//            }
//        }

//        public async Task<Result<List<MaintenanceRequestDto>>> GetByStatusAsync(MaintenanceStatus status)
//        {
//            try
//            {
//                var requests = await _unitOfWork.Repository<MaintenanceRequest>()
//                    .FindAsync(r => r.Status == status);

//                var dtos = _mapper.Map<List<MaintenanceRequestDto>>(requests);
//                return Result<List<MaintenanceRequestDto>>.Success(dtos);
//            }
//            catch (Exception ex)
//            {
//                return Result<List<MaintenanceRequestDto>>.Failure($"Error retrieving maintenance requests: {ex.Message}");
//            }
//        }

//        public async Task<Result<List<MaintenanceRequestDto>>> GetByPriorityAsync(Priority priority)
//        {
//            try
//            {
//                var requests = await _unitOfWork.Repository<MaintenanceRequest>()
//                    .FindAsync(r => r.Priority == priority);

//                var dtos = _mapper.Map<List<MaintenanceRequestDto>>(requests);
//                return Result<List<MaintenanceRequestDto>>.Success(dtos);
//            }
//            catch (Exception ex)
//            {
//                return Result<List<MaintenanceRequestDto>>.Failure($"Error retrieving maintenance requests: {ex.Message}");
//            }
//        }

//        public async Task<Result<List<MaintenanceRequestDto>>> GetAssignedToUserAsync(Guid userId)
//        {
//            try
//            {
//                var requests = await _unitOfWork.Repository<MaintenanceRequest>()
//                    .FindAsync(r => r.AssignedToId == userId);

//                var dtos = _mapper.Map<List<MaintenanceRequestDto>>(requests);
//                return Result<List<MaintenanceRequestDto>>.Success(dtos);
//            }
//            catch (Exception ex)
//            {
//                return Result<List<MaintenanceRequestDto>>.Failure($"Error retrieving maintenance requests: {ex.Message}");
//            }
//        }

//        public async Task<Result> DeleteAsync(Guid id)
//        {
//            try
//            {
//                var request = await _unitOfWork.Repository<MaintenanceRequest>().GetByIdAsync(id);
//                if (request == null)
//                {
//                    return Result.Failure("Maintenance request not found");
//                }

//                await _unitOfWork.Repository<MaintenanceRequest>().DeleteAsync(request);
//                await _unitOfWork.SaveChangesAsync();

//                return Result.Success();
//            }
//            catch (Exception ex)
//            {
//                return Result.Failure($"Error deleting maintenance request: {ex.Message}");
//            }
//        }

//        public async Task<Result> AssignToUserAsync(Guid requestId, Guid userId)
//        {
//            try
//            {
//                var request = await _unitOfWork.Repository<MaintenanceRequest>().GetByIdAsync(requestId);
//                if (request == null)
//                {
//                    return Result.Failure("Maintenance request not found");
//                }

//                var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
//                if (user == null)
//                {
//                    return Result.Failure("User not found");
//                }

//                request.AssignedToId = userId;
//                request.Status = MaintenanceStatus.Assigned;
//                request.UpdatedAt = DateTime.UtcNow;

//                await _unitOfWork.Repository<MaintenanceRequest>().UpdateAsync(request);
//                await _unitOfWork.SaveChangesAsync();

//                return Result.Success();
//            }
//            catch (Exception ex)
//            {
//                return Result.Failure($"Error assigning maintenance request: {ex.Message}");
//            }
//        }

//        public async Task<Result> StartWorkAsync(Guid requestId)
//        {
//            try
//            {
//                var request = await _unitOfWork.Repository<MaintenanceRequest>().GetByIdAsync(requestId);
//                if (request == null)
//                {
//                    return Result.Failure("Maintenance request not found");
//                }

//                if (request.Status != MaintenanceStatus.Assigned)
//                {
//                    return Result.Failure("Request must be assigned before starting work");
//                }

//                request.Status = MaintenanceStatus.InProgress;
//                request.StartedDate = DateTime.UtcNow;
//                request.UpdatedAt = DateTime.UtcNow;

//                await _unitOfWork.Repository<MaintenanceRequest>().UpdateAsync(request);
//                await _unitOfWork.SaveChangesAsync();

//                return Result.Success();
//            }
//            catch (Exception ex)
//            {
//                return Result.Failure($"Error starting maintenance work: {ex.Message}");
//            }
//        }

//        public async Task<Result> CompleteWorkAsync(Guid requestId, string completionNotes, decimal? actualCost = null)
//        {
//            try
//            {
//                var request = await _unitOfWork.Repository<MaintenanceRequest>().GetByIdAsync(requestId);
//                if (request == null)
//                {
//                    return Result.Failure("Maintenance request not found");
//                }

//                if (request.Status != MaintenanceStatus.InProgress)
//                {
//                    return Result.Failure("Request must be in progress to complete");
//                }

//                request.Status = MaintenanceStatus.Completed;
//                request.CompletedDate = DateTime.UtcNow;
//                request.CompletionNotes = completionNotes;
//                request.ActualCost = actualCost;
//                request.UpdatedAt = DateTime.UtcNow;

//                await _unitOfWork.Repository<MaintenanceRequest>().UpdateAsync(request);
//                await _unitOfWork.SaveChangesAsync();

//                return Result.Success();
//            }
//            catch (Exception ex)
//            {
//                return Result.Failure($"Error completing maintenance work: {ex.Message}");
//            }
//        }

//        public async Task<Result> CancelRequestAsync(Guid requestId, string reason)
//        {
//            try
//            {
//                var request = await _unitOfWork.Repository<MaintenanceRequest>().GetByIdAsync(requestId);
//                if (request == null)
//                {
//                    return Result.Failure("Maintenance request not found");
//                }

//                request.Status = MaintenanceStatus.Cancelled;
//                request.CancellationReason = reason;
//                request.UpdatedAt = DateTime.UtcNow;

//                await _unitOfWork.Repository<MaintenanceRequest>().UpdateAsync(request);
//                await _unitOfWork.SaveChangesAsync();

//                return Result.Success();
//            }
//            catch (Exception ex)
//            {
//                return Result.Failure($"Error cancelling maintenance request: {ex.Message}");
//            }
//        }

//        public async Task<Result> PutOnHoldAsync(Guid requestId, string reason)
//        {
//            try
//            {
//                var request = await _unitOfWork.Repository<MaintenanceRequest>().GetByIdAsync(requestId);
//                if (request == null)
//                {
//                    return Result.Failure("Maintenance request not found");
//                }

//                request.Status = MaintenanceStatus.OnHold;
//                request.HoldReason = reason;
//                request.UpdatedAt = DateTime.UtcNow;

//                await _unitOfWork.Repository<MaintenanceRequest>().UpdateAsync(request);
//                await _unitOfWork.SaveChangesAsync();

//                return Result.Success();
//            }
//            catch (Exception ex)
//            {
//                return Result.Failure($"Error putting maintenance request on hold: {ex.Message}");
//            }
//        }

//        public async Task<Result> RejectRequestAsync(Guid requestId, string reason)
//        {
//            try
//            {
//                var request = await _unitOfWork.Repository<MaintenanceRequest>().GetByIdAsync(requestId);
//                if (request == null)
//                {
//                    return Result.Failure("Maintenance request not found");
//                }

//                request.Status = MaintenanceStatus.Rejected;
//                request.RejectionReason = reason;
//                request.UpdatedAt = DateTime.UtcNow;

//                await _unitOfWork.Repository<MaintenanceRequest>().UpdateAsync(request);
//                await _unitOfWork.SaveChangesAsync();

//                return Result.Success();
//            }
//            catch (Exception ex)
//            {
//                return Result.Failure($"Error rejecting maintenance request: {ex.Message}");
//            }
//        }

//        public async Task<Result> ApproveRequestAsync(Guid requestId)
//        {
//            try
//            {
//                var request = await _unitOfWork.Repository<MaintenanceRequest>().GetByIdAsync(requestId);
//                if (request == null)
//                {
//                    return Result.Failure("Maintenance request not found");
//                }

//                request.Status = MaintenanceStatus.Approved;
//                request.ApprovedDate = DateTime.UtcNow;
//                request.UpdatedAt = DateTime.UtcNow;

//                await _unitOfWork.Repository<MaintenanceRequest>().UpdateAsync(request);
//                await _unitOfWork.SaveChangesAsync();

//                return Result.Success();
//            }
//            catch (Exception ex)
//            {
//                return Result.Failure($"Error approving maintenance request: {ex.Message}");
//            }
//        }

//        public async Task<Result> RequestApprovalAsync(Guid requestId)
//        {
//            try
//            {
//                var request = await _unitOfWork.Repository<MaintenanceRequest>().GetByIdAsync(requestId);
//                if (request == null)
//                {
//                    return Result.Failure("Maintenance request not found");
//                }

//                request.Status = MaintenanceStatus.PendingApproval;
//                request.UpdatedAt = DateTime.UtcNow;

//                await _unitOfWork.Repository<MaintenanceRequest>().UpdateAsync(request);
//                await _unitOfWork.SaveChangesAsync();

//                return Result.Success();
//            }
//            catch (Exception ex)
//            {
//                return Result.Failure($"Error requesting approval: {ex.Message}");
//            }
//        }

//        public async Task<Result<MaintenanceStatisticsDto>> GetMaintenanceStatisticsAsync(DateTime fromDate, DateTime toDate)
//        {
//            try
//            {
//                var requests = await _unitOfWork.Repository<MaintenanceRequest>()
//                    .FindAsync(r => r.RequestedDate >= fromDate && r.RequestedDate <= toDate);

//                var statistics = new MaintenanceStatisticsDto
//                {
//                    TotalRequests = requests.Count(),
//                    PendingRequests = requests.Count(r => r.Status == MaintenanceStatus.Pending),
//                    InProgressRequests = requests.Count(r => r.Status == MaintenanceStatus.InProgress),
//                    CompletedRequests = requests.Count(r => r.Status == MaintenanceStatus.Completed),
//                    CancelledRequests = requests.Count(r => r.Status == MaintenanceStatus.Cancelled),
//                    HighPriorityRequests = requests.Count(r => r.Priority == Priority.High),
//                    MediumPriorityRequests = requests.Count(r => r.Priority == Priority.Medium),
//                    LowPriorityRequests = requests.Count(r => r.Priority == Priority.Low),
//                    AverageResolutionTime = requests.Where(r => r.CompletedDate.HasValue && r.StartedDate.HasValue)
//                        .Select(r => (r.CompletedDate.Value - r.StartedDate.Value).TotalDays)
//                        .DefaultIfEmpty(0)
//                        .Average(),
//                    TotalCost = requests.Where(r => r.ActualCost.HasValue).Sum(r => r.ActualCost.Value)
//                };

//                return Result<MaintenanceStatisticsDto>.Success(statistics);
//            }
//            catch (Exception ex)
//            {
//                return Result<MaintenanceStatisticsDto>.Failure($"Error retrieving maintenance statistics: {ex.Message}");
//            }
//        }

//        public async Task<Result<List<MaintenanceRequestDto>>> GetOverdueRequestsAsync()
//        {
//            try
//            {
//                var requests = await _unitOfWork.Repository<MaintenanceRequest>()
//                    .FindAsync(r => r.PreferredDate.HasValue && 
//                                  r.PreferredDate.Value < DateTime.UtcNow && 
//                                  r.Status != MaintenanceStatus.Completed &&
//                                  r.Status != MaintenanceStatus.Cancelled);

//                var dtos = _mapper.Map<List<MaintenanceRequestDto>>(requests);
//                return Result<List<MaintenanceRequestDto>>.Success(dtos);
//            }
//            catch (Exception ex)
//            {
//                return Result<List<MaintenanceRequestDto>>.Failure($"Error retrieving overdue requests: {ex.Message}");
//            }
//        }

//        public async Task<Result<List<MaintenanceRequestDto>>> GetHighPriorityRequestsAsync()
//        {
//            try
//            {
//                var requests = await _unitOfWork.Repository<MaintenanceRequest>()
//                    .FindAsync(r => r.Priority == Priority.High && 
//                                  r.Status != MaintenanceStatus.Completed &&
//                                  r.Status != MaintenanceStatus.Cancelled);

//                var dtos = _mapper.Map<List<MaintenanceRequestDto>>(requests);
//                return Result<List<MaintenanceRequestDto>>.Success(dtos);
//            }
//            catch (Exception ex)
//            {
//                return Result<List<MaintenanceRequestDto>>.Failure($"Error retrieving high priority requests: {ex.Message}");
//            }
//        }
//    }
//}