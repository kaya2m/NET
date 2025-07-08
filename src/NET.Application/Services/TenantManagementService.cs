//using AutoMapper;
//using NET.Application.Common.Interfaces;
//using NET.Application.Common.Models;
//using NET.Application.DTOs.Tenant;
//using NET.Domain.Entities;
//using NET.Domain.Enums;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace NET.Application.Services
//{
//    public class TenantManagementService : ITenantManagementService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//        private readonly ICurrentUserService _currentUserService;

//        public TenantManagementService(
//            IUnitOfWork unitOfWork,
//            IMapper mapper,
//            ICurrentUserService currentUserService)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//            _currentUserService = currentUserService;
//        }

//        //public async Task<Result<TenantDto>> GetByIdAsync(Guid id)
////        {
////            try
////            {
////                var tenant = await _unitOfWork.Tenants.GetFirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
////                if (tenant == null)
////                {
////                    return Result<TenantDto>.Failure("Tenant not found");
////                }

////                var tenantDto = _mapper.Map<TenantDto>(tenant);

////                // Get usage statistics
////                tenantDto.CurrentBuildingsCount = await _unitOfWork.Buildings.CountAsync(b => b.TenantId == id && !b.IsDeleted);
////                tenantDto.CurrentUnitsCount = await _unitOfWork.BuildingUnits.CountAsync(u => u.TenantId == id && !u.IsDeleted);
////                tenantDto.CurrentUsersCount = await _unitOfWork.Users.CountAsync(u => u.TenantId == id && !u.IsDeleted);

////                return Result<TenantDto>.Success(tenantDto);
////            }
////            catch (Exception ex)
////            {
////                return Result<TenantDto>.Failure($"Error retrieving tenant: {ex.Message}");
////            }
////        }

////        public async Task<Result<TenantDto>> GetBySubdomainAsync(string subdomain)
////        {
////            try
////            {
////                var tenant = await _unitOfWork.Tenants.GetFirstOrDefaultAsync(t => t.Subdomain == subdomain && !t.IsDeleted);
////                if (tenant == null)
////                {
////                    return Result<TenantDto>.Failure("Tenant not found");
////                }

////                var tenantDto = _mapper.Map<TenantDto>(tenant);
////                return Result<TenantDto>.Success(tenantDto);
////            }
////            catch (Exception ex)
////            {
////                return Result<TenantDto>.Failure($"Error retrieving tenant by subdomain: {ex.Message}");
////            }
////        }

////        public async Task<Result<PagedResult<TenantDto>>> GetAllAsync(int page = 1, int pageSize = 10)
////        {
////            try
////            {
////                var tenants = await _unitOfWork.Tenants.GetPagedAsync(page, pageSize, t => !t.IsDeleted);
////                var tenantDtos = _mapper.Map<IEnumerable<TenantDto>>(tenants.Data);
////                var pagedResult = new PagedResult<TenantDto>(tenantDtos, page, pageSize, tenants.TotalCount);

////                return Result<PagedResult<TenantDto>>.Success(pagedResult);
////            }
////            catch (Exception ex)
////            {
////                return Result<PagedResult<TenantDto>>.Failure($"Error retrieving tenants: {ex.Message}");
////            }
////        }

////        public async Task<Result<TenantDto>> CreateAsync(CreateTenantDto dto)
////        {
////            try
////            {
////                // Check if subdomain already exists
////                var subdomainExists = await _unitOfWork.Tenants.ExistsAsync(t => t.Subdomain == dto.Subdomain && !t.IsDeleted);
////                if (subdomainExists)
////                {
////                    return Result<TenantDto>.Failure("Subdomain already exists");
////                }

////                var tenant = _mapper.Map<Tenant>(dto);
////                tenant.CreatedBy = _currentUserService.UserName;

////                var createdTenant = await _unitOfWork.Tenants.AddAsync(tenant);
////                await _unitOfWork.SaveChangesAsync();

