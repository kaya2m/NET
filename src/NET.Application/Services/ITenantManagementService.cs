//using NET.Application.Common.Models;
//using NET.Application.DTOs.Tenant;

//namespace NET.Application.Services
//{
//    public interface ITenantManagementService
//    {
//        Task<Result<TenantDto>> GetByIdAsync(Guid id);
//        Task<Result<TenantDto>> GetBySubdomainAsync(string subdomain);
//        Task<Result<PagedResult<TenantDto>>> GetAllAsync(int page = 1, int pageSize = 10);
//        Task<Result<List<TenantDto>>> GetActiveTenantsAsync();
//        Task<Result<TenantDto>> CreateAsync(CreateTenantDto dto);
//        Task<Result<TenantDto>> UpdateAsync(Guid id, UpdateTenantDto dto);
//        Task<Result> DeleteAsync(Guid id);
//        Task<Result> ActivateAsync(Guid id);
//        Task<Result> DeactivateAsync(Guid id);

//        // Subscription Management
//        Task<Result> ExtendSubscriptionAsync(Guid tenantId, DateTime newEndDate);
//        Task<Result> UpdateLimitsAsync(Guid tenantId, int maxBuildings, int maxUnits);
//        Task<Result<List<TenantDto>>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry = 30);

//        // Tenant Statistics
//        Task<Result<TenantStatisticsDto>> GetTenantStatisticsAsync(Guid tenantId);
//        Task<Result<TenantUsageDto>> GetTenantUsageAsync(Guid tenantId);

//        // Validation
//        Task<Result<bool>> ValidateSubdomainAsync(string subdomain);
//        Task<Result<bool>> CheckLimitsAsync(Guid tenantId);
//    }
//}