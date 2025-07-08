using NET.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace NET.Application.Common.Interfaces
{
    public interface ITenantService
    {
        Guid GetCurrentTenantId();
        Task<Tenant?> GetCurrentTenantAsync();
        void SetTenant(Guid tenantId);
        void SetTenant(string subdomain);
        Task<bool> ValidateTenantAsync(Guid tenantId);
        Task<Tenant?> GetTenantBySubdomainAsync(string subdomain);
    }
}