////                var tenantDto = _mapper.Map<TenantDto>(createdTenant);
////                return Result<TenantDto>.Success(tenantDto, "Tenant created successfully");
////            }
////            catch (Exception ex)
////            {
////                return Result<TenantDto>.Failure($"Error creating tenant: {ex.Message}");
////            }
////        }

////        public async Task<Result<TenantDto>> UpdateAsync(Guid id, UpdateTenantDto dto)
////        {
////            try
////            {
////                var tenant = await _unitOfWork.Tenants.GetByIdAsync(id);
////                if (tenant == null || tenant.IsDeleted)
////                {
////                    return Result<TenantDto>.Failure("Tenant not found");
////                }

////                _mapper.Map(dto, tenant);
////                tenant.UpdatedAt = DateTime.UtcNow;
////                tenant.UpdatedBy = _currentUserService.UserName;

////                await _unitOfWork.Tenants.UpdateAsync(tenant);
////                await _unitOfWork.SaveChangesAsync();

////                var tenantDto = _mapper.Map<TenantDto>(tenant);
////                return Result<TenantDto>.Success(tenantDto, "Tenant updated successfully");
////            }
////            catch (Exception ex)
////            {
////                return Result<TenantDto>.Failure($"Error updating tenant: {ex.Message}");
////            }
////        }

////        public async Task<Result> DeleteAsync(Guid id)
////        {
////            try
////            {
////                var tenant = await _unitOfWork.Tenants.GetByIdAsync(id);
////                if (tenant == null || tenant.IsDeleted)
////                {
////                    return Result.Failure("Tenant not found");
////                }

////                // Check if tenant has buildings
////                var hasBuildings = await _unitOfWork.Buildings.ExistsAsync(b => b.TenantId == id && !b.IsDeleted);
////                if (hasBuildings)
////                {
////                    return Result.Failure("Cannot delete tenant with existing buildings");
////                }

////                tenant.IsDeleted = true;
////                tenant.UpdatedAt = DateTime.UtcNow;
////                tenant.UpdatedBy = _currentUserService.UserName;

////                await _unitOfWork.Tenants.UpdateAsync(tenant);
////                await _unitOfWork.SaveChangesAsync();

////                return Result.Success("Tenant deleted successfully");
////            }
////            catch (Exception ex)
////            {
////                return Result.Failure($"Error deleting tenant: {ex.Message}");
////            }
////        }

////        public async Task<Result<List<TenantDto>>> GetActiveTenantsAsync()
////        {
////            try
////            {
////                var tenants = await _unitOfWork.Tenants.GetAsync(t => t.IsActive && !t.IsDeleted);
////                var tenantDtos = _mapper.Map<List<TenantDto>>(tenants);
////                return Result<List<TenantDto>>.Success(tenantDtos);
////            }
////            catch (Exception ex)
////            {
////                return Result<List<TenantDto>>.Failure($"Error retrieving active tenants: {ex.Message}");
////            }
////        }

////        public async Task<Result> ActivateAsync(Guid id)
////        {
////            try
////            {
////                var tenant = await _unitOfWork.Tenants.GetByIdAsync(id);
////                if (tenant == null || tenant.IsDeleted)
////                {
////                    return Result.Failure("Tenant not found");
////                }

////                tenant.IsActive = true;
////                tenant.UpdatedAt = DateTime.UtcNow;
////                tenant.UpdatedBy = _currentUserService.UserName;

////                await _unitOfWork.Tenants.UpdateAsync(tenant);
////                await _unitOfWork.SaveChangesAsync();

////                return Result.Success("Tenant activated successfully");
////            }
////            catch (Exception ex)
////            {
////                return Result.Failure($"Error activating tenant: {ex.Message}");
////            }
////        }

////        public async Task<Result> DeactivateAsync(Guid id)
////        {
////            try
////            {
////                var tenant = await _unitOfWork.Tenants.GetByIdAsync(id);
////                if (tenant == null || tenant.IsDeleted)
////                {
////                    return Result.Failure("Tenant not found");
////                }

