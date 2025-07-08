using AutoMapper;
using NET.Application.Common.Interfaces;
using NET.Application.Common.Models;
using NET.Application.DTOs.Resident;
using NET.Domain.Entities;
using NET.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NET.Application.Services
{
    public class ResidentService : IResidentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantService _tenantService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;

        public ResidentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITenantService tenantService,
            ICurrentUserService currentUserService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantService = tenantService;
            _currentUserService = currentUserService;
            _emailService = emailService;
        }

        public async Task<Result<ResidentDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var resident = await _unitOfWork.Residents.GetFirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
                if (resident == null)
                {
                    return Result<ResidentDto>.Failure("Resident not found");
                }

                var residentDto = _mapper.Map<ResidentDto>(resident);
                return Result<ResidentDto>.Success(residentDto);
            }
            catch (Exception ex)
            {
                return Result<ResidentDto>.Failure($"Error retrieving resident: {ex.Message}");
            }
        }

        public async Task<Result<PagedResult<ResidentDto>>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var residents = await _unitOfWork.Residents.GetPagedAsync(
                    page, pageSize,
                    r => !r.IsDeleted);

                var residentDtos = _mapper.Map<IEnumerable<ResidentDto>>(residents.Data);
                var pagedResult = new PagedResult<ResidentDto>(residentDtos, page, pageSize, residents.TotalCount);

                return Result<PagedResult<ResidentDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return Result<PagedResult<ResidentDto>>.Failure($"Error retrieving residents: {ex.Message}");
            }
        }

        public async Task<Result<ResidentDto>> CreateAsync(CreateResidentDto dto)
        {
            try
            {
                // Check if email already exists
                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var emailExists = await _unitOfWork.Residents.ExistsAsync(r => r.Email == dto.Email && !r.IsDeleted);
                    if (emailExists)
                    {
                        return Result<ResidentDto>.Failure("Email address already exists");
                    }
                }

                var resident = _mapper.Map<Resident>(dto);
                resident.TenantId = _tenantService.GetCurrentTenantId();
                resident.CreatedBy = _currentUserService.UserName;
                resident.IsActive = true;

                var createdResident = await _unitOfWork.Residents.AddAsync(resident);
                await _unitOfWork.SaveChangesAsync();

                var residentDto = _mapper.Map<ResidentDto>(createdResident);

                // Send welcome email if email provided
                if (!string.IsNullOrEmpty(dto.Email))
                {
                    await _emailService.SendWelcomeEmailAsync(dto.Email, residentDto.FullName, "N/A");
                }

                return Result<ResidentDto>.Success(residentDto, "Resident created successfully");
            }
            catch (Exception ex)
            {
                return Result<ResidentDto>.Failure($"Error creating resident: {ex.Message}");
            }
        }

        //public async Task<Result<ResidentDto>> UpdateAsync(Guid id, UpdateResidentDto dto)
        //{
        //    try
        //    {
        //        var resident = await _unitOfWork.Residents.GetByIdAsync(id);
        //        if (resident == null || resident.IsDeleted)
        //        {
        //            return Result<ResidentDto>.Failure("Resident not found");
        //        }

        //        // Check email uniqueness if changed
        //        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != resident.Email)
        //        {
        //            var emailExists = await _unitOfWork.Residents.ExistsAsync(r => r.Email == dto.Email && r.Id != id && !r.IsDeleted);
        //            if (emailExists)
        //            {
        //                return Result<ResidentDto>.Failure("Email address already exists");
        //            }
        //        }

        //        _mapper.Map(dto, resident);
        //        resident.UpdatedAt = DateTime.UtcNow;
        //        resident.UpdatedBy = _currentUserService.UserName;

        //        await _unitOfWork.Residents.UpdateAsync(resident);
        //        await _unitOfWork.SaveChangesAsync();

        //        var residentDto = _mapper.Map<ResidentDto>(resident);
        //        return Result<ResidentDto>.Success(residentDto, "Resident updated successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Result<ResidentDto>.Failure($"Error updating resident: {ex.Message}");
        //    }
        //}

        public async Task<Result> AssignToUnitAsync(Guid residentId, Guid unitId, bool isOwner = false, bool isPrimary = true)
        {
            try
            {
                // Validate resident exists
                var resident = await _unitOfWork.Residents.GetByIdAsync(residentId);
                if (resident == null || resident.IsDeleted)
                {
                    return Result.Failure("Resident not found");
                }

                // Validate unit exists
                var unit = await _unitOfWork.BuildingUnits.GetByIdAsync(unitId);
                if (unit == null || unit.IsDeleted)
                {
                    return Result.Failure("Unit not found");
                }

                // Check if resident is already assigned to this unit
                var existingAssignment = await _unitOfWork.ResidentUnits.GetFirstOrDefaultAsync(
                    ru => ru.ResidentId == residentId && ru.BuildingUnitId == unitId && ru.IsActive);
                if (existingAssignment != null)
                {
                    return Result.Failure("Resident is already assigned to this unit");
                }

                // If this is primary resident, set other assignments as non-primary
                if (isPrimary)
                {
                    var otherAssignments = await _unitOfWork.ResidentUnits.GetAsync(
                        ru => ru.ResidentId == residentId && ru.IsActive);
                    foreach (var assignment in otherAssignments)
                    {
                        assignment.IsPrimaryResident = false;
                        await _unitOfWork.ResidentUnits.UpdateAsync(assignment);
                    }
                }

                // Create new assignment
                var residentUnit = new ResidentUnit
                {
                    TenantId = _tenantService.GetCurrentTenantId(),
                    ResidentId = residentId,
                    BuildingUnitId = unitId,
                    StartDate = DateTime.UtcNow,
                    IsActive = true,
                    IsOwner = isOwner,
                    IsPrimaryResident = isPrimary,
                    CreatedBy = _currentUserService.UserName
                };

                await _unitOfWork.ResidentUnits.AddAsync(residentUnit);

                // Update unit occupancy status
                unit.IsOccupied = true;
                unit.LastOccupiedDate = DateTime.UtcNow;
                unit.UpdatedBy = _currentUserService.UserName;
                await _unitOfWork.BuildingUnits.UpdateAsync(unit);

                // Update resident move-in date if not set
                if (resident.MoveInDate == null)
                {
                    resident.MoveInDate = DateTime.UtcNow;
                    resident.UpdatedBy = _currentUserService.UserName;
                    await _unitOfWork.Residents.UpdateAsync(resident);
                }

                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Resident assigned to unit successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error assigning resident to unit: {ex.Message}");
            }
        }

        public async Task<Result> RemoveFromUnitAsync(Guid residentId, Guid unitId)
        {
            try
            {
                var residentUnit = await _unitOfWork.ResidentUnits.GetFirstOrDefaultAsync(
                    ru => ru.ResidentId == residentId && ru.BuildingUnitId == unitId && ru.IsActive);

                if (residentUnit == null)
                {
                    return Result.Failure("Resident unit assignment not found");
                }

                // End the assignment
                residentUnit.IsActive = false;
                residentUnit.EndDate = DateTime.UtcNow;
                residentUnit.UpdatedBy = _currentUserService.UserName;
                await _unitOfWork.ResidentUnits.UpdateAsync(residentUnit);

                // Check if unit has any other active residents
                var otherResidents = await _unitOfWork.ResidentUnits.ExistsAsync(
                    ru => ru.BuildingUnitId == unitId && ru.IsActive && ru.Id != residentUnit.Id);

                // Update unit occupancy if no other residents
                if (!otherResidents)
                {
                    var unit = await _unitOfWork.BuildingUnits.GetByIdAsync(unitId);
                    if (unit != null)
                    {
                        unit.IsOccupied = false;
                        unit.UpdatedBy = _currentUserService.UserName;
                        await _unitOfWork.BuildingUnits.UpdateAsync(unit);
                    }
                }

                // Update resident move-out date if this was their primary residence
                if (residentUnit.IsPrimaryResident)
                {
                    var resident = await _unitOfWork.Residents.GetByIdAsync(residentId);
                    if (resident != null)
                    {
                        resident.MoveOutDate = DateTime.UtcNow;
                        resident.UpdatedBy = _currentUserService.UserName;
                        await _unitOfWork.Residents.UpdateAsync(resident);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Resident removed from unit successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error removing resident from unit: {ex.Message}");
            }
        }

        public async Task<Result<List<ResidentDto>>> SearchResidentsAsync(string searchTerm)
        {
            try
            {
                var residents = await _unitOfWork.Residents.GetAsync(r =>
                    !r.IsDeleted &&
                    (r.FirstName.Contains(searchTerm) ||
                     r.LastName.Contains(searchTerm) ||
                     (r.Email != null && r.Email.Contains(searchTerm)) ||
                     (r.Phone != null && r.Phone.Contains(searchTerm))));

                var residentDtos = _mapper.Map<List<ResidentDto>>(residents);
                return Result<List<ResidentDto>>.Success(residentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<ResidentDto>>.Failure($"Error searching residents: {ex.Message}");
            }
        }

        public async Task<Result<List<ResidentDto>>> GetActiveResidentsAsync()
        {
            try
            {
                var residents = await _unitOfWork.Residents.GetAsync(r => r.IsActive && !r.IsDeleted);
                var residentDtos = _mapper.Map<List<ResidentDto>>(residents);
                return Result<List<ResidentDto>>.Success(residentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<ResidentDto>>.Failure($"Error retrieving active residents: {ex.Message}");
            }
        }

        public async Task<Result<List<ResidentDto>>> GetResidentsByBuildingAsync(Guid buildingId)
        {
            try
            {
                var residentUnits = await _unitOfWork.ResidentUnits.GetAsync(ru =>
                    ru.BuildingUnit.BuildingId == buildingId && ru.IsActive);

                var residentIds = residentUnits.Select(ru => ru.ResidentId).Distinct();
                var residents = await _unitOfWork.Residents.GetAsync(r =>
                    residentIds.Contains(r.Id) && !r.IsDeleted);

                var residentDtos = _mapper.Map<List<ResidentDto>>(residents);
                return Result<List<ResidentDto>>.Success(residentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<ResidentDto>>.Failure($"Error retrieving residents by building: {ex.Message}");
            }
        }

        public async Task<Result<List<ResidentDto>>> GetResidentsByUnitAsync(Guid unitId)
        {
            try
            {
                var residentUnits = await _unitOfWork.ResidentUnits.GetAsync(ru =>
                    ru.BuildingUnitId == unitId && ru.IsActive);

                var residentIds = residentUnits.Select(ru => ru.ResidentId);
                var residents = await _unitOfWork.Residents.GetAsync(r =>
                    residentIds.Contains(r.Id) && !r.IsDeleted);

                var residentDtos = _mapper.Map<List<ResidentDto>>(residents);
                return Result<List<ResidentDto>>.Success(residentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<ResidentDto>>.Failure($"Error retrieving residents by unit: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            try
            {
                var resident = await _unitOfWork.Residents.GetByIdAsync(id);
                if (resident == null || resident.IsDeleted)
                {
                    return Result.Failure("Resident not found");
                }

                // Check if resident has active unit assignments
                var hasActiveUnits = await _unitOfWork.ResidentUnits.ExistsAsync(ru => ru.ResidentId == id && ru.IsActive);
                if (hasActiveUnits)
                {
                    return Result.Failure("Cannot delete resident with active unit assignments");
                }

                // Check if resident has pending payments
                var hasPendingPayments = await _unitOfWork.Payments.ExistsAsync(p =>
                    p.ResidentId == id && p.Status == Domain.Enums.PaymentStatus.Pending);
                if (hasPendingPayments)
                {
                    return Result.Failure("Cannot delete resident with pending payments");
                }

                resident.IsDeleted = true;
                resident.UpdatedAt = DateTime.UtcNow;
                resident.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Residents.UpdateAsync(resident);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Resident deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error deleting resident: {ex.Message}");
            }
        }

        public async Task<Result> ActivateAsync(Guid id)
        {
            try
            {
                var resident = await _unitOfWork.Residents.GetByIdAsync(id);
                if (resident == null || resident.IsDeleted)
                {
                    return Result.Failure("Resident not found");
                }

                resident.IsActive = true;
                resident.UpdatedAt = DateTime.UtcNow;
                resident.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Residents.UpdateAsync(resident);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Resident activated successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error activating resident: {ex.Message}");
            }
        }

        public async Task<Result> DeactivateAsync(Guid id)
        {
            try
            {
                var resident = await _unitOfWork.Residents.GetByIdAsync(id);
                if (resident == null || resident.IsDeleted)
                {
                    return Result.Failure("Resident not found");
                }

                resident.IsActive = false;
                resident.UpdatedAt = DateTime.UtcNow;
                resident.UpdatedBy = _currentUserService.UserName;

                await _unitOfWork.Residents.UpdateAsync(resident);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Resident deactivated successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error deactivating resident: {ex.Message}");
            }
        }

        public async Task<Result> MoveToUnitAsync(Guid residentId, Guid fromUnitId, Guid toUnitId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Remove from current unit
                var removeResult = await RemoveFromUnitAsync(residentId, fromUnitId);
                if (!removeResult.IsSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return removeResult;
                }

                // Add to new unit
                var assignResult = await AssignToUnitAsync(residentId, toUnitId, false, true);
                if (!assignResult.IsSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return assignResult;
                }

                await _unitOfWork.CommitTransactionAsync();
                return Result.Success("Resident moved to new unit successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"Error moving resident: {ex.Message}");
            }
        }

        public async Task<Result<List<ResidentDto>>> GetResidentsWithoutUnitsAsync()
        {
            try
            {
                var allResidents = await _unitOfWork.Residents.GetAsync(r => r.IsActive && !r.IsDeleted);
                var residentUnits = await _unitOfWork.ResidentUnits.GetAsync(ru => ru.IsActive);
                var assignedResidentIds = residentUnits.Select(ru => ru.ResidentId).Distinct();

                var residentsWithoutUnits = allResidents.Where(r => !assignedResidentIds.Contains(r.Id));
                var residentDtos = _mapper.Map<List<ResidentDto>>(residentsWithoutUnits);

                return Result<List<ResidentDto>>.Success(residentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<ResidentDto>>.Failure($"Error retrieving residents without units: {ex.Message}");
            }
        }
    }
}