////                tenant.IsActive = false;
////                tenant.UpdatedAt = DateTime.UtcNow;
////                tenant.UpdatedBy = _currentUserService.UserName;

////                await _unitOfWork.Tenants.UpdateAsync(tenant);
////                await _unitOfWork.SaveChangesAsync();

////                return Result.Success("Tenant deactivated successfully");
////            }
////            catch (Exception ex)
////            {
////                return Result.Failure($"Error deactivating tenant: {ex.Message}");
////            }
////        }

////        public async Task<Result> ExtendSubscriptionAsync(Guid tenantId, DateTime newEndDate)
////        {
////            try
////            {
////                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
////                if (tenant == null || tenant.IsDeleted)
////                {
////                    return Result.Failure("Tenant not found");
////                }

////                tenant.SubscriptionEndDate = newEndDate;
////                tenant.UpdatedAt = DateTime.UtcNow;
////                tenant.UpdatedBy = _currentUserService.UserName;

////                await _unitOfWork.Tenants.UpdateAsync(tenant);
////                await _unitOfWork.SaveChangesAsync();

////                return Result.Success("Subscription extended successfully");
////            }
////            catch (Exception ex)
////            {
////                return Result.Failure($"Error extending subscription: {ex.Message}");
////            }
////        }

////        public async Task<Result> UpdateLimitsAsync(Guid tenantId, int maxBuildings, int maxUnits)
////        {
////            try
////            {
////                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
////                if (tenant == null || tenant.IsDeleted)
////                {
////                    return Result.Failure("Tenant not found");
////                }

////                tenant.MaxBuildings = maxBuildings;
////                tenant.MaxUnits = maxUnits;
////                tenant.UpdatedAt = DateTime.UtcNow;
////                tenant.UpdatedBy = _currentUserService.UserName;

////                await _unitOfWork.Tenants.UpdateAsync(tenant);
////                await _unitOfWork.SaveChangesAsync();

////                return Result.Success("Limits updated successfully");
////            }
////            catch (Exception ex)
////            {
////                return Result.Failure($"Error updating limits: {ex.Message}");
////            }
////        }

////        public async Task<Result<List<TenantDto>>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry = 30)
////        {
////            try
////            {
////                var expiryDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);
////                var tenants = await _unitOfWork.Tenants.GetAsync(t =>
////                    t.SubscriptionEndDate.HasValue &&
////                    t.SubscriptionEndDate.Value <= expiryDate &&
////                    t.IsActive &&
////                    !t.IsDeleted);

////                var tenantDtos = _mapper.Map<List<TenantDto>>(tenants);
////                return Result<List<TenantDto>>.Success(tenantDtos);
////            }
////            catch (Exception ex)
////            {
////                return Result<List<TenantDto>>.Failure($"Error retrieving expiring subscriptions: {ex.Message}");
////            }
////        }

////        public async Task<Result<TenantStatisticsDto>> GetTenantStatisticsAsync(Guid tenantId)
////        {
////            try
////            {
////                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
////                if (tenant == null || tenant.IsDeleted)
////                {
////                    return Result<TenantStatisticsDto>.Failure("Tenant not found");
////                }

////                var buildings = await _unitOfWork.Buildings.GetAsync(b => b.TenantId == tenantId && !b.IsDeleted);
////                var units = await _unitOfWork.BuildingUnits.GetAsync(u => u.TenantId == tenantId && !u.IsDeleted);
////                var residents = await _unitOfWork.Residents.GetAsync(r => r.TenantId == tenantId && !r.IsDeleted);
////                var users = await _unitOfWork.Users.GetAsync(u => u.TenantId == tenantId && !u.IsDeleted);

////                var payments = await _unitOfWork.Payments.GetAsync(p =>
////                    p.TenantId == tenantId &&
////                    p.PaymentDate >= DateTime.UtcNow.AddMonths(-1) &&
////                    !p.IsDeleted);

////                var expenses = await _unitOfWork.Expenses.GetAsync(e =>
////                    e.TenantId == tenantId &&
////                    e.ExpenseDate >= DateTime.UtcNow.AddMonths(-1) &&
////                    !e.IsDeleted);

////                var maintenanceRequests = await _unitOfWork.MaintenanceRequests.CountAsync(mr =>
////                    mr.TenantId == tenantId &&
////                    mr.Status == MaintenanceStatus.Open &&
////                    !mr.IsDeleted);

////                var statistics = new TenantStatisticsDto
////                {
////                    TenantId = tenantId,
////                    TenantName = tenant.Name,
////                    TotalBuildings = buildings.Count(),
////                    TotalUnits = units.Count(),
////                    TotalResidents = residents.Count(),
////                    TotalUsers = users.Count(),
////                    TotalRevenue = payments.Sum(p => p.Amount),
////                    TotalExpenses = expenses.Sum(e => e.Amount),
////                    ActiveMaintenanceRequests = maintenanceRequests,
////                    LastActivity = users.Max(u => u.LastLoginDate) ?? DateTime.MinValue
////                };

////                return Result<TenantStatisticsDto>.Success(statistics);
////            }
////            catch (Exception ex)
////            {
////                return Result<TenantStatisticsDto>.Failure($"Error generating tenant statistics: {ex.Message}");
////            }
////        }

////        public async Task<Result<TenantUsageDto>> GetTenantUsageAsync(Guid tenantId)
////        {
////            try
////            {
////                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
////                if (tenant == null || tenant.IsDeleted)
////                {
////                    return Result<TenantUsageDto>.Failure("Tenant not found");
////                }

////                var buildingsUsed = await _unitOfWork.Buildings.CountAsync(b => b.TenantId == tenantId && !b.IsDeleted);
////                var unitsUsed = await _unitOfWork.BuildingUnits.CountAsync(u => u.TenantId == tenantId && !u.IsDeleted);

////                var warnings = new List<string>();
////                if (buildingsUsed >= tenant.MaxBuildings)
////                {
////                    warnings.Add("Building limit reached");
////                }
////                if (unitsUsed >= tenant.MaxUnits)
////                {
////                    warnings.Add("Unit limit reached");
////                }

////                var usage = new TenantUsageDto
////                {
////                    TenantId = tenantId,
////                    BuildingsUsed = buildingsUsed,
////                    MaxBuildings = tenant.MaxBuildings,
////                    UnitsUsed = unitsUsed,
////                    MaxUnits = tenant.MaxUnits,
////                    StorageUsed = 0, // TODO: Implement storage calculation
////                    MaxStorage = 1000, // TODO: Get from tenant configuration
////                    IsWithinLimits = warnings.Count == 0,
////                    LimitWarnings = warnings
////                };

////                return Result<TenantUsageDto>.Success(usage);
////            }
////            catch (Exception ex)
////            {
////                return Result<TenantUsageDto>.Failure($"Error getting tenant usage: {ex.Message}");
////            }
////        }

////        public async Task<Result<bool>> ValidateSubdomainAsync(string subdomain)
////        {
////            try
////            {
////                var exists = await _unitOfWork.Tenants.ExistsAsync(t => t.Subdomain == subdomain && !t.IsDeleted);
////                return Result<bool>.Success(!exists);
////            }
////            catch (Exception ex)
////            {
////                return Result<bool>.Failure($"Error validating subdomain: {ex.Message}");
////            }
////        }

////        public async Task<Result<bool>> CheckLimitsAsync(Guid tenantId)
////        {
////            try
////            {
////                var usage = await GetTenantUsageAsync(tenantId);
////                if (!usage.IsSuccess)
////                {
////                    return Result<bool>.Failure(usage.Message);
////                }

////                return Result<bool>.Success(usage.Data!.IsWithinLimits);
////            }
////            catch (Exception ex)
////            {
////                return Result<bool>.Failure($"Error checking limits: {ex.Message}");
////            }
////        }
////    }
////}